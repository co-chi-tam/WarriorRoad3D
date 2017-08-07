using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CEventComponent : CComponent {

		protected Dictionary<string, EVCallback> m_Events;

		public override void Init ()
		{
			base.Init ();
			this.m_Events = new Dictionary<string, EVCallback> ();
		}

		public virtual void AddCallback(string name, Action callback) {
			if (this.m_Events.ContainsKey (name))
				return;
			var callbackEvent = new EVCallback ();
			callbackEvent.callback = callback;
			this.m_Events.Add (name, callbackEvent);
		}

		public virtual void RemoveCallback(string name) {
			if (this.m_Events.ContainsKey (name) == false)
				return;
			this.m_Events.Remove (name);
		}

		public virtual void TriggerCallback(string name) {
			if (this.m_Events.ContainsKey (name) == false)
				return;
			if (this.m_Events [name].callback != null) {
				this.m_Events [name].callback ();
			}
		}
		
	}

	public class EVCallback {
		public Action callback;
	}
}
