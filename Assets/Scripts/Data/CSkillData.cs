using System;
using System.Collections;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CSkillData : CObjectData {

		public float skillDelay;		// DELAY TIME
		public float skillTime;			// TIME AFFECT
		public CSkillEffect[] skillTriggers;

		public CSkillData (): base ()
		{
			this.skillDelay 		= 0f;
			this.skillTime 			= 0f;
		}

	}

	[Serializable]
	public class CSkillEffect {
		public object skillValue;
		public string skillMethod;

		public CSkillEffect ()
		{
			this.skillValue 	= null;
			this.skillMethod 	= string.Empty;
		}

	}

}
