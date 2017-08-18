using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CSimpleClientTask : CSimpleTask {

		#region Properties

		protected CUserManager m_UserManager;
		protected Dictionary<string, Action<SocketIOEvent>> m_ClientEvents;

		#endregion

		#region Constructor

		public CSimpleClientTask () : base ()
		{
			// NEW REGISTER EVENTS
			this.m_ClientEvents = new Dictionary<string, Action<SocketIOEvent>> ();
			// REGISTER ENVETS
			this.RegisterEvents ();
		}

		#endregion

		#region Implementation Task

		protected virtual void RegisterEvents() {

		}

		protected virtual void RemoveEvents () {
			// CLEAR OLD EVENT
			foreach (var item in this.m_ClientEvents) {
				this.m_UserManager.Off (item.Key, item.Value);
			}
		}

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_UserManager = CUserManager.GetInstance ();
			// INVOKE ALL EVENTS
			foreach (var item in this.m_ClientEvents) {
				this.m_UserManager.On (item.Key, item.Value);
			}
		}

		#endregion

	}
}
