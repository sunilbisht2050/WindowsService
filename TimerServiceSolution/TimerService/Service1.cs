using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
 
namespace TimerService
{
	public class Service1 : System.ServiceProcess.ServiceBase
	{
		private System.ComponentModel.IContainer components;

		// declare class level variable for the timer 
		private Timer serviceTimer;

		private int interval = 60000;

		/// <summary>
		/// Flag date to indicate that work is going on
		/// Will be set to DateTime.Now when work is started
		/// and reset to DateTime.MinValue when work is done
		/// </summary>
		private DateTime _workStartTime = DateTime.MinValue; 

		#region plumbing
		public Service1()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
		}

		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
            //
            //ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service1() };
            //System.ServiceProcess.ServiceBase.Run(ServicesToRun);

            Service1 service = new Service1();
            service.RunAsConsole(new String[] { });
        }

        public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            OnStop();
        }
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
			// 
			// Service1
			// 
			this.ServiceName = "TimerService";

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			TimerCallback timerDelegate = new TimerCallback(DoWork);

			// create timer and attach our method delegate to it
			serviceTimer = new Timer(timerDelegate, null, 1000, interval);
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			// stop the timer
			serviceTimer.Dispose();
		}

		/// <summary>
		/// Do some work in this procedure
		/// </summary>
		private void DoWork(object state)
		{
			if (_workStartTime != DateTime.MinValue)
			{
				// probably check how much time has elapsed since work started
				// and log any warning
				ServiceEventLog.WriteEntry("Warning! Worker busy since " + _workStartTime.ToLongTimeString()
						, System.Diagnostics.EventLogEntryType.Warning);
			}
			else
			{
				// set work start time
				_workStartTime = DateTime.Now;

				// Do some work
				// Note: exception handling is very important here 
				// if you dont, the error will vanish along with your worker thread
				try 
				{
					ServiceEventLog.WriteEntry ("Timer Service Tick :" + DateTime.Now.ToString());    
				}
				catch (System.Exception ex)
				{
					// add some robust logging here
					ServiceEventLog.WriteEntry("Error! " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
				}

				// reset work start time
				_workStartTime = DateTime.MinValue;
			}
		}

		System.Diagnostics.EventLog ServiceEventLog
		{
			get 
			{
				return this.EventLog;
			}
		}
	}
}
