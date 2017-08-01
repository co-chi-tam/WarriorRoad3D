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

		[SerializeField]	protected SocketIOComponent m_SocketIO;

		private CUICustomManager m_UICustomManager;
		private float m_PingDelayTime = 3f;
		private JSONObject jsonObject;
		private int m_MessageReceived;

		public CUserData currentUser;

		public Action OnLoginCompleted;
		public Action<string> OnLoginFailed;

		public Action OnRegisterCompleted;
		public Action<string> OnRegisterFailed;

		public Action OnConectServerCompleted;
		public Action<string> OnConectServerFailed;

		public Action<string> OnReceiveMessage;

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
//			this.m_UICustomManager = CUICustomManager.GetInstance ();
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

		public virtual void Login() {
			var url = CTaskUtil.LOGIN_URL;
			var header = CTaskUtil.VERIFY_HEADERS;
			var loginParam = new Dictionary<string, string> ();
			loginParam ["uname"] = this.currentUser.userName;
			loginParam ["upass"] = this.currentUser.userPassword;
			var request = new CRequest (url, header);
			request.Post (url, loginParam, (CResult obj) =>  {
				var objContent = obj.ToJSONObject();
				if (objContent.ContainsKey ("resultCode")) {
					var userResponse = objContent["resultContent"] as Dictionary<string, object>;
					currentUser.userName = userResponse["userName"].ToString();
					currentUser.userEmail = userResponse["userEmail"].ToString();
					currentUser.userDisplayName = userResponse["userDisplayName"].ToString();
					currentUser.token = userResponse["token"].ToString();
					this.OnUserLoginCompleted ();
				} else if (objContent.ContainsKey ("errorCode")) {
					var errorContent = objContent["errorContent"].ToString();
					this.OnUserLoginFailed (errorContent);
				}
			}, (err) => {
				this.OnUserLoginFailed (err);
			}, null);
			// Start loading UI.
			if (this.m_UICustomManager != null) {
				this.m_UICustomManager.ActiveLoading (true);
			}
		}

		public virtual void Logout() {
		
		}

		public virtual void OnUserLoginCompleted() {
			if (this.OnLoginCompleted != null) {
				this.OnLoginCompleted ();
			}
			// TRY CONNECT TO SERVER
			this.OnUserConnectServer ();
		}

		public virtual void OnUserLoginFailed (string error) {
			if (this.OnLoginFailed != null) {
				this.OnLoginFailed (error);
			}
			// Start message UI.
			if (this.m_UICustomManager != null) {
				this.m_UICustomManager.ActiveMessage (true, error);
			}
		}

		#endregion

		#region Register

		public virtual void Register () {
			var url = CTaskUtil.REGISTER_URL;
			var header = CTaskUtil.VERIFY_HEADERS;
			var registerParam = new Dictionary<string, string> ();
			registerParam ["uname"] = this.currentUser.userName;
			registerParam ["upass"] = this.currentUser.userPassword;
			registerParam ["uemail"] = this.currentUser.userEmail;
			registerParam ["udisplayname"] = this.currentUser.userDisplayName;
			registerParam ["uloginmethod"] = "ANDROID";
			var request = new CRequest (url, header);
			request.Post (url, registerParam, (CResult obj) =>  {
				var objContent = obj.ToJSONObject();
				if (objContent.ContainsKey ("resultCode")) {
					var userResponse = objContent["resultContent"] as Dictionary<string, object>;
					currentUser.userName = userResponse["userName"].ToString();
					currentUser.userEmail = userResponse["userEmail"].ToString();
					currentUser.userDisplayName = userResponse["userDisplayName"].ToString();
					currentUser.token = userResponse["token"].ToString();
					this.OnUserRegisterCompleted ();
				} else if (objContent.ContainsKey ("errorCode")) {
					var errorContent = objContent["errorContent"].ToString();
					this.OnUserRegisterFailed (errorContent);
				}
			}, (err) => {
				this.OnRegisterFailed (err);
			}, null);
			// Start loading UI.
			if (this.m_UICustomManager != null) {
				this.m_UICustomManager.ActiveLoading (true);
			}
		}

		public virtual void OnUserRegisterCompleted() {
			if (this.OnRegisterCompleted != null) {
				this.OnRegisterCompleted ();
			}
			// TRY CONNECT TO SERVER
			this.OnUserConnectServer ();
		}

		public virtual void OnUserRegisterFailed (string error) {
			if (this.OnRegisterFailed != null) {
				this.OnRegisterFailed (error);
			}
			// Start message UI.
			if (this.m_UICustomManager != null) {
				this.m_UICustomManager.ActiveMessage (true, error);
			}
			Debug.LogError (error);
		}

		#endregion

		#region Connect server

		public virtual void OnUserConnectServer() {
			// UPDATE SOCKET CONNECT
			this.m_SocketIO.AddHeader ("username", this.currentUser.userName);
			this.m_SocketIO.AddHeader ("token", this.currentUser.token);
			this.m_SocketIO.Connect ();
			this.m_SocketIO.On ("connect", delegate(SocketIOEvent obj) {
				this.OnUserConnectServerCompleted ();

				this.m_SocketIO.On ("message", delegate(SocketIOEvent mes) {
					if (this.OnReceiveMessage != null) {
						this.OnReceiveMessage (mes.ToString());
					}	
				});

				this.m_SocketIO.On ("serverSendPing", delegate(SocketIOEvent oserverPingMsg) {
					this.m_MessageReceived += 1;
					Debug.LogWarning ("serverSendPing " + oserverPingMsg.ToString());	
				});

				this.m_SocketIO.On ("error", delegate(SocketIOEvent errorMsg) {
					this.OnUserRegisterFailed (errorMsg.ToString ());
				});
			});
		}

		public virtual void OnUserConnectServerCompleted() {
			if (this.OnConectServerCompleted != null) {
				this.OnConectServerCompleted ();
			}
			// Stop loading UI.
			if (this.m_UICustomManager != null) {
				this.m_UICustomManager.ActiveLoading (false);
			}
		}

		public virtual void OnUserRegisterClose() {
			Debug.Log ("Socket client closed.");
		}

		public virtual void OnUserConnectServerFailed (string error) {
			if (this.OnConectServerFailed != null) {
				this.OnConectServerFailed (error);
			}
			// Start message UI.
			if (this.m_UICustomManager != null) {
				this.m_UICustomManager.ActiveMessage (true, error);
			}
			Debug.LogError (error);
		}

		private void SendPing() {
			if (this.m_SocketIO.IsConnected == false)
				return;
			this.m_SocketIO.Emit ("clientSendPing", jsonObject);
		}

		#endregion
		
	}
}
