using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CLobbyTask : CSimpleClientTask {

		#region Properties

		protected CUILobbyManager m_LobbyManager;
		protected CRoomData m_CurrentFightingRoomData;

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
			// Fighting
			this.m_ClientEvents.Add ("clientReceiveFightingRoomList",  this.OnClientReceiveFightingRoomList);
			this.m_ClientEvents.Add ("clientInitFightingRoom", 		this.OnClientInitFightingRoom);
		}

		public override void StartTask ()
		{
			base.StartTask ();
			var skillList = CTaskUtil.Get (CTaskUtil.SKILL_DATA_LIST) as List<CSkillData>;
			this.m_LobbyManager = CUILobbyManager.GetInstance ();
			this.m_LobbyManager.OnSetupHeroSkill ("Normal Attack", 3, skillList, null);
		}

		public override void EndTask ()
		{
			base.EndTask ();
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

		public virtual void OnHeroAlreadySetupSkill(List<CSkillData> skillDatas) {
			if (this.m_UserManager.IsConnected() == false)
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
			Debug.LogWarning (obj.ToString());
			// COMPLETE TASK
			this.m_NextTask = "PlayScene";
			this.OnTaskCompleted ();
		}

		#endregion

		#region Chat

		public virtual void OnClientSendChat(string chat) {
			if (this.m_UserManager.IsConnected() == false)
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

		#region Fighting

		public virtual void OnClientGetFightingRoomList() {
			if (this.m_UserManager.IsConnected() == false)
				return;
			this.m_UserManager.Emit ("clientGetFightingRoomList", new JSONObject());
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientReceiveFightingRoomList(SocketIOEvent obj) {
			if (CSceneManager.Instance.GetActiveSceneName () != this.taskName)
				return;
			Debug.LogWarning ("clientReceiveFightingRoomList " + obj.ToString());
			var roomList = obj.data.GetField ("roomListData").list;
			var roomListData = new List<CRoomData> ();
			for (int i = 0; i < roomList.Count; i++) {
				var objectStr = roomList [i].ToString ();
				if (objectStr.Equals ("null") == false) {
					var objectData = TinyJSON.JSON.Load (objectStr).Make<CRoomData> ();
					roomListData.Add (objectData);
				} else {
					roomListData.Add (null);
				}
			}
			CUILobbyManager.Instance.SetUpFightingRoom (roomListData, (index, room) => {
				this.OnClientRequestJoinFightingRoom (index);
			});
			CUICustomManager.Instance.ActiveLoading (false);
		}

		public virtual void OnClientRequestJoinFightingRoom(int roomIndex) {
			if (this.m_UserManager.IsConnected() == false)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["roomIndex"] = roomIndex.ToString ();
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("clientRequestJoinFightingRoom", jsonSend);
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientRequestLeaveFightingRoom () {
			if (this.m_UserManager.IsConnected() == false)
				return;
			this.m_UserManager.Emit ("clientRequestLeaveFightingRoom", new JSONObject());
			if (this.m_CurrentFightingRoomData != null) {
				var roomResponseCode = this.m_CurrentFightingRoomData.eventResponseCode;
				this.m_UserManager.Off (roomResponseCode, OnClientReceiveFightingResponseCode);
			}
			this.m_CurrentFightingRoomData = null;
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientInitFightingRoom(SocketIOEvent obj) {
			Debug.LogWarning ("clientInitFightingRoom " + obj.ToString());
			var isRoomData = obj.data.HasField ("roomData");
			if (isRoomData) {
				var objStr = obj.data.GetField ("roomData").ToString ();
				this.m_CurrentFightingRoomData = TinyJSON.JSON.Load (objStr).Make<CRoomData> ();
				var roomResponseCode = this.m_CurrentFightingRoomData.eventResponseCode;
				CTaskUtil.Set (CTaskUtil.FIGHTING_ROOM, this.m_CurrentFightingRoomData);
				// REGISTER EVENT
				this.m_UserManager.Off (roomResponseCode, OnClientReceiveFightingResponseCode);
				this.m_UserManager.On (roomResponseCode, OnClientReceiveFightingResponseCode);
				// COMPLETE TASK
				this.m_NextTask = "MiniGameFightingScene";
				this.OnTaskCompleted();
				CTaskUtil.Set (CTaskUtil.FIGHTING_ROOM_RESPONSE_CODE, roomResponseCode);
				CUICustomManager.Instance.ActiveLoading (false);
			}
		}

		protected virtual void OnClientReceiveFightingResponseCode(SocketIOEvent obj) {
			Debug.LogWarning (obj.ToString ());
		}

		#endregion


	}
}
