using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CErrorTask : CSimpleTask {

		#region Properties

		#endregion

		#region Constructor

		public CErrorTask () : base ()
		{
			this.taskName = "ErrorScene";
			this.nextTask = "LoginScene";
		}

		#endregion

		#region Implementation Task

		#endregion

	}
}
