// <copyright file="ApplicationWatcher.cs" company="DevThread">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Thijs Elenbaas</author>
// <summary>The ApplicationWatcher class implementation. This is the actual watchdog.</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NLog;
using Utilities;

namespace WatchdogLib
{
	/// <summary>
	/// Monitors and manages a set of application handlers as part of a watchdog system.
	/// This class starts an asynchronous worker that periodically checks the health and status
	/// of each monitored application.
	/// </summary>
	public class ApplicationWatcher
	{
		//private readonly Stopwatch _sleepStopwatch;
		private readonly Logger _logger;
		private readonly AsyncWorker asyncWorkerMonitor;

		/// <summary>
		/// Fires when a new heartbeat is received. Params: sender = applicationhandler
		/// </summary>
		public EventHandler HeartbeatEvent;

		/// <summary>
		/// Gets or sets the list of application handlers which are being monitored.
		/// </summary>
		public List<ApplicationHandler> ApplicationHandlers { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationWatcher"/> class using the provided logger.
		/// An asynchronous monitoring job is started upon instantiation.
		/// </summary>
		public ApplicationWatcher(Logger logger)
		{
			_logger = logger;
			ApplicationHandlers = new List<ApplicationHandler>();
			//_sleepStopwatch = new Stopwatch();
			// Create and start an asynchronous worker thread for monitoring applications.
			asyncWorkerMonitor = new AsyncWorker(MonitorJob) { Name = "ApplicationWatcher" };
			//asyncWorkerMonitor.Start();

			HeartbeatServer.Instance.HeartbeatEvent += HeartbeatServer_OnHeartbeat;
		}

		public void Pausar()
		{
			if (asyncWorkerMonitor.IsRunning)
				asyncWorkerMonitor.Suspend();
		}

		public void Continuar()
		{
			if (asyncWorkerMonitor.IsSuspended)
				asyncWorkerMonitor.Resume();
			else
				asyncWorkerMonitor.Start();
		}

		private void HeartbeatServer_OnHeartbeat(object sender, EventArgs eventArgs)
		{
			var p = (HeartbeatClient)sender;
			var app = FindApplication(p.ProcessId);
			if (app is null)
				return;
			HeartbeatEvent?.Invoke(app, null);
		}

		/// <summary>
		/// Finds the application that has the process Id
		/// </summary>
		/// <param name="ProcessId"></param>
		/// <returns></returns>
		private ApplicationHandler FindApplication(int ProcessId)
		{
			return ApplicationHandlers.FirstOrDefault(app => app.HasProcess(ProcessId));
		}

		/// <summary>
		/// The monitoring job that is periodically executed.
		/// This method iterates through the list of application handlers and invokes their Check() method.
		/// </summary>
		/// <returns>Always returns true after performing the monitoring cycle.</returns>
		private bool MonitorJob()
		{
			try
			{
				_logger.Trace("Monitoring {0} applications.", ApplicationHandlers?.Count ?? 0);
				// Walk through list of applications to see which ones are running.
				//_sleepStopwatch.Restart();
				foreach (var applicationHandler in ApplicationHandlers)
				{
					applicationHandler?.Check();
				}
			}
			catch (Exception e)
			{
				_logger.Error(e, "Exceptionn in MonitorJob " + e.Message);

			}
			// Optionally, you can sleep for a specific interval:
			// Thread.Sleep(Math.Max(0, 500 - (int)_sleepStopwatch.ElapsedMilliseconds));
			Thread.Sleep(1000);
			return true;
		}


		/*
		/// Registers a new application for monitoring.
		/// </summary>
		/// <param name="applicationName">The name of the application.</param>
		/// <param name="applicationPath">The file system path to the application executable.</param>
		/// <param name="nonResponsiveInterval">The interval after which a process is considered non-responsive.</param>
		/// <param name="heartbeatInterval">The heartbeat check interval (default is 15 seconds).</param>
		/// <param name="minProcesses">The minimum number of processes required (default is 1).</param>
		/// <param name="maxProcesses">The maximum allowed number of processes (default is 1).</param>
		/// <param name="keepExistingNoProcesses">Whether to maintain the existing process count even if none are running (default is false).</param>
		/// <param name="useHeartbeat">Whether heartbeat monitoring is enabled (default is false).</param>
		/// <param name="grantKillRequest">Whether process kill requests are permitted (default is true).</param>
		/// <param name="active">Whether the application should be immediately active (default is false).</param>
		/// <param name="startupMonitorDelay">Delay after startup before starting process monitoring (default is 20 seconds).</param>
		/// <returns>Returns true if the application was successfully registered for monitoring.</returns>
		public bool AddMonitoredApplication(string applicationName, string applicationPath, int nonResponsiveInterval,
			uint heartbeatInterval = 15, int minProcesses = 1, int maxProcesses = 1, bool keepExistingNoProcesses = false,
			bool useHeartbeat = false, bool grantKillRequest = true, bool active = false, uint startupMonitorDelay = 20)
		{
			_logger.Trace("Registering {0} for monitoring", applicationName);
			ApplicationHandlers.Add(new ApplicationHandler(applicationName, applicationPath, nonResponsiveInterval, heartbeatInterval, minProcesses, maxProcesses, keepExistingNoProcesses, useHeartbeat, grantKillRequest, startupMonitorDelay) { Logger = _logger });
			return true; // todo
		}
		*/

		/// <summary>
		/// Deserializes the configuration object and adds the corresponding application handlers
		/// to the monitoring list.
		/// </summary>
		/// <param name="configuration">The configuration object containing ApplicationHandlerConfig entries.</param>
		public void Deserialize(Configuration configuration)
		{
			foreach (var applicationHandlerConfig in configuration.ApplicationHandlers)
			{
				var applicationHandler = new ApplicationHandler(applicationHandlerConfig);
				ApplicationHandlers.Add(applicationHandler);
			}
		}

		/// <summary>
		/// Adds a new ApplicationHandler based on the provided `Configuration`.
		/// </summary>
		/// <param name="configuration">The configuration object containing the updated list of `ApplicationHandlerConfig` objects.</param>
		public void Add(ApplicationHandlerConfig applicationHandlerConfig)
		{
			ApplicationHandlers.Add(new ApplicationHandler(applicationHandlerConfig));
		}

		/// <summary>
		/// Updates one `ApplicationHandler` based on the provided `Configuration`.
		/// </summary>
		/// <param name="configuration">The configuration object containing the updated list of `ApplicationHandlerConfig` objects.</param>
		public void Update(ApplicationHandlerConfig applicationHandlerConfig)
		{
			ApplicationHandlers
				.Find(app => app.Id == applicationHandlerConfig.Id)
				.UpdateConfiguration(applicationHandlerConfig);
		}

		/// <summary>
		/// Removes an existing application handler
		/// </summary>
		/// <param name="applicationHandlerConfig"></param>
		public void Remove(ApplicationHandlerConfig applicationHandlerConfig)
		{
			var appId = applicationHandlerConfig.Id;
			var appHandler = ApplicationHandlers.FirstOrDefault(app => app.Id == appId);
			if (appHandler != null && appHandler.Id == appId)
			{
				ApplicationHandlers.Remove(appHandler);
			}
		}
	}
}
