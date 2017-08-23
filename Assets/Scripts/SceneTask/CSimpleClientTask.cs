using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CSimpleClientTask : CSimpleTask {

		#region Properties

		protected CUserManager m_UserManager;

		#endregion

		#region Constructor

		public CSimpleClientTask () : base ()
		{
			this.m_UserManager = CUserManager.GetInstance ();
		}

		#endregion

		#region Implementation Task

		protected virtual void RegisterEvent(string name, Action<SocketIOEvent> evnt) {
			this.m_UserManager.OnOnce (name, evnt);
		}

		public override void StartTask ()
		{
			base.StartTask ();
		}

		#endregion

	}
}
