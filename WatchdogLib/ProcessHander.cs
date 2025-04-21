using NLog;
using System;
using System.Diagnostics;
using System.IO;
using Utilities;

namespace WatchdogLib
{

	// todo: http://msdn.microsoft.com/en-us/library/system.diagnostics.process.exitcode(v=vs.110).aspx
	//http://stackoverflow.com/questions/2279181/catch-another-process-unhandled-exception
	// http://social.msdn.microsoft.com/Forums/vstudio/en-US/62259e21-3280-4d10-a27c-740d35efe51c/catch-another-process-unhandled-exception?forum=csharpgeneral
	public class ProgressEventArgs : EventArgs
	{
		public float Progress { get; private set; }
		public Process Process { get; private set; }
		public ProgressEventArgs(float progress, Process process)
		{
			Progress = progress;
			Process = process;
		}
	}

	public class ProcessMessageArgs : EventArgs
	{
		public string Message { get; private set; }
		public Process Process { get; private set; }
		public ProcessMessageArgs(string message, Process process)
		{
			Message = message;
			Process = process;
		}
	}

	public class ProcessStatusArgs : EventArgs
	{
		public int ExitCode { get; private set; }
		public Process Process { get; private set; }
		public ProcessStatusArgs(int exitCode, Process process)
		{
			ExitCode = exitCode;
			Process = process;
		}
	}
	
	/// <summary>
	/// ProcessHandler is a class that handles the execution of a process and monitors its status.
	/// </summary>
	public class ProcessHandler
	{

		private readonly object _exitedLock = new object();
		
		/// <summary>
		/// The stopwatch for checking if the process is non-responsive.
		/// </summary>
		private Stopwatch _nonresponsiveInterval;
		
		/// <summary>
		/// The stopwatch for checking if the process has started.
		/// </summary>
		private Stopwatch _fromStart;

		public DataReceivedEventHandler OutputHandler;
		public EventHandler<ProcessMessageArgs> ErrorOutputHandler;

		public event EventHandler<ProcessStatusArgs> ExitHandler;
		public event EventHandler<ProcessMessageArgs> ErrorHandler;


		/// <summary>
		/// The path to the executable to be run.
		/// </summary>
		public string Executable { get; set; }
		
		/// <summary>
		/// The arguments to be passed to the executable.
		/// </summary>
		public string Args { get; set; }
		
		/// <summary>
		/// Boolean that indicates if the process should wait for exit.
		/// </summary>
		public bool WaitForExit { get; set; }
		
		/// <summary>
		/// Boolean that indicates if the process should run in the directory of the executable.
		/// </summary>
		public bool RunInDir { get; set; }
		
		/// <summary>
		/// The interval in seconds to wait before considering the process as non-responsive.
		/// </summary>
		public int NonResponsiveInterval { get; set; }
		
		/// <summary>
		/// The interval in seconds to wait before considering the process as started.
		/// </summary>
		public uint StartingInterval { get; set; }

		/// <summary>
		/// Boolean that indicates if the process has exited.
		/// </summary>
		public bool HasExited
		{
			get { return (Process == null) || Process.HasExited; }
		}
		
		/// <summary>
		/// Boolean that indicates if the process is responding.
		/// </summary>
		public bool Responding
		{
			get
			{
				// If the other process changes its main window,
				// the check of Process.Responding might return false if we don't .Refresh() it
				Process.Refresh();
				// todo: add heartbeat
				if (!Process.Responding)
				{
					if (!_nonresponsiveInterval.IsRunning) _nonresponsiveInterval.Restart();
				}
				else
				{
					_nonresponsiveInterval.Reset();
				}

				return (Process == null) || Process.Responding;
			}
		}

		/// <summary>
		/// Constructor for the ProcessHandler class
		/// that sets default values for the properties.
		/// </summary>
		public ProcessHandler()
		{
			WaitForExit = true;
			RunInDir = true;
			NonResponsiveInterval = 2000;
			StartingInterval = 5000;
			_nonresponsiveInterval = new Stopwatch();
			_fromStart = new Stopwatch();
		}

		/// <summary>
		/// Method to call an executable with the specified arguments.
		/// </summary>
		/// <param name="executable">Path of the executable</param>
		/// <param name="args">Arguments for the executable</param>
		public void CallExecutable(string executable, string args, Logger logger)
		{
			Args = args;
			Executable = executable;
			CallExecutable(logger);
		}

		/// <summary>
		/// Method to monitor a process.
		/// </summary>
		/// <param name="process">The monitored process</param>
		/// <returns>return true if an exception isn't catched</returns>
		public bool MonitorProcess(Process process, Logger logger)
		{
			Process = process;
			try
			{
				Process.OutputDataReceived += Output;
				Process.ErrorDataReceived += OutputError;
				Name = Process.ProcessName;
				_fromStart.Restart();
				if (WaitForExit)
				{
					Process.WaitForExit();
					EndProcess();
				}
				else
				{
					Process.Exited += ProcessExited;
				}
			}
			catch (Exception ex)
			{
				if (ErrorHandler != null) ErrorHandler(this, new ProcessMessageArgs(ex.Message, Process));
				if (Process.HasExited)
				{
					if (ExitHandler != null) ExitHandler(this, new ProcessStatusArgs(-1, Process));
					return false;
				}

			}
			return true;
		}
		
		/// <summary>
		/// Method to end the process.
		/// </summary>
		private void EndProcess()
		{
			if (Process == null) return;
			Process.Close();
			Process.Dispose();
			Process = null;
		}

		/// <summary>
		/// Method to call an executable with the atributes from the processHandler.
		/// </summary>
		public void CallExecutable(Logger logger)
		{

			if (!File.Exists(Executable))
			{
				logger.Fatal("Error, ejecutable inexistente: " + Executable );
				return;
			}
			var commandLine = Executable;
			logger.Error("Running command: " + Executable + " " + Args);
			var psi = new ProcessStartInfo(commandLine)
			{
				UseShellExecute = false,
				LoadUserProfile = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				Arguments = Args
			};
			if (RunInDir)
			{
				var path = Path.GetDirectoryName(Executable);
				if (path != null) psi.WorkingDirectory = path;
			}
			Process = new Process { StartInfo = psi };
			try
			{
				Process.Start();
				Process.BeginOutputReadLine();
				Process.BeginErrorReadLine();
				Process.OutputDataReceived += Output;
				Process.ErrorDataReceived += OutputError;
				_fromStart.Restart();
				Name = Process.ProcessName;

				// Watch process for not responding
				if (WaitForExit)
				{
					Process.WaitForExit();
					EndProcess();
				}
				else
				{
					Process.Exited += ProcessExited;
				}
			}
			catch (Exception ex)
			{
				if (ErrorHandler != null) ErrorHandler(this, new ProcessMessageArgs(ex.Message, Process));
				if (!ProcessUtils.ProcessRunning(Executable))
				{
					if (ExitHandler != null) ExitHandler(this, new ProcessStatusArgs(-1, Process));
				}

			}
		}
		
		/// <summary>
		/// Set Running to true when the process has exited.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProcessExited(object sender, EventArgs e)
		{
			Running = true;
		}
		
		/// <summary>
		/// boolean that indicates if the process is running.
		/// </summary>
		public bool Running { get; set; }

		/// <summary>
		/// The process that is being monitored.
		/// </summary>
		public Process Process { get; private set; }
		
		/// <summary>
		/// The name of the processHandler.
		/// </summary>
		public string Name { get; private set; }
		
		/// <summary>
		/// boolean that indicates if the process is not responding after the specified interval.
		/// </summary>
		public bool NotRespondingAfterInterval
		{
			get { return (!Responding && _nonresponsiveInterval.ElapsedMilliseconds > NonResponsiveInterval); }
		}
		
		/// <summary>
		/// boolean that indicates if the process is starting after the specified interval.
		/// </summary>
		public bool IsStarting
		{
			get { return (_fromStart.ElapsedMilliseconds < StartingInterval); }
		}

		/// <summary>
		/// Method that call to private EndProcess method to end the process.
		/// </summary>
		public void Close()
		{
			EndProcess();
		}

		private void Output(object sender, DataReceivedEventArgs dataReceivedEventArgs)
		{
			if (string.IsNullOrEmpty(dataReceivedEventArgs.Data)) return;

			var output = dataReceivedEventArgs.Data;
			// Fire Output event
			if (OutputHandler != null) OutputHandler(sender, dataReceivedEventArgs);
		}

		private void OutputError(object sender, DataReceivedEventArgs dataReceivedEventArgs)
		{
			if (string.IsNullOrEmpty(dataReceivedEventArgs.Data)) return;
			// Fire OutputError event
			var progressEventArgs = new ProcessMessageArgs(dataReceivedEventArgs.Data, Process);
			if (ErrorOutputHandler != null) ErrorOutputHandler(sender, progressEventArgs);
		}
		
		/// <summary>
		/// Kill the process.
		/// </summary>
		/// <returns></returns>
		public bool Kill()
		{
			return ProcessUtils.KillProcess(Process);
			// Todo set state indicating that kill has been tried
		}
	}


}
