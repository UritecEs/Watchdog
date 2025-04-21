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
		private readonly Stopwatch _sleepStopwatch;
		private readonly Logger _logger;
		private readonly AsyncWorker asyncWorkerMonitor;


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
			_sleepStopwatch = new Stopwatch();
			asyncWorkerMonitor = new AsyncWorker(MonitorJob) { Name = "ApplicationWatcher" };
			//asyncWorkerMonitor.Start();
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
				// Walk through list of applications to see which ones are running
				_sleepStopwatch.Restart();
				foreach (var applicationHandler in ApplicationHandlers)
				{
					applicationHandler?.Check();
				}
			}
			catch (Exception e)
			{
				_logger.Error(e, "Exceptionn in MonitorJob " + e.Message);

			}
			Thread.Sleep(Math.Max(0, 500 - (int)_sleepStopwatch.ElapsedMilliseconds));
			return true;
		}


		/*
		public bool AddMonitoredApplication(string applicationName, string applicationPath, int nonResponsiveInterval, uint heartbeatInterval = 15, int minProcesses = 1, int maxProcesses = 1, bool keepExistingNoProcesses = false, bool useHeartbeat = false, bool grantKillRequest = true, bool active = false, uint startupMonitorDelay = 20)
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
