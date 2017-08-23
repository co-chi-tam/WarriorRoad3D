using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CSimpleTask : CTask {

		#region Constructor

		public CSimpleTask () : base ()
		{
			this.taskName = string.Empty;
			this.nextTask = string.Empty;
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Application.targetFrameRate = 60;
			this.m_IsCompleteTask = false;
		}

		#endregion
		
	}
}
