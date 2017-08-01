using System;
using System.Collections;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CUserData : CObjectData {

		public string userName;
		public string userPassword;
		public string userEmail;
		public string userDisplayName;
		public string token;

		public CUserData () : base()
		{
			this.userName 		= string.Empty;
			this.userPassword 	= string.Empty;
			this.userEmail 		= string.Empty;
			this.userDisplayName = string.Empty;
			this.token			= string.Empty;
		}
	
	}
}
