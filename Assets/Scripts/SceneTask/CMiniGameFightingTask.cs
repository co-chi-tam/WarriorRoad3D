using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CMiniGameFightingTask : CSimpleClientTask {

		#region Properties

		protected CRoomData m_CurrentRoomData;
		protected bool m_IsStartedBoard;
		protected float m_Delay;
		protected int m_CurrentIndex = -1;

		#endregion

		#region Constructor

		public CMiniGameFightingTask () : base ()
		{
			this.taskName = "MiniGameFightingScene";
			this.nextTask = "LobbyScene";
			// RECEIVE ROOM COMMAND 
			this.RegisterEvent ("clientReceiveLeaveFightingRoom",		this.OnClientReceiveLeaveFightingRoom);
			// RECEIVE ROOM READY MESSAGE 
			this.RegisterEvent ("clientFightingRoomReceiveMessage",	this.OnClientFightingRoomReceiveMessage);
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_CurrentRoomData = CTaskUtil.Get (CTaskUtil.FIGHTING_ROOM) as CRoomData;
			this.m_IsStartedBoard = false;
		}

		public override void UpdateTask (float dt)
		{
			base.UpdateTask (dt);
		}

		public override void EndTask ()
		{
			base.EndTask ();
			this.m_IsStartedBoard = false;
		}

		#endregion

		#region Fighting

		public virtual void OnClientSendDataFightingRoom(string eventName, JSONObject eventData) {
			if (this.m_UserManager.IsConnected() == false)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["eventName"] = eventName;
			dictData ["eventData"] = eventData.ToString().Replace ("\"", "'");
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("clientSendDataFightingRoom", jsonSend);
		}

		public virtual void OnFightingRoomPlayerReady () {
			var dict = new Dictionary<string, string> ();
			dict ["IsReady"] = "YEAH";
			this.OnClientSendDataFightingRoom ("onFightingRoomPlayerReady", JSONObject.Create (dict));
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientReceiveLeaveFightingRoom (SocketIOEvent obj) {
			// RECEIVE COMMAND LEAVE ROOM
			Debug.LogWarning ("OnClientReceiveLeaveFightingRoom " + obj.ToString ());
			this.OnClientRequestLeaveFightingRoom ();
		}

		public virtual void OnClientRequestLeaveFightingRoom () {
			if (this.m_UserManager.IsConnected() == false)
				return;
			this.m_UserManager.Emit ("clientRequestLeaveFightingRoom", new JSONObject());
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientFightingRoomReceiveMessage (SocketIOEvent obj) {
			Debug.LogWarning ("OnClientFightingRoomReceiveMessage " + obj.ToString ());
			CUICustomManager.Instance.ActiveLoading (false);
		}

		#endregion

	}
}
