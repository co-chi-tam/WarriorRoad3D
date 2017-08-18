using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUIMiniGameFightingManager : CMonoSingleton<CUIMiniGameFightingManager> {

		protected CMiniGameFightingTask m_MiniGameFightingTask;

		protected virtual void Start() {
			this.m_MiniGameFightingTask = CRootTask.Instance.GetCurrentTask () as CMiniGameFightingTask;
		}

		public virtual void OnReadyPressed() {
			this.m_MiniGameFightingTask.OnFightingRoomPlayerReady ();
		}

		public void OnBackPressed () {
			// WARNING
			this.m_MiniGameFightingTask.OnClientRequestLeaveFightingRoom ();
			CUserManager.Instance.OnClientInitAccount ();
			CUICustomManager.Instance.ActiveLoading (true);
		}

	}
}
