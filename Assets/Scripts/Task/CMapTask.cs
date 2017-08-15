using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarriorRoad {
	public class CMapTask {

		#region Properties

		private Dictionary<string, CTask> m_Map;

		#endregion

		#region Constructor

		public CMapTask ()
		{
			this.m_Map = new Dictionary<string, CTask> ();
			this.LoadMap ();
		}

		#endregion

		#region Main methods

		public virtual void LoadMap() {
			this.m_Map ["IntroScene"]			= new CIntroTask ();
			this.m_Map ["LoginScene"]			= new CLoginTask ();
			this.m_Map ["LobbyScene"]			= new CLobbyTask ();
			this.m_Map ["CreateHeroScene"]		= new CCreateHeroTask ();
			this.m_Map ["PlayScene"] 			= new CPlayTask ();
		}

		#endregion

		#region Getter && Setter

		public virtual string GetTaskName<T>() {
			foreach (var item in this.m_Map) {
				if (typeof(T).Equals (item.GetType ())) {
					return item.Key;
				}
			}
			return "NULL";
		}

		public virtual CTask GetFirstTask() {
			var keys = this.m_Map.Keys.ToList();
			var firstTask = this.m_Map[keys[0]];
			firstTask.nextTask = this.m_Map[keys[1]].GetTaskName();
			return firstTask;
		}

		public virtual CTask GetTask(string name) {
			if (this.m_Map.ContainsKey (name)) {
				return this.m_Map [name];
			}
			return null;
		}

		#endregion
		
	}
}
