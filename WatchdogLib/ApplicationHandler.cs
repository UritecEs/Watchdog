using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NLog;
using Utilities;

namespace WatchdogLib
{
	/// <summary>
	/// Provides functionality to monitor, manage, and restart application processes.
	/// It supports checking for non-responsive processes via heartbeat signals,
	/// handling duplicate and exited processes, and ensuring that the number of processes
	/// stays within predefined minimum and maximum limits.
	/// </summary>
	public class ApplicationHandler : IEquatable<ApplicationHandler>
	{
		private readonly HeartbeatServer _heartbeatServer;
		//private ApplicationHandlerConfig _applicationHandlerConfig;

		/// <summary>
		/// Gets the unique identifier for this ApplicationHandler instance.
		/// </summary>
		public Guid Id { get; private set; }
		
		/// <summary>
		/// Gets or sets the list of process handlers monitoring the application processes.
		/// </summary>
		private List<ProcessHandler> ProcessHandlers { get; set; }
		
		/// <summary>
		/// Gets or sets the time interval (in ms) used to decide when a process is considered non-responsive.
		/// </summary>
		public int NonResponsiveInterval { get; set; }
		
		/// <summary>
		/// Gets or sets the path to the application executable.
		/// </summary>
		public string ApplicationPath { get; set; }
		
		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		public string ApplicationName { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether heartbeat monitoring is enabled.
		/// </summary>
		public bool UseHeartbeat { get; set; }
		/// <summary>
		/// Gets or sets the logger instance used to record messages for monitoring purposes.
		/// </summary>
		private Logger Logger { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether the handler is permitted to issue kill requests for processes.
		/// </summary>
		public bool GrantKillRequest { get; set; }
		
		/// <summary>
		/// Gets or sets the interval (in seconds) used by the heartbeat mechanism to check process responsiveness.
		/// </summary>
		public uint HeartbeatInterval { get; set; }
		
		/// <summary>
		/// Gets or sets the maximum allowed number of running processes.
		/// </summary>
		public int MaxProcesses { get; set; }
		
		/// <summary>
		/// Gets or sets the minimum required number of running processes.
		/// </summary>
		public int MinProcesses { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether this handler is actively monitoring processes.
		/// </summary>
		public bool Active { get; set; }
		
		/// <summary>
		/// Gets or sets a value that determines whether to maintain the existing process count even if no process is currently running.
		/// </summary>
		public bool KeepExistingNoProcesses { get; set; }
		
		/// <summary>
		/// Gets or sets the delay (in ms) to wait after startup before beginning process monitoring.
		/// </summary>
		public uint StartupMonitorDelay { get; set; }
		/*
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationHandler"/> class using specified parameters.
		/// </summary>
		/// <param name="applicationName">The name of the application.</param>
		/// <param name="applicationPath">The file system path to the application executable.</param>
		/// <param name="nonResponsiveInterval">The interval defining non-responsive behavior.</param>
		/// <param name="heartbeatInterval">The heartbeat check interval (default is 15 seconds).</param>
		/// <param name="minProcesses">The minimum number of processes required (default is 1).</param>
		/// <param name="maxProcesses">The maximum allowed processes (default is 1).</param>
		/// <param name="keepExistingNoProcesses">Whether to maintain existing processes if none are running (default is false).</param>
		/// <param name="useHeartbeat">Whether heartbeat monitoring is enabled (default is false).</param>
		/// <param name="grantKillRequest">Whether process kill requests are permitted (default is true).</param>
		/// <param name="startupMonitorDelay">The delay before starting process monitoring (default is 20 seconds).</param>
		/// <param name="active">Specifies if the monitoring should be active immediately (default is true).</param>
		public ApplicationHandler(string applicationName, string applicationPath, int nonResponsiveInterval, uint heartbeatInterval = 15,
			int minProcesses = 1, int maxProcesses = 1, bool keepExistingNoProcesses = false, bool useHeartbeat = false,
			bool grantKillRequest = true, uint startupMonitorDelay = 20, bool active = true)
		{
			Id = Guid.NewGuid();
			Logger = LogManager.GetLogger("WatchdogServer");
			ProcessHandlers = new List<ProcessHandler>();
			ApplicationName = applicationName;
			ApplicationPath = applicationPath;
			NonResponsiveInterval = nonResponsiveInterval * 1000;
			HeartbeatInterval = heartbeatInterval;
			MinProcesses = minProcesses;
			MaxProcesses = maxProcesses;
			KeepExistingNoProcesses = keepExistingNoProcesses;
			UseHeartbeat = useHeartbeat;
			GrantKillRequest = grantKillRequest;
			StartupMonitorDelay = startupMonitorDelay * 1000;

			_heartbeatServer = HeartbeatServer.Instance;
			Active = active;
		}
		*/
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationHandler"/> class using a configuration object.
		/// </summary>
		/// <param name="applicationHandlerConfig">An object containing configuration settings for the application handler.</param>
		public ApplicationHandler(ApplicationHandlerConfig applicationHandlerConfig)
		{
			Id = applicationHandlerConfig.Id;
			Logger = LogManager.GetLogger("WatchdogServer");
			ProcessHandlers = new List<ProcessHandler>();
			SetConfiguration(applicationHandlerConfig);
			_heartbeatServer = HeartbeatServer.Instance;
		}

		/// <summary>
		/// Updates the properties of this application handler using values from the given configuration.
		/// </summary>
		/// <param name="applicationHandlerConfig">The configuration object with updated settings.</param>
		public void SetConfiguration(ApplicationHandlerConfig applicationHandlerConfig)
		{
			Id = applicationHandlerConfig.Id;
			ApplicationName = applicationHandlerConfig.ApplicationName;
			ApplicationPath = applicationHandlerConfig.ApplicationPath;
			NonResponsiveInterval = applicationHandlerConfig.NonResponsiveInterval * 1000;
			HeartbeatInterval = applicationHandlerConfig.HeartbeatInterval;
			MinProcesses = applicationHandlerConfig.MinProcesses;
			MaxProcesses = applicationHandlerConfig.MaxProcesses;
			KeepExistingNoProcesses = applicationHandlerConfig.KeepExistingNoProcesses;
			UseHeartbeat = applicationHandlerConfig.UseHeartbeat;
			GrantKillRequest = applicationHandlerConfig.GrantKillRequest;
			Active = applicationHandlerConfig.Active;
			StartupMonitorDelay = applicationHandlerConfig.StartupMonitorDelay * 1000;
			Active = applicationHandlerConfig.Active;

			// Update the `NonResponsiveInterval` of each ProcessHandler to match the new configuration.
			foreach (var processHandler in ProcessHandlers)
			{
				processHandler.NonResponsiveInterval = NonResponsiveInterval;
				processHandler.StartingInterval = StartupMonitorDelay;
			}
		}

		/// <summary>
		/// Performs periodic checks on the monitored processes to ensure they are running, responsive,
		/// and within the allowed process count limits.
		/// </summary>
		public void Check()
		{
			if (!Active) return;
			// Check if  new unmonitored process is running 
			HandleDuplicateProcesses();
			HandleNonResponsiveProcesses();
			HandleExitedProcesses();
			HandleUnmonitoredProcesses();
			HandleProcessNotRunning();
		}

		/// <summary>
		/// Checks if no processes are running and starts a new process if needed.
		/// </summary>
		private void HandleProcessNotRunning()
		{
			var processes = Process.GetProcessesByName(ApplicationName);

			if (processes.Length == 0)
			{
				// Start new process
				var processHandler = new ProcessHandler
				{
					WaitForExit = false,
					NonResponsiveInterval = NonResponsiveInterval,
					StartingInterval = StartupMonitorDelay
				};
				Logger.Info("No process of application {0} is running, so one will be started", ApplicationName);
				processHandler.CallExecutable(ApplicationPath, "", Logger);
				ProcessHandlers.Add(processHandler);
			}
		}
		
		/// <summary>
		/// Iterates through monitored processes and handles those that have exited.
		/// </summary>
		private void HandleExitedProcesses()
		{
			for (int index = 0; index < ProcessHandlers.Count; index++)
			{
				var processHandler = ProcessHandlers[index];
				if (processHandler.HasExited)
				{
					Logger.Warn("Process {0} has exited", processHandler.Name);
					//Debug.WriteLine("{0} has exited", processHandler.Name);
					processHandler.Close();

					var notEnoughProcesses = (ProcessNo(processHandler.Name) < MinProcesses);
					var lessProcessesThanBefore = (ProcessNo(processHandler.Name) < MaxProcesses) && KeepExistingNoProcesses;

					if (notEnoughProcesses || lessProcessesThanBefore)
					{
						if (notEnoughProcesses) Logger.Info("Process {0} has exited and no others are running, so start new", processHandler.Name);
						if (lessProcessesThanBefore) Logger.Info("Process {0} has exited, and number of processed needs to maintained , so start new", processHandler.Name);
						processHandler.CallExecutable(Logger);
					}
					else
					{
						Logger.Info("Process {0} has exited, but no requirement to start new one", processHandler.Name);
						ProcessHandlers.Remove(processHandler);
					}
				}
			}
		}

		/// <summary>
		/// Checks each monitored process for non-responsiveness using heartbeat signals and response intervals.
		/// If a process is non-responsive or a kill request has been issued, the process is terminated.
		/// </summary>
		private void HandleNonResponsiveProcesses()
		{
			for (var index = 0; index < ProcessHandlers.Count; index++)
			{
				var processHandler = ProcessHandlers[index];

				// Skip processes that have already exited.
				if (processHandler.HasExited) continue; // We will deal with this later
				try
				{
					if (processHandler.Process.HasExited)
						continue;
				}
				catch (Exception)
				{
					continue;
				}

				// If still in startup phase, skip checking.
				if (processHandler.IsStarting)
				{
					//Logger.Info("Process {0} is still in startup phase, no checking is being performed", processHandler.Name);
					Debug.WriteLine("Process {0} is still in startup phase, no checking is being performed", processHandler.Name);
					continue; // Is still starting up, so ignore
				}
				// Check heartbeat soft limit.
				if (UseHeartbeat && _heartbeatServer.HeartbeatTimedOut(processHandler.Process.Id, HeartbeatInterval / 2))
				{
					//todo: add throttling
					Logger.Warn("No heartbeat received from process {0} within the soft limit", processHandler.Name);
					//Debug.WriteLine("Process {0} has no heartbeat within soft limit", processHandler.Name);
				}

				//if (processHandler.Responding && !_heartbeatServer.HeartbeatHardTimeout(processHandler.Name)) continue;
				//Debug.WriteLine("Process {0} not responding", processHandler.Name);
				
				var notRespondingAfterInterval = processHandler.NotRespondingAfterInterval;
				var noHeartbeat = UseHeartbeat && _heartbeatServer.HeartbeatTimedOut(processHandler.Process.Id, HeartbeatInterval);
				var requestedKill = _heartbeatServer.KillRequested(processHandler.Process.Id);

				var performKill = notRespondingAfterInterval || noHeartbeat || requestedKill;

				var reason = notRespondingAfterInterval ? "not responding" : noHeartbeat ? "not sending a heartbeat signal within hard limit" : "requesting to be killed";

				if (performKill)
				{
					Logger.Error("process {0} is {1}, and will be killed ", processHandler.Name, reason);
					//Debug.WriteLine("Process {0} is not responsive, or no heartbeat within hard limit",processHandler.Name);

					if (processHandler.Kill())
					{

						Logger.Error("Process {0} was {1} and has been successfully killed ", processHandler.Name, reason);
						//Debug.WriteLine("Process {0} has been killed due to non-responsiveness not responding",processHandler.Name);
						processHandler.Close();
						var notEnoughProcesses = (ProcessNo(processHandler.Name) < MinProcesses);
						var lessProcessesThanBefore = (ProcessNo(processHandler.Name) < MaxProcesses) && KeepExistingNoProcesses;


						//if ((ProcessNo(processHandler.Name) == 0) || (ProcessNo(processHandler.Name) > 0) && (KeepExistingNoProcesses && !EnsureSingleProcess))
						if (notEnoughProcesses || lessProcessesThanBefore)
						{
							processHandler.CallExecutable(Logger);
						}
						else
						{
							ProcessHandlers.Remove(processHandler);
						}
					}
					else
					{
						// todo smarter handling of this case (try again in next loop, put to sleep, etc)
						Logger.Error("Process {0} was {1} but could not be successfully killed ", processHandler.Name, reason);
						//Debug.WriteLine("Process {0} could not be killed after getting non responsive",processHandler.Name);
					}
				}
			}
		}

		//If the MaxProcesses is reached this app will close all instances of the monitored application that are not being monitored
		/// <summary>
		/// Checks for duplicate running processes and terminates any duplicates that exceed the maximum allowed.
		/// </summary>
		public void HandleDuplicateProcesses()
		{
			//if (ProcessNo(ApplicationName) < MaxProcesses))
			{

				var processes = Process.GetProcessesByName(ApplicationName);


				if (processes.Length <= MaxProcesses) return;
				//if (processes.Length <= 1) return ;


				Logger.Error("multiple processes of application {0} are running, all but one will be killed ", ApplicationName);

				var remainingProcesses = new List<Process>();
				var result = true;

				var nummProcesses = processes.Length;
				//Wield out the bad applications first
				foreach (var process in processes)
				{
					var processHandler = FindProcessHandler(process);

					// Make sure we leave at least one process running
					if (nummProcesses <= MaxProcesses) break;
					if (!process.Responding)
					{
						Logger.Warn("unresponsive duplicate process {0} will now be killed ", ApplicationName);

						var currResult = (processHandler != null) ? processHandler.Kill() : ProcessUtils.KillProcess(process);
						if (currResult && processHandler != null)
						{
							ProcessHandlers.Remove(processHandler);
							processHandler.Close();
							Logger.Info("Unresponsive duplicate process {0} has been killed ", ApplicationName);
						}
						else
						{
							Logger.Error("Unresponsive duplicate process {0} could not be killed ", ApplicationName);
						}
						result = result && currResult;
						nummProcesses--;
					}
					else
					{
						remainingProcesses.Add(process);
					}

					//Return the other process instance.  
				}

				//Loop through the running processes in with the same name  
				for (var index = MaxProcesses; index < remainingProcesses.Count; index++)
				{
					var process = remainingProcesses[index];
					var processHandler = FindProcessHandler(process);
					Logger.Warn("unresponsive duplicate process {0} will now be killed ", ApplicationName);
					var currResult = ProcessUtils.KillProcess(process);
					if (currResult && processHandler != null)
					{
						ProcessHandlers.Remove(processHandler);
						processHandler.Close();
						Logger.Info("Duplicate process {0} has been killed ", ApplicationName);
					}
					else
					{
						Logger.Error("Unresponsive duplicate process {0} could not be killed ", ApplicationName);
					}
					result = result && currResult;
				}

			}
		}
		
		/// <summary>
		/// Searches the list of process handlers for one corresponding to the specified process.
		/// </summary>
		/// <param name="process">The process to search for.</param>
		/// <returns>The associated ProcessHandler if found; otherwise, null.</returns>
		private ProcessHandler FindProcessHandler(Process process)
		{
			return ProcessHandlers.Find((processHander) => processHander.Process.Id == process.Id);
		}

		/// <summary>
		/// Retrieves the number of currently running processes matching the application executable.
		/// </summary>
		/// <param name="applicationName">The application name (not directly used here).</param>
		/// <returns>The count of running processes.</returns>
		private int ProcessNo(string applicationName)
		{
			return Process.GetProcessesByName(applicationName).Length;
		}

		/// <summary>
		/// Monitors any running processes that are currently not being tracked.
		/// </summary>
		/// <returns>Returns true if the unmonitored processes are handled successfully.</returns>
		public bool HandleUnmonitoredProcesses()
		{
			try
			{
				var processes = Process.GetProcessesByName(ApplicationName);
				foreach (var process in processes)
				{
					if (ProcessHandlers.All(procHandle => procHandle.Process == null || procHandle.Process.Id != process.Id))
					{
						// Begin monitoring the process if it is not already tracked.
						var processHandler = new ProcessHandler
						{
							WaitForExit = false,
							NonResponsiveInterval = NonResponsiveInterval,
						};
						processHandler.MonitorProcess(process, Logger);
						ProcessHandlers.Add(processHandler);
					}

					// Perform 
				}
			}
			catch (Exception ex)
			{
				Logger.Warn("Error while starting to monitor application {0}: {1}", ApplicationName, ex.Message);
				//Debug.WriteLine(ex.Message);
			}

			return true;
		}
				
		/// <summary>
		/// Determines whether this ApplicationHandler instance is equal to another instance by comparing their unique identifiers.
		/// </summary>
		/// <param name="other">Another instance of ApplicationHandler.</param>
		/// <returns>True if both instances share the same identifier; otherwise, false.</returns>
		public bool Equals(ApplicationHandler other)
		{
			return Id.Equals(other.Id);
		}

		/// <summary>
		/// Returns true if one of the process handlers has the selected processId
		/// </summary>
		/// <param name="processId"></param>
		/// <returns></returns>
		internal bool HasProcess(int processId)
		{
			return ProcessHandlers.Any(p => p?.Process?.Id == processId);
		}
	}
}
