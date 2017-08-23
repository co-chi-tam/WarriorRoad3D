using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CMiniGameFightingTask : CSimpleClientTask {

		#region Properties

		protected CMiniGameFightingManager m_MiniGameFightingManager;

		#endregion

		#region Constructor

		public CMiniGameFightingTask () : base ()
		{
			this.taskName = "MiniGameFightingScene";
			this.nextTask = "LobbyScene";
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_MiniGameFightingManager = CMiniGameFightingManager.GetInstance ();
			this.m_MiniGameFightingManager.OnLoadMiniGameCompleted -= this.OnLoadControllerCompleted;
			this.m_MiniGameFightingManager.OnLoadMiniGameCompleted += this.OnLoadControllerCompleted;
			this.m_MiniGameFightingManager.StartLoading ();
		}

		#endregion

		#region Fighting

		public virtual void OnLoadControllerCompleted() {
			this.m_MiniGameFightingManager.SetupTargets ();
		}

		public virtual void OnClientEndFighting(string winnerId, string closerId) {
			if (this.m_UserManager.IsConnected() == false)
				return;
			var miniFightingData = CTaskUtil.Get (CTaskUtil.MINI_FIGHTING_DATA) as CMiniFightingData;
			var dictData = new Dictionary<string, string> ();
			dictData ["isoTime"] = miniFightingData.isoTime;
			dictData ["winnerId"] = winnerId;
			dictData ["closerId"] = closerId;
			dictData ["randomSeed"] = miniFightingData.randomSeed.ToString ();
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("clientEndMiniFightingGame", jsonSend);
		}

		#endregion

	}
}
