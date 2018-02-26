using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
 
namespace MultiThreadedService
{
	public class Service1 : System.ServiceProcess.ServiceBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// array of worker threads
		Thread[] workerThreads;

		//the objects that do the actual work
		Worker[] arrWorkers;

		// number of threads; typically specified in config file
		// along with config values for each worker
		int numberOfThreads = 2;

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
			this.ServiceName = "MultiThreadedService";
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

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			arrWorkers = new Worker[numberOfThreads]; 
			workerThreads = new Thread[numberOfThreads];
			for (int i =0; i < numberOfThreads; i++) 
			{
				// create an object
				arrWorkers[i] = new Worker(i+1, EventLog);

				// set properties on the object
				arrWorkers[i].ServiceStarted = true;

				// create a thread and attach to the object
				ThreadStart st = new ThreadStart(arrWorkers[i].ExecuteTask);
				workerThreads[i] = new Thread(st);
			}

			// start the threads
			for (int i = 0; i < numberOfThreads; i++) 
			{
				workerThreads[i].Start(); 
			}		
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			for (int i =0; i < numberOfThreads; i++) 
			{
				// set flag to stop worker thread.
				arrWorkers[i].ServiceStarted = false;

				// give it a little time to finish any pending work
				workerThreads[i].Join(new TimeSpan(0,2,0));
			}
		}
	}
}
