using System;
using System.Collections;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CSkillData : CObjectData {

		public string[] characterClasses;
		public int levelRequire;
		public float skillDelay;		// DELAY TIME
		public float skillTime;			// TIME AFFECT
		public CSkillEffect[] skillEffects;

		public CSkillData (): base ()
		{
			this.characterClasses   = new string[] {"Warrior","Archer","Wizard"};
			this.levelRequire		= 0;
			this.skillDelay 		= 0f;
			this.skillTime 			= 0f;
		}

	}

	public class CSkillEffect {
		public int skillValue;
		public string skillMethod;

		public CSkillEffect ()
		{
			this.skillValue 	= 0;
			this.skillMethod 	= string.Empty;
		}

	}

}
