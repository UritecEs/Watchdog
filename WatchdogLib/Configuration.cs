﻿using System;
using System.Collections.Generic;

namespace WatchdogLib
{
	public class Configuration
	{
		public enum RebootModes
		{
			ShutDown,
			Reboot,
			PowerOff,
			HybridShutdown,
		};


		public enum ForceModes
		{
			Normal,
			Force,
			ForceIfHung,
		};

		public enum RebootAfterTimeSlotModes
		{
			FirstOccasion,
			TryNextDay
		};


		public List<ApplicationHandlerConfig> ApplicationHandlers { get; set; }
		public bool ShowTrayIcon { get; set; }
        public bool ShowMainForm { get; set; }
        public bool ShowLogForm { get; set; }
		public bool PeriodicReboot { get; set; }
		public int RebootPeriod { get; set; }
		public DateTime RebootBefore { get; set; }
		public DateTime RebootAfter { get; set; }
		public RebootModes RebootMode { get; set; }
		public RebootModes ForceMode { get; set; }
		public RebootAfterTimeSlotModes RebootAfterTimeSlot { get; set; }

		public Configuration()
		{
			ApplicationHandlers = new List<ApplicationHandlerConfig>();
			ShowTrayIcon = true;
            ShowMainForm = true;

			//var app = new ApplicationHandlerConfig()
			//{
			//    ApplicationPath = @"D:\DevelPers\WatchDog\MonitoredApplication\bin\Release\MonitoredApplication.exe",
			//    ApplicationName = "MonitoredApplication",
			//    Active = true
			//};
			//ApplicationHandlers.Add(app);
		}

	}


	public class ApplicationHandlerConfig
	{
        public Guid Id { get; set; }
		public int NonResponsiveInterval { get; set; }
		public string ApplicationPath { get; set; }
		public string ApplicationName { get; set; }
		public bool UseHeartbeat { get; set; }
		public bool GrantKillRequest { get; set; }
		public uint HeartbeatInterval { get; set; }
		public int MaxProcesses { get; set; }
		public int MinProcesses { get; set; }

		public bool Active { get; set; }
		public bool KeepExistingNoProcesses { get; set; }

		public uint StartupMonitorDelay { get; set; }

		public ApplicationHandlerConfig()
		{
            Id = Guid.NewGuid();    
			NonResponsiveInterval = 20;
			ApplicationPath = "";
			ApplicationName = "";
			UseHeartbeat = false;
			GrantKillRequest = true;
			HeartbeatInterval = 20;
			MinProcesses = 1;
            MaxProcesses            = 1;
            Active                  = true;
			KeepExistingNoProcesses = true;
			StartupMonitorDelay = 20;
		}
	}


}