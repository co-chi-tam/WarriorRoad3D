using System;
using System.Collections;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CCharacterData : CObjectData {

		public int characterAttackPoint;
		public int maxAttackPoint;

		public float characterAttackSpeed;
		public float maxAttackSpeed;

		public int characterDefendPoint;
		public int maxDefendPoint;

		public int characterHealthPoint;
		public int characterMaxHealthPoint;
		public int maxHealthPoint;

		public int characterLevel;
		public int maxLevel;

		public int characterStep;

		public CSkillData[] characterSkillSlots;
		public CCharacterData dataPerLevel;

		public CCharacterData (): base ()
		{
			this.characterAttackPoint = 0;
			this.maxAttackPoint = 500;
			this.characterAttackSpeed = 0f;
			this.maxAttackSpeed = 2f;
			this.characterDefendPoint = 0;
			this.maxDefendPoint = 500;
			this.characterHealthPoint = 0;
			this.characterMaxHealthPoint = 0;
			this.maxHealthPoint = 9999;
			this.characterLevel = 1;
			this.maxLevel = 99;
			this.characterStep = 0;
		}

	}
}
