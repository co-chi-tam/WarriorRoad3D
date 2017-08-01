using System;
using System.Collections;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CObjectData {

		public string objectName;
		public string objectModel;
		public string objectAvatar;

		public CObjectData ()
		{
			this.objectName 	= string.Empty;
			this.objectModel 	= string.Empty;
			this.objectAvatar 	= string.Empty;
		}
		
	}
}
