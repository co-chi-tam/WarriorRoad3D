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
		public CCharacterData currentHeroData;

		// EVENT
		public Action<CUserData> OnEventLoginCompleted;
		public Action<CUserData> OnEventRegisterCompleted;
		public Action OnEventConectServerCompleted;
		public Action<JSONObject> OnEventReceiveMessage;
		public Action<string> OnEventClientError;
		public Action OnEventInitUserCompleted;

		// PRIVATE FIELDS
		private float m_PingDelayTime = 3f;
		private JSONObject jsonObject;
		private int m_MessageReceived;

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
			jsonObject = JSONObject.Create (msgDict);
		}

		protected virtual void LateUpdate() {
			m_PingDelayTime -= Time.deltaTime;
			if (m_PingDelayTime <= 0f) {
				SendPing ();
				m_PingDelayTime = 3f;
			}
		}

		protected virtual void OnGUI() {
			if (this.m_SocketIO != null) {
				GUILayout.Label (" ==== " + this.m_MessageReceived);
			}
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

		#endregion

		#region Connect server

		public virtual void OnClientConnectServer() {
			// UPDATE SOCKET CONNECT
			this.m_SocketIO.AddHeader ("username", this.currentUser.userName);
			this.m_SocketIO.AddHeader ("token", this.currentUser.token);
			this.m_SocketIO.Connect ();
			this.m_SocketIO.On ("connect", delegate(SocketIOEvent obj) {
				this.OnClientConnectCompleted ();

				this.m_SocketIO.On ("message", delegate(SocketIOEvent mes) {
					this.OnClientReceiveMessage (mes.data);
				});

				this.m_SocketIO.On ("clientInit", delegate(SocketIOEvent onClientInitMsg) {
					Debug.LogWarning ("clientInit " + onClientInitMsg.ToString());
					this.OnClientInitAccount ();
				});

				this.m_SocketIO.On ("serverSendPing", delegate(SocketIOEvent onServerPingMsg) {
					this.m_MessageReceived += 1;
					Debug.LogWarning ("serverSendPing " + onServerPingMsg.ToString());	
				});

				this.m_SocketIO.On ("clientChangeTask", (SocketIOEvent onClientChangeTaskMsg) => {
					Debug.LogWarning ("clientChangeTask " + onClientChangeTaskMsg.ToString());
					this.OnClientChangeSceneTask (onClientChangeTaskMsg.data);
				});

				this.m_SocketIO.On ("clientInitMap", delegate(SocketIOEvent onClientInitGameMsg) {
					Debug.LogWarning ("clientInitMap " + onClientInitGameMsg.ToString());	
					this.OnClientReceiveMap (onClientInitGameMsg.data);
				});

				this.m_SocketIO.On ("error", delegate(SocketIOEvent errorMsg) {
					this.OnClientError (errorMsg.ToString ());
				});
			});
		}

		public virtual void OnClientReceiveMessage(JSONObject msg) {
			if (this.OnEventReceiveMessage != null) {
				this.OnEventReceiveMessage (msg);
			}	
		}

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
			this.m_SocketIO.Emit ("clientSendPing", jsonObject);
		}

		public virtual void OnClientInitAccount() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientInitAccount", new JSONObject());
		}

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
			CCharacterData heroData = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CCharacterData;
			if (isHeroData) {
				var heroDataJson = receiveData.GetField ("heroData").ToString ();
				heroData = TinyJSON.JSON.Load (heroDataJson).Make <CCharacterData> ();
			} 
			CTaskUtil.Set (CTaskUtil.HERO_DATA, heroData);
			this.currentHeroData = heroData;
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
			CCharacterData heroData = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CCharacterData;
			if (isHeroData) {
				var heroDataJson = receiveData.GetField ("heroData").ToString ();
				heroData = TinyJSON.JSON.Load (heroDataJson).Make <CCharacterData> ();
			} 
			CTaskUtil.Set (CTaskUtil.HERO_DATA, heroData);
			this.currentHeroData = heroData;
		}

		protected virtual void OnClientSetupLoginScene(JSONObject receiveData) {

		}

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

		public virtual void OnClientInitMap() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientInitMap", new JSONObject());
		}

		public virtual void OnClientReceiveMap(JSONObject data) {
			var mapList = data.GetField ("mapBlocks").list;
			var mapBlock = new List<CCharacterData> ();
			for (int i = 0; i < mapList.Count; i++) {
				var objectStr = mapList [i].ToString ();
				if (objectStr.Equals ("null") == false) {
					var objectData = TinyJSON.JSON.Load (objectStr).Make<CCharacterData> ();
					mapBlock.Add (objectData);
				} else {
					mapBlock.Add (null);
				}
			}
			CMapManager.Instance.LoadMapObject (mapBlock);
		}

		#endregion
		
	}
}
