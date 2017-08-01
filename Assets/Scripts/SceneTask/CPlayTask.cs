using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CPlayTask : CSimpleTask {

		#region Properties

		#endregion

		#region Constructor

		public CPlayTask () : base ()
		{
			this.taskName = "PlayScene";
			this.nextTask = "LoginScene";
		}

		#endregion

		#region Implementation Task

		#endregion

	}
}
