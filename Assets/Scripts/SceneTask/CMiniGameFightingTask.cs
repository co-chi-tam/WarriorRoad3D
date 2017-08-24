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
			// GET REWARD
			this.RegisterEvent  ("clientGetRewardBattle", 		this.OnClientGetRewardBattle);
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
			dictData ["isoTime"] 	= miniFightingData.isoTime;
			dictData ["winnerId"] 	= winnerId;
			dictData ["closerId"] 	= closerId;
			dictData ["randomSeed"] = miniFightingData.randomSeed.ToString ();
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("clientEndMiniFightingGame", jsonSend);
		}

		#endregion

		#region REWARD

		public virtual void OnClientGetRewardBattle(SocketIOEvent obj) {
			Debug.LogWarning (obj.ToString ());
			var glory = obj.data.GetField ("glory").ToString ();
			var gold = obj.data.GetField ("gold").ToString ();
			CUIMiniGameFightingManager.Instance.SetUpWinningBox ("WINNING", 
				string.Format ("YOU WIN x{0} GLORY and x{1} GOLD", glory, gold));
		}

		public virtual void OnClientClosedBattle() {
			CUIMiniGameFightingManager.Instance.SetUpClosingBox ("CLOSE", "YOU CLOSE TRY AGAIN LATE !!");
		}

		#endregion

	}
}
