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
			PlayerPrefs.SetString (CTaskUtil.USER_NAME, "user0007");
			PlayerPrefs.SetString (CTaskUtil.USER_PASSWORD, "123456789");
#endif
		}

		#endregion

		#region Implementation Task

		// START LOGIN AND INIT ACCOUNT.
		public override void StartTask ()
		{
			base.StartTask ();
			var userName 		= PlayerPrefs.GetString (CTaskUtil.USER_NAME, string.Empty);
			var userPassword 	= PlayerPrefs.GetString (CTaskUtil.USER_PASSWORD, string.Empty);
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
			this.OnUserAlready ();
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

		#endregion

	}
}
