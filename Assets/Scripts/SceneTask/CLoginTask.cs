using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CLoginTask : CSimpleTask {

		#region Properties

		protected CUserManager m_UserManager;

		#endregion

		#region Constructor

		public CLoginTask () : base ()
		{
			this.taskName = "LoginScene";
			this.nextTask = "PlayScene";

//			PlayerPrefs.SetString (CTaskUtil.USER_NAME, "user0005");
//			PlayerPrefs.SetString (CTaskUtil.USER_PASSWORD, "123456789");
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_UserManager = CUserManager.GetInstance ();
			this.m_UserManager.OnEventInitUserCompleted -= OnUserAlready;
			this.m_UserManager.OnEventInitUserCompleted += OnUserAlready;

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
