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
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_UserManager = CUserManager.GetInstance ();
			var userName = PlayerPrefs.GetString (CTaskUtil.USER_NAME, string.Empty);
			var userPassword = PlayerPrefs.GetString (CTaskUtil.USER_PASSWORD, string.Empty);
			if (string.IsNullOrEmpty (userName) == false 
				&& string.IsNullOrEmpty (userPassword) == false) {
				var currentUser = this.m_UserManager.currentUser;
				currentUser.userName = userName;
				currentUser.userPassword = userPassword;
				this.m_UserManager.Login ();
			} 
			this.m_UserManager.OnConectServerCompleted -= OnUserAlready;
			this.m_UserManager.OnConectServerCompleted += OnUserAlready;
		}

		protected virtual void OnUserAlready() {
			this.OnTaskCompleted ();
			var currentUser = this.m_UserManager.currentUser;
			PlayerPrefs.SetString (CTaskUtil.USER_NAME, currentUser.userName);
			PlayerPrefs.SetString (CTaskUtil.USER_PASSWORD, currentUser.userPassword);
			Debug.Log ("OnUserAlready");
		}

		#endregion

	}
}
