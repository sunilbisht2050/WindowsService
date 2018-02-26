using System;
using System.Diagnostics;
using System.Threading;
//using System.Configuration;

namespace MultiThreadedService
{
	/// <summary>
	/// Worker object for the multi-threaded service
	/// </summary>
	public class Worker
	{
		// flag to tell if the service is active
		private bool _serviceStarted = false;

		// thread id
		private int _id = 0;

		// the event log of the service
		private EventLog _serviceEventLog;

		// interval at which the task is executed
		private int _interval = 60;		

		public Worker(int id, EventLog serviceEventLog) 
		{
			this._id = id;
			this._serviceEventLog = serviceEventLog;
		}

		#region ExecuteTask method
		/// <summary>
		/// Execution loop
		/// </summary>
		public void ExecuteTask() 
		{
			DateTime lastRunTime = DateTime.UtcNow;

			while (_serviceStarted) 
			{
				// check the current time against the last run plus interval
				if ( ((TimeSpan) (DateTime.UtcNow.Subtract(lastRunTime))).TotalSeconds >= _interval) 
				{

					// If time to do something, do so
					// Note: exception handling is very important here 
					// if you dont, the error will vanish along with your worker thread
					try 
					{
						_serviceEventLog.WriteEntry ("Multithreaded Service; Thread" + _id.ToString() + 
									" Tick :" + DateTime.Now.ToString());    
					}
					catch (System.Exception ex)
					{
						// add some robust logging here
						_serviceEventLog.WriteEntry("Error! " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
					}

					// set new run time
					lastRunTime = DateTime.UtcNow;
				}
				
				// yield
				if (_serviceStarted) 
				{
					Thread.Sleep(new TimeSpan(0,0,15));   				 
				}
			}
			
			Thread.CurrentThread.Abort();  
		}
		#endregion

		/// <summary>
		/// Flag to start/stop the processing on the thread
		/// </summary>
		public bool ServiceStarted
		{
			get
			{
				return _serviceStarted;
			}
			set
			{
				_serviceStarted = value;
			}
		}
	
	}
}
