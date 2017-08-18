using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CLobbyTask : CSimpleClientTask {

		#region Properties

		protected CUILobbyManager m_LobbyManager;
		protected CBingoRoomData m_CurrentBingoRoomData;

		#endregion

		#region Constructor

		public CLobbyTask () : base ()
		{
			this.taskName = "LobbyScene";
			this.nextTask = "PlayScene";
		}

		#endregion

		#region Implementation Task

		protected override void RegisterEvents() {
			base.RegisterEvents ();
			// ON CLIENT RECEIVE SKILLS
			this.m_ClientEvents.Add ("clientReceiveSkills", 		this.OnClientReceiveSkills);
			// ON CLIENT SET UP SKILL COMPLETED
			this.m_ClientEvents.Add ("clientCompletedSetupSkill", 	this.OnClientCompleteSetupSkills);
			// CHAT
			this.m_ClientEvents.Add ("clientReceiveChat", 			this.OnClientReceiveChat);
			// BINGO
			this.m_ClientEvents.Add ("clientReceiveBingoRoomList",  this.OnClientReceiveBingoRoomList);
			this.m_ClientEvents.Add ("clientInitBingoRoom", 		this.OnClientInitBingoRoom);
		}

		public override void StartTask ()
		{
			base.StartTask ();
			var skillList = CTaskUtil.Get (CTaskUtil.SKILL_DATA_LIST) as List<CSkillData>;
			this.m_LobbyManager = CUILobbyManager.GetInstance ();
			this.m_LobbyManager.OnSetupHeroSkill ("Normal Attack", 3, skillList, null);
			this.m_LobbyManager.OnHeroSetupSubmit -= OnHeroAlreadySetupSkill;
			this.m_LobbyManager.OnHeroSetupSubmit += OnHeroAlreadySetupSkill;
		}

		public override void EndTask ()
		{
			base.EndTask ();
			var roomResponseCode = this.m_CurrentBingoRoomData.eventResponseCode;
			this.m_UserManager.Off (roomResponseCode, OnClientReceiveBingoResponseCode);
		}

		#endregion

		#region Skill

		public virtual void OnClientInitSkill() {
			if (this.m_UserManager == null)
				return;
			this.m_UserManager.Emit ("clientInitSkill", new JSONObject());
		}

		public virtual void OnClientReceiveSkills(SocketIOEvent obj) {
			Debug.LogWarning ("OnClientReceiveSkills " + obj.ToString ());
		}

		protected virtual void OnHeroAlreadySetupSkill(List<CSkillData> skillDatas) {
			if (this.m_UserManager.IsConnected() == null)
				return;
			var heroData = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CHeroData;
			heroData.characterSkillSlots = new CSkillData[skillDatas.Count];
			for (int i = 0; i < skillDatas.Count; i++) {
				heroData.characterSkillSlots [i] = skillDatas [i];
			}
			var dictData = new Dictionary<string, string> ();
			var setupSkills = "";
			for (int i = 0; i < skillDatas.Count; i++) {
				var skillData = skillDatas [i];
				setupSkills += skillData.objectName + (i < skillDatas.Count - 1 ? "," : "");
			}
			dictData ["skills"] = setupSkills;
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("clientSetupSkills", jsonSend);
		}

		public virtual void OnClientCompleteSetupSkills (SocketIOEvent obj) {
			this.OnTaskCompleted ();
		}

		#endregion

		#region Chat

		public virtual void OnClientSendChat(string chat) {
			if (this.m_UserManager.IsConnected() == null)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["chatString"] = chat.ToString();
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("clientSendChat", jsonSend);
		}

		public virtual void OnClientReceiveChat(SocketIOEvent obj) {
			Debug.LogWarning ("clientReceiveChat " + obj.ToString());
			var isHasChat = obj.data.HasField ("chatStr");
			if (isHasChat) {
				// WARNING
				if (CSceneManager.Instance.GetActiveSceneName () != this.taskName)
					return;
				var chatOwner = obj.data.GetField ("chatOwner").ToString().Replace ("\"", string.Empty);
				var chatStr = obj.data.GetField ("chatStr").ToString().Replace ("\"", string.Empty);
				var chat = chatOwner + ": " + chatStr; 
				var chatData = new CChatData () { 
					chatOwner = chatOwner, 
					chatStr = chat, 
					isMine = chatOwner == this.m_UserManager.currentHero.objectName
				};
				CUILobbyManager.Instance.ReceiveChatText (chatData);
			}
		}

		#endregion 

		#region Bingo

		public virtual void OnClientGetBingoRoomList() {
			if (this.m_UserManager.IsConnected() == null)
				return;
			this.m_UserManager.Emit ("onClientGetBingoRoomList", new JSONObject());
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientReceiveBingoRoomList(SocketIOEvent obj) {
			if (CSceneManager.Instance.GetActiveSceneName () != this.taskName)
				return;
			Debug.LogWarning ("clientReceiveBingoRoomList " + obj.ToString());
			var roomList = obj.data.GetField ("roomListData").list;
			var roomListData = new List<CBingoRoomData> ();
			for (int i = 0; i < roomList.Count; i++) {
				var objectStr = roomList [i].ToString ();
				if (objectStr.Equals ("null") == false) {
					var objectData = TinyJSON.JSON.Load (objectStr).Make<CBingoRoomData> ();
					roomListData.Add (objectData);
				} else {
					roomListData.Add (null);
				}
			}
			CUILobbyManager.Instance.SetUpBingoRoom (roomListData, (index, room) => {
				this.OnClientRequestJoinBingoRoom (index);
			});
			CUICustomManager.Instance.ActiveLoading (false);
		}

		public virtual void OnClientRequestJoinBingoRoom(int roomIndex) {
			if (this.m_UserManager.IsConnected() == null)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["roomIndex"] = roomIndex.ToString ();
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("onClientRequestJoinBingoRoom", jsonSend);
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientRequestLeaveBingoRoom () {
			if (this.m_UserManager.IsConnected() == null)
				return;
			this.m_UserManager.Emit ("onClientRequestLeaveBingoRoom", new JSONObject());
			if (this.m_CurrentBingoRoomData != null) {
				var roomResponseCode = this.m_CurrentBingoRoomData.eventResponseCode;
				this.m_UserManager.Off (roomResponseCode, OnClientReceiveBingoResponseCode);
			}
			this.m_CurrentBingoRoomData = null;
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientInitBingoRoom(SocketIOEvent obj) {
			Debug.LogWarning ("clientInitBingoRoom " + obj.ToString());
			var isRoomData = obj.data.HasField ("roomData");
			if (isRoomData) {
				var objStr = obj.data.GetField ("roomData").ToString ();
				this.m_CurrentBingoRoomData = TinyJSON.JSON.Load (objStr).Make<CBingoRoomData> ();
				var roomResponseCode = this.m_CurrentBingoRoomData.eventResponseCode;
				CTaskUtil.Set (CTaskUtil.BINGO_ROOM, this.m_CurrentBingoRoomData);
				// REGISTER EVENT
				this.m_UserManager.Off (roomResponseCode, OnClientReceiveBingoResponseCode);
				this.m_UserManager.On (roomResponseCode, OnClientReceiveBingoResponseCode);
				// COMPLETE TASK
				CRootTask.Instance.ProcessNextTask ("MiniGameBingoScene");
				CRootTask.Instance.GetCurrentTask().OnTaskCompleted();

				CTaskUtil.Set (CTaskUtil.BINGO_ROOM_RESPONSE_CODE, roomResponseCode);
				CUICustomManager.Instance.ActiveLoading (false);
			}
		}

		protected virtual void OnClientReceiveBingoResponseCode(SocketIOEvent obj) {
			Debug.LogWarning (obj.ToString ());
		}

		#endregion


	}
}
