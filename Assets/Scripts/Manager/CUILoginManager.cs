using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;
using SocketIO;

namespace WarriorRoad {
	public class CUILoginManager : CMonoSingleton<CUILoginManager> {

		protected CUserManager m_UserManager;

		protected virtual void Start() {
			this.m_UserManager = CUserManager.GetInstance ();
		}

		public virtual void OnRegisterPressed() {
			this.m_UserManager.RegisterUser ();
		}

		public virtual void OnLoginPressed() {
			this.m_UserManager.LoginUser ();
		}

		public virtual void SetUserNameInput(InputField value) {
			this.m_UserManager.InputUserName (value);
		}

		public virtual void SetUserPasswordInput(InputField value) {
			this.m_UserManager.InputUserPassword (value);
		}

		public virtual void SetUserEmailInput(InputField value) {
			this.m_UserManager.InputUserEmail (value);
		}

		public virtual void SetUserDisplayNameInput(InputField value) {
			this.m_UserManager.InputUserDisplayName (value);
		}

	}
}
