using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CLoginTask : CSimpleClientTask {

		#region Properties

		#endregion

		#region Constructor

		public CLoginTask () : base ()
		{
			this.taskName = "LoginScene";
			this.nextTask = "LobbyScene";
#if UNITY_EDITOR 
//			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString (CTaskUtil.USER_NAME, "user0001");
			PlayerPrefs.SetString (CTaskUtil.USER_PASSWORD, "123456789");
#endif
		}

		#endregion

		#region Implementation Task

		protected override void RegisterEvents() {
			base.RegisterEvents ();
			// INIT CLIENT WHEN LOGIN COMPLETED
			this.m_ClientEvents.Add ("clientInit", this.OnClientInitAccount);
		}

		// START LOGIN AND INIT ACCOUNT.
		public override void StartTask ()
		{
			base.StartTask ();
			var userName = PlayerPrefs.GetString (CTaskUtil.USER_NAME, string.Empty);
			var userPassword = PlayerPrefs.GetString (CTaskUtil.USER_PASSWORD, string.Empty);
			if (string.IsNullOrEmpty (userName) == false 
				&& string.IsNullOrEmpty (userPassword) == false) {
				var currentUser = this.m_UserManager.currentUser;
				currentUser.userName = userName;
				currentUser.userPassword = userPassword;
				this.m_UserManager.LoginUser ();
			} 
		}

		public override void EndTask ()
		{
			base.EndTask ();
			if (this.m_IsCompleteTask) {
				this.OnUserAlready ();
			}
		}

		#endregion

		#region ACCOUNT

		// SAVE ACCOUNT INFO FOR NEXT LOGIN
		protected virtual void OnUserAlready() {
			var currentUser = this.m_UserManager.currentUser;
			PlayerPrefs.SetString (CTaskUtil.USER_NAME, currentUser.userName);
			PlayerPrefs.SetString (CTaskUtil.USER_PASSWORD, currentUser.userPassword);
			PlayerPrefs.Save ();
			Debug.Log ("OnUserAlready");
		}

		// SEND TO SERVER USER LOGIN COMPLETED AND INIT ACCOUNT.
		public virtual void OnClientInitAccount(SocketIOEvent onClientInitMsg) {
			Debug.LogWarning ("clientInit " + onClientInitMsg.ToString());
			this.m_UserManager.Emit ("clientInitAccount", new JSONObject());
		}

		#endregion

	}
}
