using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
 
namespace ThreadService
{
	public class Service1 : System.ServiceProcess.ServiceBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// This is a flag to indicate the service status
		private bool serviceStarted = false;

		// the thread that will do the work
		Thread workerThread;

		#region Service plumbing
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
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service1() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.ServiceName = "ThreadService";
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
			// create worker thread; this will invoke the WorkerFunction
			// when we start it. 
			// Since we use a separate worker thread, the main service 
			// thread will return quickly telling windows that service has started
			ThreadStart st = new ThreadStart(WorkerFunction);
			workerThread = new Thread(st);

			// set flag to indicate worker thread is active
			serviceStarted = true;

			// start the thread
			workerThread.Start(); 
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			// flag to tell the worker process to stop 
			serviceStarted = false;

			// give it a little time to finish any pending work
			workerThread.Join(new TimeSpan(0,2,0));
		}

		/// <summary>
		/// This function will do all the work 
		/// Once it is done with its tasks, it will be suspended for some time; 
		/// it will continue to repeat this until the service is stopped
		/// </summary>
 		private void WorkerFunction() 
		{
			// start an endless loop until; loop will abort only when "serviceStarted" flag = false
			while (serviceStarted) 
			{
				// Do some work
				// Note: exception handling is very important here 
				// if you dont, the error will vanish along with your worker thread
				try 
				{
					EventLog.WriteEntry ("Single Thread Service Tick :" + DateTime.Now.ToString());    
				}
				catch (System.Exception ex)
				{
					// add some robust logging here
					EventLog.WriteEntry("Error! " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
				}

				// yield
				if (serviceStarted) 
				{
					Thread.Sleep(new TimeSpan(0,1,0));   				 
				}
			}

			// time to end the thread
			Thread.CurrentThread.Abort();  
		}

	}
}
