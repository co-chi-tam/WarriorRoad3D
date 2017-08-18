using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CMiniGameBingoTask : CSimpleClientTask {

		#region Properties

		protected CBingoRoomData m_CurrentBingoRoomData;

		#endregion

		#region Constructor

		public CMiniGameBingoTask () : base ()
		{
			this.taskName = "MiniGameBingoScene";
			this.nextTask = "LobbyScene";
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_CurrentBingoRoomData = CTaskUtil.Get (CTaskUtil.BINGO_ROOM) as CBingoRoomData;
		}

		protected override void RegisterEvents() {
			base.RegisterEvents ();
			this.m_ClientEvents.Add ("onClientBingoRoomReceiveBoard", 	this.OnClientBingoRoomReceiveBoard);
		}

		#endregion

		#region Bingo

		public virtual void OnClientSendDataBingoRoom(string eventName, JSONObject eventData) {
			if (this.m_UserManager.IsConnected() == null)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["eventName"] = eventName;
			dictData ["eventData"] = eventData.ToString().Replace ("\"", "'");
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("onClientSendDataBingoRoom", jsonSend);
		}

		public virtual void OnBingoRoomPlayerReady () {
			var dict = new Dictionary<string, string> ();
			dict ["IsReady"] = "YEAH";
			this.OnClientSendDataBingoRoom ("onBingoRoomPlayerReady", JSONObject.Create (dict));
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientBingoRoomReceiveBoard (SocketIOEvent obj) {
			Debug.LogWarning ("onClientBingoRoomReceiveBoard " + obj.ToString());
			var isBingoBoard = obj.data.HasField ("bingoBoard");
			if (isBingoBoard) {
				var boardList = obj.data.GetField ("bingoBoard").list;
				var boardStr = new string [boardList.Count];
				for (int i = 0; i < boardList.Count; i++) {
					boardStr [i] = boardList [i].ToString();
				}
				CUIMiniGameBingoManager.Instance.LoadBingoBoard (boardStr);
				CUICustomManager.Instance.ActiveLoading (false);
			}
		}

		public virtual void OnClientRequestLeaveBingoRoom () {
			if (this.m_UserManager.IsConnected() == null)
				return;
			this.m_UserManager.Emit ("onClientRequestLeaveBingoRoom", new JSONObject());
			CUICustomManager.Instance.ActiveLoading (true);
		}

		#endregion

	}
}
