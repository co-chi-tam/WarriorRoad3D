using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CMiniGameBingoTask : CSimpleClientTask {

		#region Properties

		protected CBingoRoomData m_CurrentRoomData;
		protected CBingoBoardData m_CurrentBoardData;
		protected bool m_IsStartedBoard;
		protected float m_Delay;
		protected int m_CurrentIndex = -1;

		#endregion

		#region Constructor

		public CMiniGameBingoTask () : base ()
		{
			this.taskName = "MiniGameBingoScene";
			this.nextTask = "LobbyScene";
		}

		#endregion

		#region Implementation Task

		protected override void RegisterEvents() {
			base.RegisterEvents ();
			// RECEIVE BOARD
			this.m_ClientEvents.Add ("clientBingoRoomReceiveBoard", 	this.OnClientBingoRoomReceiveBoard);
			// RECEIVE ROOM COMMAND 
			this.m_ClientEvents.Add ("clientReceiveLeaveBingoRoom",		this.OnClientReceiveLeaveBingoRoom);
		}

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_CurrentRoomData = CTaskUtil.Get (CTaskUtil.BINGO_ROOM) as CBingoRoomData;
			this.m_CurrentBoardData = CTaskUtil.Get (CTaskUtil.BINGO_BOARD) as CBingoBoardData;
			this.m_IsStartedBoard = false;
		}

		public override void UpdateTask (float dt)
		{
			base.UpdateTask (dt);
			if (this.m_IsStartedBoard) {
				if (this.m_Delay > 0f) {
					this.m_Delay -= dt; 
				} else {
					this.m_Delay = this.m_CurrentBoardData.resultPerSecond;
					this.m_CurrentIndex += 1;
					var fakeNumber = this.m_CurrentBoardData.roomFakeResult [this.m_CurrentIndex];
					CUIMiniGameBingoManager.Instance.ActiveANumber (fakeNumber);
				}
			}
		}

		public override void EndTask ()
		{
			base.EndTask ();
			this.m_IsStartedBoard = false;
		}

		#endregion

		#region Bingo

		public virtual void OnClientSendDataBingoRoom(string eventName, JSONObject eventData) {
			if (this.m_UserManager.IsConnected() == false)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["eventName"] = eventName;
			dictData ["eventData"] = eventData.ToString().Replace ("\"", "'");
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("clientSendDataBingoRoom", jsonSend);
		}

		public virtual void OnBingoRoomPlayerReady () {
			var dict = new Dictionary<string, string> ();
			dict ["IsReady"] = "YEAH";
			this.OnClientSendDataBingoRoom ("onBingoRoomPlayerReady", JSONObject.Create (dict));
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientBingoRoomReceiveBoard (SocketIOEvent obj) {
			Debug.LogWarning ("clientBingoRoomReceiveBoard " + obj.ToString());
			var isBingoBoard = obj.data.HasField ("bingoBoard");
			if (isBingoBoard) {
				var boardData = TinyJSON.JSON.Load (obj.data.ToString ()).Make<CBingoBoardData> ();
				this.m_CurrentBoardData = boardData;
				CUIMiniGameBingoManager.Instance.LoadBingoBoard (this.m_CurrentBoardData.bingoBoard);
				CTaskUtil.Set (CTaskUtil.BINGO_BOARD, this.m_CurrentBoardData);
				CUICustomManager.Instance.ActiveLoading (false);
				this.m_IsStartedBoard = true;
				this.m_Delay = boardData.resultPerSecond;
				this.m_CurrentIndex = -1;
			}
		}

		public virtual void OnClientReceiveLeaveBingoRoom (SocketIOEvent obj) {
			// RECEIVE COMMAND LEAVE ROOM
			Debug.LogWarning ("OnClientReceiveLeaveBingoRoom " + obj.ToString ());
			this.OnClientRequestLeaveBingoRoom ();
		}

		public virtual void OnClientRequestLeaveBingoRoom () {
			if (this.m_UserManager.IsConnected() == false)
				return;
			this.m_UserManager.Emit ("clientRequestLeaveBingoRoom", new JSONObject());
			CUICustomManager.Instance.ActiveLoading (true);
		}

		#endregion

	}
}
