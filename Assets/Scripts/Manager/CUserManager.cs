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

		// PRIVATE FIELDS
		// PING
		protected float m_PingDelayTime = 3f;
		protected JSONObject jsonPingObject;
		protected int m_MessageReceived;
		// INITED
		protected bool m_Inited = false;
		protected bool m_LogedIn = false;

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
		}

		protected virtual void OnApplicationQuit() {
			this.OnClientLeaveGame ();
		}

		protected virtual void OnApplicationFocus (bool value) {
#if !UNITY_EDITOR
			if (value == false) {
				this.OnClientInitAccount ();
			}
#endif
		}

		protected virtual void OnApplicationPause (bool value) {
#if !UNITY_EDITOR
			if (value) {
				this.OnClientInitAccount ();
			}
#endif
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
			// INITED
			this.m_Inited = false;
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
			Debug.LogWarning ("OnClientLoginCompleted USER " + user.userName + " PASSWORD " + user.userPassword);
			if (this.OnEventLoginCompleted != null) {
				this.OnEventLoginCompleted (user);
			}
			// TRY CONNECT TO SERVER
			this.OnClientConnectServer ();
			// SAVE USER DATA
			CTaskUtil.Set (CTaskUtil.USER_DATA, user);
			// LOGGED IN
			this.m_LogedIn = true;
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
			// INIT
			this.m_Inited = false;
		}

		public virtual void OnClientRegisterCompleted(CUserData user) {
			if (this.OnEventRegisterCompleted != null) {
				this.OnEventRegisterCompleted (user);
			}
			// TRY CONNECT TO SERVER
			this.OnClientConnectServer ();
			// SAVE USER DATA
			CTaskUtil.Set (CTaskUtil.USER_DATA, user);
			// LOGGED IN
			this.m_LogedIn = true;
		}

		#endregion

		#region Notice

		public virtual void OnClientError (string error) {
			if (this.OnEventClientError != null) {
				this.OnEventClientError (error);
			}
			// Start message UI.
			CUICustomManager.Instance.ActiveNotice (true, error);
			Debug.LogError (error);
		}

		public virtual void OnClientWarning (string warning) {
			// Start message UI.
			CUICustomManager.Instance.ActiveNotice (true, warning);
			Debug.LogWarning (warning);
		}

		public virtual void OnClientNotice (string notice) {
			// Start message UI.
			CUICustomManager.Instance.ActiveNotice (true, notice);
			Debug.Log (notice);
		}

		#endregion

		#region Socket

		public virtual void On (string name, Action<SocketIOEvent> obj) {
			if (this.m_SocketIO == null) {
				return;
			}
			this.m_SocketIO.On (name, obj);
		} 

		public virtual void Off (string name, Action<SocketIOEvent> obj) {
			if (this.m_SocketIO == null) {
				return;
			}
			this.m_SocketIO.Off (name, obj);
		}

		public virtual void Emit (string name, JSONObject data) {
			if (this.m_SocketIO == null) {
				return;
			}
			this.m_SocketIO.Emit (name, data);
		}

		public virtual void ClearAll () {
			this.m_SocketIO.ClearAll ();
		}

		public virtual bool IsConnected() {
			if (this.m_SocketIO == null) {
				return false;
			}
			return this.m_SocketIO.IsConnected;
		}

		#endregion

		#region Connect server

		public virtual void OnClientConnectServer() {
			// UPDATE SOCKET CONNECT
			this.m_SocketIO.AddHeader ("username", this.currentUser.userName);
			this.m_SocketIO.AddHeader ("token", this.currentUser.token);
			this.m_SocketIO.Connect ();
			this.m_SocketIO.On ("connect", delegate(SocketIOEvent obj) {
				// CONNECTED SERVER
				this.OnClientConnectCompleted ();
				// MESSAGES
				this.m_SocketIO.On ("message", delegate(SocketIOEvent mes) {
					this.OnClientReceiveMessage (mes.data);
				});
				// PING
				this.m_SocketIO.On ("serverSendPing", delegate(SocketIOEvent onServerPingMsg) {
					Debug.LogWarning ("serverSendPing " + onServerPingMsg.ToString());
					this.m_MessageReceived += 1;
					// RE-INIT WHEN SERVER NOT RESPONSE
//					if (this.m_MessageReceived > 3 
//						&& this.m_Inited == false 
//						&& this.m_LogedIn == true) {
//						this.m_MessageReceived = 0;
//						this.OnClientInitAccount ();
//					}
				});
				// INIT
//				this.m_SocketIO.On ("clientInit", delegate(SocketIOEvent onInitMsg) {
//					Debug.LogWarning ("clientInit " + onInitMsg.ToString());
//					this.OnClientInitAccount ();
//				});
				// TASK MANAGER
				this.m_SocketIO.On ("clientChangeTask", (SocketIOEvent onClientChangeTaskMsg) => {
					Debug.LogWarning ("clientChangeTask " + onClientChangeTaskMsg.ToString());
					this.OnClientChangeSceneTask (onClientChangeTaskMsg.data);
				});
				// DEBUG
				this.m_SocketIO.On ("debug", delegate(SocketIOEvent debugMsg) {
#if DEBUG_MODE
					Debug.Log (debugMsg.ToString ());
#endif
				});
				// NOTICE
				this.m_SocketIO.On ("notice", delegate(SocketIOEvent noticeMsg) {
					var noticeData = noticeMsg.data.ToString();
					if (string.IsNullOrEmpty (noticeData) == false) {
						this.OnClientNotice (noticeMsg.ToString ());
					} else {
						this.OnClientNotice ("WARNING: NOT DEFINE.");
					}
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
			// INIT ACCOUNT
			this.OnClientInitAccount ();
		}

		public virtual void OnClientConnectServerFailed (string error) {
			this.OnClientError (error);
			// Start message UI.
			CUICustomManager.Instance.ActiveNotice (true, error);
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
			// INIT
			this.m_Inited = false;
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
			// COMPLETE TASK
			CRootTask.Instance.ProcessNextTask (processTask);
			CRootTask.Instance.GetCurrentTask().OnTaskCompleted();
			CUICustomManager.Instance.ActiveLoading (false);
			// INITED
			this.m_Inited = true;
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

	}
}
