using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;
using SocketIO;

namespace WarriorRoad {
	public class CUserManager : CMonoSingleton<CUserManager> {

		#region Fields

		// SOCKET IO
		[Header ("SocketIO")]
		[SerializeField]	protected SocketIOComponent m_SocketIO;

		// CURRENT USER
		[Header ("USER DATA")]
		public CUserData currentUser;
		[Header ("HERO DATA")]
		public CHeroData currentHero;

		// EVENT
		public Action<CUserData> OnEventLoginCompleted;
		public Action<CUserData> OnEventRegisterCompleted;
		public Action OnEventConectServerCompleted;
		public Action<JSONObject> OnEventReceiveMessage;
		public Action<string> OnEventClientError;
		public Action OnEventInitUserCompleted;

		// PRIVATE FIELDS
		private float m_PingDelayTime = 3f;
		private JSONObject jsonPingObject;
		private int m_MessageReceived;
		private List<CCharacterData> m_MapObjects;
		protected CBingoRoomData m_CurrentBingoRoomData;

		#endregion

		#region Implementation MonoBehavious

		protected override void Awake() {
			base.Awake ();
			DontDestroyOnLoad (this.gameObject);
			this.currentUser = new CUserData ();
			// SET UP CONNECT
			this.m_SocketIO.url = CTaskUtil.SOCKET_HOST;
			this.m_SocketIO.autoConnect = false;
		}

		protected virtual void Start() {
			var msgDict = new Dictionary<string, string> ();
			msgDict.Add ("msg", "This is message ping.");
			jsonPingObject = JSONObject.Create (msgDict);
			// LEAVE GAME
//			this.OnClientLeaveGame ();
		}

		protected virtual void LateUpdate() {
			// SEND PING
			m_PingDelayTime -= Time.deltaTime;
			if (m_PingDelayTime <= 0f) {
				SendPing ();
				m_PingDelayTime = 3f;
			}

			if (Input.GetKeyDown (KeyCode.A)) {
				var dict = new Dictionary<string, string> ();
				dict ["DATA"] = "DATA AAAAAAAAAAAAA";
				this.OnClientSendDataBingoRoom ("onBingoRoomPlayerReady", JSONObject.Create (dict));
			}
		}

		protected virtual void OnApplicationQuit() {
			this.OnClientLeaveGame ();
		}

		protected virtual void OnApplicationFocus (bool value) {
//#if !UNITY_EDITOR
//			if (value == false) {
//				this.OnClientLeaveGame ();
//			}
//#endif
		}

		protected virtual void OnApplicationPause (bool value) {
//#if !UNITY_EDITOR
//			if (value) {
//				this.OnClientLeaveGame ();
//			}
//#endif
		}

		#endregion

		#region Login

		public void InputUserName(InputField value) {
			this.currentUser.userName = value.text;
		}

		public void InputUserPassword (InputField value) {
			this.currentUser.userPassword = value.text;
		}

		public void InputUserEmail (InputField value) {
			this.currentUser.userEmail = value.text;
		}

		public void InputUserDisplayName (InputField value) {
			this.currentUser.userDisplayName = value.text;
		}

		public virtual void LoginUser() {
			var url = CTaskUtil.LOGIN_URL;
			var header = CTaskUtil.VERIFY_HEADERS;
			var loginParam = new Dictionary<string, string> ();
			loginParam ["uname"] = this.currentUser.userName;
			loginParam ["upass"] = this.currentUser.userPassword;
			bool canSubmit = string.IsNullOrEmpty (this.currentUser.userName) == false
				&& string.IsNullOrEmpty (this.currentUser.userPassword) == false;
			if (canSubmit) {
				var request = new CRequest (url, header);
				request.Post (url, loginParam, (CResult obj) => {
					var objContent = obj.ToJSONObject ();
					if (objContent.ContainsKey ("resultCode")) {
						var userResponse = objContent ["resultContent"] as Dictionary<string, object>;
						currentUser.userName = userResponse ["userName"].ToString ();
						currentUser.userEmail = userResponse ["userEmail"].ToString ();
						currentUser.userDisplayName = userResponse ["userDisplayName"].ToString ();
						currentUser.token = userResponse ["token"].ToString ();
						this.OnClientLoginCompleted (currentUser);
					} else if (objContent.ContainsKey ("errorCode")) {
						var errorContent = objContent ["errorContent"].ToString ();
						this.OnClientError (errorContent);
					}
				}, (err) => {
					this.OnClientError (err);
				}, null);
				// Start loading UI.
				CUICustomManager.Instance.ActiveLoading (true);
			} else {
				this.OnClientError ("Field do not empty.");
			}
		}

		public virtual void Logout() {
			PlayerPrefs.SetString (CTaskUtil.USER_NAME, string.Empty);
			PlayerPrefs.SetString (CTaskUtil.USER_PASSWORD, string.Empty);
			// COMPLETE TASK
			CRootTask.Instance.ProcessNextTask ("LoginScene");
			CRootTask.Instance.GetCurrentTask().OnTaskCompleted();
			CUICustomManager.Instance.ActiveLoading (false);
		}

		public virtual void OnClientLoginCompleted(CUserData user) {
			if (this.OnEventLoginCompleted != null) {
				this.OnEventLoginCompleted (user);
			}
			// TRY CONNECT TO SERVER
			this.OnClientConnectServer ();
			// SAVE USER DATA
			CTaskUtil.Set (CTaskUtil.USER_DATA, user);
		}

		#endregion

		#region Register

		public virtual void RegisterUser () {
			var url = CTaskUtil.REGISTER_URL;
			var header = CTaskUtil.VERIFY_HEADERS;
			var registerParam = new Dictionary<string, string> ();
			registerParam ["uname"] = this.currentUser.userName;
			registerParam ["upass"] = this.currentUser.userPassword;
			registerParam ["uemail"] = this.currentUser.userEmail;
			registerParam ["udisplayname"] = this.currentUser.userDisplayName;
			registerParam ["uloginmethod"] = "ANDROID";
			bool canSubmit = string.IsNullOrEmpty (this.currentUser.userName) == false
			                 && string.IsNullOrEmpty (this.currentUser.userPassword) == false
			                 && string.IsNullOrEmpty (this.currentUser.userEmail) == false
			                 && string.IsNullOrEmpty (this.currentUser.userDisplayName) == false;
			if (canSubmit) {
				var request = new CRequest (url, header);
				request.Post (url, registerParam, (CResult obj) => {
					var objContent = obj.ToJSONObject ();
					if (objContent.ContainsKey ("resultCode")) {
						var userResponse = objContent ["resultContent"] as Dictionary<string, object>;
						currentUser.userName = userResponse ["userName"].ToString ();
						currentUser.userEmail = userResponse ["userEmail"].ToString ();
						currentUser.userDisplayName = userResponse ["userDisplayName"].ToString ();
						currentUser.token = userResponse ["token"].ToString ();
						this.OnClientRegisterCompleted (currentUser);
					} else if (objContent.ContainsKey ("errorCode")) {
						var errorContent = objContent ["errorContent"].ToString ();
						this.OnClientError (errorContent);
					}
				}, (err) => {
					this.OnClientError (err);
				}, null);
				// Start loading UI.
				CUICustomManager.Instance.ActiveLoading (true);
			} else {
				this.OnClientError ("Field do not empty.");
			}
		}

		public virtual void OnClientRegisterCompleted(CUserData user) {
			if (this.OnEventRegisterCompleted != null) {
				this.OnEventRegisterCompleted (user);
			}
			// TRY CONNECT TO SERVER
			this.OnClientConnectServer ();
			// SAVE USER DATA
			CTaskUtil.Set (CTaskUtil.USER_DATA, user);
		}

		public virtual void OnClientError (string error) {
			if (this.OnEventClientError != null) {
				this.OnEventClientError (error);
			}
			// Start message UI.
			CUICustomManager.Instance.ActiveMessage (true, error);
			Debug.LogError (error);
		}

		public virtual void OnClientWarning (string warning) {
			// Start message UI.
			CUICustomManager.Instance.ActiveMessage (true, warning);
			Debug.LogWarning (warning);
		}

		#endregion

		#region Connect server

		public virtual void OnClientConnectServer() {
			// UPDATE SOCKET CONNECT
			this.m_SocketIO.AddHeader ("username", this.currentUser.userName);
			this.m_SocketIO.AddHeader ("token", this.currentUser.token);
			this.m_SocketIO.Connect ();
			this.m_SocketIO.On ("connect", delegate(SocketIOEvent obj) {
				this.OnClientConnectCompleted ();
				// MESSAGEs
				this.m_SocketIO.On ("message", delegate(SocketIOEvent mes) {
					this.OnClientReceiveMessage (mes.data);
				});
				// INIT CLIENT
				this.m_SocketIO.On ("clientInit", delegate(SocketIOEvent onClientInitMsg) {
					Debug.LogWarning ("clientInit " + onClientInitMsg.ToString());
					this.OnClientInitAccount ();
				});
				// PING
				this.m_SocketIO.On ("serverSendPing", delegate(SocketIOEvent onServerPingMsg) {
					Debug.LogWarning ("serverSendPing " + onServerPingMsg.ToString());
					this.m_MessageReceived += 1;	
				});
				// TASK MANAGER
				this.m_SocketIO.On ("clientChangeTask", (SocketIOEvent onClientChangeTaskMsg) => {
					Debug.LogWarning ("clientChangeTask " + onClientChangeTaskMsg.ToString());
					this.OnClientChangeSceneTask (onClientChangeTaskMsg.data);
				});
				// MAP
				this.m_SocketIO.On ("clientInitMap", delegate(SocketIOEvent onClientInitGameMsg) {
					Debug.LogWarning ("clientInitMap " + onClientInitGameMsg.ToString());	
					this.OnClientReceiveMapObjects (onClientInitGameMsg.data);
				});
				// DICE
				this.m_SocketIO.On ("clientReceiveDice", delegate(SocketIOEvent onClientReceiveDiceMsg) {
					Debug.LogWarning ("OnClientReceiveDice " + onClientReceiveDiceMsg.ToString());	
					this.OnClientReceiveDice (onClientReceiveDiceMsg.data);
				}); 
				// UPDATE CLIENT
				this.m_SocketIO.On ("clientUpdated", delegate(SocketIOEvent onClientUpdateMsg) {
					Debug.LogWarning ("clientUpdated " + onClientUpdateMsg.ToString());	
				}); 
				// SKILL
				this.m_SocketIO.On ("clientReceiveSkills", delegate(SocketIOEvent onClientRevSkills) {
					Debug.LogError ("clientReceiveSkills " + onClientRevSkills.ToString());
				});

				this.m_SocketIO.On ("clientCompletedSetupSkill", delegate(SocketIOEvent onClientSetupSkill) {
					Debug.LogWarning ("clientCompletedSetupSkill " + onClientSetupSkill.ToString());
					this.OnClientCompleteSetupSkills (onClientSetupSkill.data);
				});
				// CHAT
				this.m_SocketIO.On ("clientReceiveChat", delegate(SocketIOEvent onClientRevChat) {
					Debug.LogWarning ("clientReceiveChat " + onClientRevChat.ToString());
					this.OnClientReceiveChat (onClientRevChat.data);
				});
				// BINGO
				this.m_SocketIO.On ("clientReceiveBingoRoomList", delegate(SocketIOEvent onClientReveiceRooms) {
					Debug.LogWarning ("clientReceiveBingoRoomList " + onClientReveiceRooms.ToString());
					this.OnClientReceiveBingoRoomList (onClientReveiceRooms.data);
				});

				this.m_SocketIO.On ("clientInitBingoRoom", delegate(SocketIOEvent onClientInitBingoRoom) {
					Debug.LogWarning ("clientInitBingoRoom " + onClientInitBingoRoom.ToString());
					this.OnClientInitBingoRoom (onClientInitBingoRoom.data);
				});

				this.m_SocketIO.On ("onClientBingoRoomReceiveBoard", delegate(SocketIOEvent onClientBingoBoard) {
					Debug.LogWarning ("onClientBingoRoomReceiveBoard " + onClientBingoBoard.ToString());
					this.OnClientBingoRoomReceiveBoard (onClientBingoBoard.data);
				});

				// ERROR
				this.m_SocketIO.On ("error", delegate(SocketIOEvent errorMsg) {
					var errorData = errorMsg.data.ToString();
					if (string.IsNullOrEmpty (errorData) == false) {
						this.OnClientError (errorMsg.ToString ());
					} else {
						this.OnClientError ("ERROR: NOT DEFINE.");
					}
				});
				// WARNING
				this.m_SocketIO.On ("warning", delegate(SocketIOEvent warningMsg) {
					var warningData = warningMsg.data.ToString();
					if (string.IsNullOrEmpty (warningData) == false) {
						this.OnClientWarning (warningMsg.ToString ());
					} else {
						this.OnClientWarning ("WARNING: NOT DEFINE.");
					}
				});
			});
		}

		#endregion

		#region MESSAGE

		public virtual void OnClientReceiveMessage(JSONObject msg) {
			if (this.OnEventReceiveMessage != null) {
				this.OnEventReceiveMessage (msg);
			}	
		}

		#endregion

		#region CLIENT CONNECT SERVER EVENT

		public virtual void OnClientConnectCompleted() {
			if (this.OnEventConectServerCompleted != null) {
				this.OnEventConectServerCompleted ();
			}
		}

		public virtual void OnClientConnectServerFailed (string error) {
			this.OnClientError (error);
			// Start message UI.
			CUICustomManager.Instance.ActiveMessage (true, error);
			Debug.LogError (error);
		}

		private void SendPing() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientSendPing", jsonPingObject);
		}

		#endregion

		#region Account

		public virtual void OnClientInitAccount() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientInitAccount", new JSONObject());
		}

		public virtual void OnClientLeaveGame () {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientLeaveGame", new JSONObject());
		}

		#endregion

		#region Client task manager

		public virtual void OnClientChangeSceneTask(JSONObject receiveData) {
			// NEXT TASK
			var processTask = receiveData.GetField ("taskChange").ToString().Replace ("\"", string.Empty);
			switch (processTask) {
			case "CreateHeroScene":
				this.OnClientSetupCreateHeroScene (receiveData);
				break;
			case "PlayScene": 
				this.OnClientSetupPlayScene (receiveData);
				break;
			case "LoginScene":
				this.OnClientSetupLoginScene (receiveData);
				break;
			case "LobbyScene":
				this.OnClientSetupLobbyScene (receiveData);
				break;
			default:
				processTask = "LoginScene";
				this.OnClientSetupLoginScene (receiveData);
				break;
			}
			// TRIGGER EVENT
			if (this.OnEventInitUserCompleted != null) {
				this.OnEventInitUserCompleted ();
			}
			// COMPLETE TASK
			CRootTask.Instance.ProcessNextTask (processTask);
			CRootTask.Instance.GetCurrentTask().OnTaskCompleted();
			CUICustomManager.Instance.ActiveLoading (false);
		}

		protected virtual void OnClientSetupCreateHeroScene(JSONObject receiveData) {
			// HERO DATA
			var isHeroData = receiveData.HasField ("heroData");
			this.currentHero = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CHeroData;
			if (isHeroData) {
				var heroDataJson = receiveData.GetField ("heroData").ToString ();
				this.currentHero = TinyJSON.JSON.Load (heroDataJson).Make <CHeroData> ();
			} 
			CTaskUtil.Set (CTaskUtil.HERO_DATA, this.currentHero);
			// HEROES TEMPLATE
			var isHeroTemplate = receiveData.HasField ("heroesTemplate");
			Dictionary<string, CCharacterData> heroesTemplate = CTaskUtil.Get (CTaskUtil.HERO_TEMPLATES) as Dictionary<string, CCharacterData>;
			if (isHeroTemplate) {
				var heroTemplateJson = receiveData.GetField ("heroesTemplate").ToString ();
				heroesTemplate = TinyJSON.JSON.Load (heroTemplateJson).Make <Dictionary<string, CCharacterData>> ();
			}
			CTaskUtil.Set (CTaskUtil.HERO_TEMPLATES, heroesTemplate);
		}

		protected virtual void OnClientSetupPlayScene(JSONObject receiveData) {
			// HERO DATA
			var isHeroData = receiveData.HasField ("heroData");
			this.currentHero = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CHeroData;
			if (isHeroData) {
				var heroDataJson = receiveData.GetField ("heroData").ToString ();
				this.currentHero = TinyJSON.JSON.Load (heroDataJson).Make <CHeroData> ();
			} 
			CTaskUtil.Set (CTaskUtil.HERO_DATA, this.currentHero);
		}

		protected virtual void OnClientSetupLoginScene(JSONObject receiveData) {

		}

		public virtual void OnClientSetupLobbyScene(JSONObject receiveData) {
			// HERO DATA
			var isHeroData = receiveData.HasField ("heroData");
			this.currentHero = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CHeroData;
			if (isHeroData) {
				var heroDataJson = receiveData.GetField ("heroData").ToString ();
				this.currentHero = TinyJSON.JSON.Load (heroDataJson).Make <CHeroData> ();
			} 
			CTaskUtil.Set (CTaskUtil.HERO_DATA, this.currentHero);
			// SKILL DATA
			var isSkillData = receiveData.HasField ("skillDatas");
			if (isSkillData) {
				var skillList = receiveData.GetField ("skillDatas").list;
				var tmpSkillList = new List<CSkillData>();
				for (int i = 0; i < skillList.Count; i++) {
					var objectStr = skillList [i].ToString ();
					var skillData = TinyJSON.JSON.Load (objectStr).Make<CSkillData> ();
					tmpSkillList.Add (skillData);
				}
				CTaskUtil.Set (CTaskUtil.SKILL_DATA_LIST, tmpSkillList);
			}
		}

		#endregion

		#region Hero

		public virtual void OnClientCreateHero(string heroType, string heroName) {
			if (this.m_SocketIO.IsConnected == false)
				return;
			var heroSubmitData = new Dictionary<string, string> ();
			heroSubmitData.Add ("htype", heroType);
			heroSubmitData.Add ("hname", heroName);
			heroSubmitData.Add ("uname", this.currentUser.userName);
			heroSubmitData.Add ("token", this.currentUser.token);
			var jsonCreateHero = JSONObject.Create (heroSubmitData);
			this.m_SocketIO.Emit ("clientCreateHero", jsonCreateHero);
		}

		public virtual void OnClientUpdateHero(CCharacterData clientData) {
			if (this.m_SocketIO.IsConnected == false)
				return;
			if (clientData == null)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["hhealth"] = clientData.characterHealthPoint.ToString();
			var jsonSend = JSONObject.Create (dictData);
			this.m_SocketIO.Emit ("clientUpdateHero", jsonSend);
		}

		#endregion

		#region Skill

		public virtual void OnClientInitSkill() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientInitSkill", new JSONObject());
		}

		public virtual void OnClientSetupSkills (CSkillData[] skills) {
			if (this.m_SocketIO.IsConnected == false)
				return;
			var dictData = new Dictionary<string, string> ();
			var setupSkills = "";
			for (int i = 0; i < skills.Length; i++) {
				var skillData = skills [i];
				setupSkills += skillData.objectName + (i < skills.Length - 1 ? "," : "");
			}
			dictData ["skills"] = setupSkills;
			var jsonSend = JSONObject.Create (dictData);
			this.m_SocketIO.Emit ("clientSetupSkills", jsonSend);
		}

		public virtual void OnClientCompleteSetupSkills (JSONObject data) {
			var currentTask = CRootTask.Instance.GetCurrentTask ();
			if (currentTask.taskName == "LobbyScene") {
				currentTask.OnTaskCompleted ();
			} else {
				Debug.LogError ("ERROR TASK: NOT CORRECT TASK.");
			}
		}

		#endregion

		#region Map

		public virtual void OnClientInitMap() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientInitMap", new JSONObject());
		}

		public virtual void OnClientReceiveMapObjects(JSONObject data) {
			var mapList = data.GetField ("mapBlocks").list;
			this.m_MapObjects = new List<CCharacterData> ();
			for (int i = 0; i < mapList.Count; i++) {
				var objectStr = mapList [i].ToString ();
				if (objectStr.Equals ("null") == false) {
					var objectData = TinyJSON.JSON.Load (objectStr).Make<CCharacterData> ();
					this.m_MapObjects.Add (objectData);
				} else {
					this.m_MapObjects.Add (null);
				}
			}
			var mapPath = data.GetField ("mapPath").ToString ().Replace ("\"", string.Empty);
			CMapManager.Instance.GenerateRoadMap (mapPath, 7);
			CMapManager.Instance.OnMapGenerateComplete -= OnLoadBlockMapCompleted;
			CMapManager.Instance.OnMapGenerateComplete += OnLoadBlockMapCompleted;
		}

		private void OnLoadBlockMapCompleted() {
			CMapManager.Instance.LoadMapObject (this.m_MapObjects);
		}

		public virtual void OnClientCompletedMap() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientCompletedMap", new JSONObject());
		}

		public virtual void OnClientEndGame() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientEndGame", new JSONObject());
		}

		#endregion

		#region Dice

		public virtual void OnClientRollDice() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientRollDice", new JSONObject());
		}

		public virtual void OnClientReceiveDice(JSONObject data) {
			var step = int.Parse (data.GetField ("diceStep").ToString());
			var curEnergy = int.Parse (data.GetField ("currentEnergy").ToString());
			var maxEnergy = int.Parse (data.GetField ("maxEnergy").ToString());
			CGameManager.Instance.OnPlayerUpdateStep (step, curEnergy, maxEnergy);
		}

		#endregion

		#region Chat

		public virtual void OnClientSendChat(string chat) {
			if (this.m_SocketIO.IsConnected == false)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["chatString"] = chat.ToString();
			var jsonSend = JSONObject.Create (dictData);
			this.m_SocketIO.Emit ("clientSendChat", jsonSend);
		}

		public virtual void OnClientReceiveChat(JSONObject data) {
			var isHasChat = data.HasField ("chatStr");
			if (isHasChat) {
				// WARNING
				if (CSceneManager.Instance.GetActiveSceneName () != "PlayScene")
					return;
				var chatOwner = data.GetField ("chatOwner").ToString().Replace ("\"", string.Empty);
				var chatStr = data.GetField ("chatStr").ToString().Replace ("\"", string.Empty);
				var chat = chatOwner + ": " + chatStr; 
				var chatData = new CChatData () { 
					chatOwner = chatOwner, 
					chatStr = chat, 
					isMine = chatOwner == this.currentHero.objectName
				};
				CUIGameManager.Instance.ReceiveChatText (chatData);
			}
		}

		#endregion 

		#region Bingo

		public virtual void OnClientGetBingoRoomList() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("onClientGetBingoRoomList", new JSONObject());
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientReceiveBingoRoomList(JSONObject data) {
			if (CSceneManager.Instance.GetActiveSceneName () != "LobbyScene")
				return;
			var roomList = data.GetField ("roomListData").list;
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
			if (this.m_SocketIO.IsConnected == false)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["roomIndex"] = roomIndex.ToString ();
			var jsonSend = JSONObject.Create (dictData);
			this.m_SocketIO.Emit ("onClientRequestJoinBingoRoom", jsonSend);
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientRequestLeaveBingoRoom () {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("onClientRequestLeaveBingoRoom", new JSONObject());
			if (this.m_CurrentBingoRoomData != null) {
				var roomResponseCode = this.m_CurrentBingoRoomData.eventResponseCode;
				this.m_SocketIO.Off (roomResponseCode, OnClientReceiveBingoResponseCode);
			}
			this.m_CurrentBingoRoomData = null;
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientInitBingoRoom(JSONObject data) {
			var isRoomData = data.HasField ("roomData");
			if (isRoomData) {
				var objStr = data.GetField ("roomData").ToString ();
				this.m_CurrentBingoRoomData = TinyJSON.JSON.Load (objStr).Make<CBingoRoomData> ();
				var roomResponseCode = this.m_CurrentBingoRoomData.eventResponseCode;
				// REGISTER EVENT
				this.m_SocketIO.Off (roomResponseCode, OnClientReceiveBingoResponseCode);
				this.m_SocketIO.On (roomResponseCode, OnClientReceiveBingoResponseCode);
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

		public virtual void OnClientSendDataBingoRoom(string eventName, JSONObject eventData) {
			if (this.m_SocketIO.IsConnected == false)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["eventName"] = eventName;
			dictData ["eventData"] = eventData.ToString().Replace ("\"", "'");
			var jsonSend = JSONObject.Create (dictData);
			this.m_SocketIO.Emit ("onClientSendDataBingoRoom", jsonSend);
		}

		public virtual void OnBingoRoomPlayerReady () {
			var dict = new Dictionary<string, string> ();
			dict ["IsReady"] = "YEAH";
			this.OnClientSendDataBingoRoom ("onBingoRoomPlayerReady", JSONObject.Create (dict));
			CUICustomManager.Instance.ActiveLoading (true);
		}

		public virtual void OnClientBingoRoomReceiveBoard (JSONObject data) {
			var isBingoBoard = data.HasField ("bingoBoard");
			if (isBingoBoard) {
				var boardList = data.GetField ("bingoBoard").list;
				var boardStr = new string [boardList.Count];
				for (int i = 0; i < boardList.Count; i++) {
					boardStr [i] = boardList [i].ToString();
				}
				CUIMiniGameBingoManager.Instance.LoadBingoBoard (boardStr);
				CUICustomManager.Instance.ActiveLoading (false);
			}
		}
			
		#endregion
		
	}
}
