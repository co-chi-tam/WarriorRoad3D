using System;
using System.Collections;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CSkillData : CObjectData {

		public float skillTime;
		public float skillEffectPerTime;
		public CSkillEffect skillEffects;

		public class CSkillEffect {
			public int skillValue;
			public string skillMethod;
		}

		public CSkillData (): base ()
		{
			this.skillTime = 0f;
			this.skillEffectPerTime = 0f;
		}

	}


}
