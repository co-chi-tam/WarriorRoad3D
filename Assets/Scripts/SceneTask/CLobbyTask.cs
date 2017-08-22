using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CLobbyTask : CSimpleClientTask {

		#region Properties

		protected CUILobbyManager m_LobbyManager;
		protected CMiniFightingData m_CurrentFightingRoomData;
		protected float m_UpdateEnergyPerSecond = 60f;
		protected float m_UpdateEnergyTimer = 0f;

		#endregion

		#region Constructor

		public CLobbyTask () : base ()
		{
			this.taskName = "LobbyScene";
			this.nextTask = "PlayScene";
			// ON CLIENT RECEIVE SKILLS
			this.RegisterEvent ("clientReceiveSkills", 				this.OnClientReceiveSkills);
			// ON CLIENT SET UP SKILL COMPLETED
			this.RegisterEvent ("clientCompletedSetupSkill", 		this.OnClientCompleteSetupSkills);
			// CHAT
			this.RegisterEvent ("clientReceiveChat", 				this.OnClientReceiveChat);
			// Fighting
			this.RegisterEvent ("clientWaitPlayerQueue",  			this.OnClientWaitPlayerQueue);
			this.RegisterEvent ("clientReceiveResultPlayerQueue",  	this.OnClientReceiveResultPlayerQueue);
			this.RegisterEvent ("clientCancelPlayerQueue",  		this.OnClientCancelPlayerQueue);
			// ENERGY
			this.RegisterEvent ("clientUpdateEnergy", 				this.OnClientUpdateEnergy);
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			// SKILL DATA
			var skillList = CTaskUtil.Get (CTaskUtil.SKILL_DATA_LIST) as List<CSkillData>;
			this.m_LobbyManager = CUILobbyManager.GetInstance ();
			this.m_LobbyManager.OnSetupHeroSkill ("Normal Attack", 3, skillList, null);
			this.m_UpdateEnergyTimer = this.m_UpdateEnergyPerSecond;
			// HERO DATA
			var heroData = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CHeroData;
			this.m_LobbyManager.UpdateEnergyText (heroData.currentEnergy.ToString (), heroData.maxEnergy.ToString ());
		}

	public override void UpdateTask (float dt)
		{
			base.UpdateTask (dt);
			if (this.m_UpdateEnergyTimer < 0f) {
				this.OnClientRequestEnergy ();
				this.m_UpdateEnergyTimer = this.m_UpdateEnergyPerSecond;
			} else {
				this.m_UpdateEnergyTimer -= dt;
			}
		}

		#endregion

		#region Energy

		public virtual void OnClientRequestEnergy() {
			if (this.m_UserManager.IsConnected() == false)
				return;
			this.m_UserManager.Emit ("clientRequestEnergy", new JSONObject());
		}

		public virtual void OnClientUpdateEnergy(SocketIOEvent obj) {
			Debug.Log (obj.ToString ());
			var currentEnergy = obj.data.GetField ("current").ToString ();
			var maxEnergy = obj.data.GetField ("max").ToString ();
			this.m_LobbyManager.UpdateEnergyText (currentEnergy, maxEnergy);
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

		public virtual void OnClientRequestJoinPlayerQueue() {
			CUICustom.ActiveMessage (true, "JOIN QUEUE", "Do you want join queue ?\nCost 1 energy", () => {
				if (this.m_UserManager.IsConnected() == false)
					return;
				var dictData = new Dictionary<string, string> ();
				dictData ["requestJoinQueue"] = "true";
				var jsonSend = JSONObject.Create (dictData);
				this.m_UserManager.Emit ("clientRequestJoinPlayerQueue", jsonSend);
			}, null);
		}

		public virtual void OnClientRequestLeavePlayerQueue() {
			if (this.m_UserManager.IsConnected() == false)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["requestJoinQueue"] = "false";
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("clientRequestLeavePlayerQueue", jsonSend);
		}

		public virtual void OnClientWaitPlayerQueue(SocketIOEvent obj) {
			Debug.LogWarning (obj.ToString ());
		}

		public virtual void OnClientCancelPlayerQueue (SocketIOEvent obj) {
			Debug.LogWarning (obj.ToString ());
		}

		public virtual void OnClientReceiveResultPlayerQueue(SocketIOEvent obj) {
			Debug.LogWarning (obj.ToString ());
			var responseData 	= obj.data;
			var playerDataStr 	= obj.data.GetField ("playerData").ToString();
			var enemyDataStr 	= obj.data.GetField ("enemyData").ToString();
			var randomSeedStr 	= obj.data.GetField ("randomSeed").ToString();
			var miniFightingData = new CMiniFightingData ();
			miniFightingData.playerData = TinyJSON.JSON.Load (playerDataStr).Make<CHeroData> ();
			miniFightingData.enemyData 	= TinyJSON.JSON.Load (enemyDataStr).Make<CHeroData> ();
			miniFightingData.randomSeed = int.Parse (randomSeedStr);
			CTaskUtil.Set (CTaskUtil.MINI_FIGHTING_DATA, 	miniFightingData);
			// COMPLETE TASK
			this.m_NextTask = "MiniGameFightingScene";
			this.OnTaskCompleted ();
		}

		#endregion

	}
}
