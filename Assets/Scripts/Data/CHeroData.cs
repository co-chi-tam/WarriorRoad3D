using System;
using System.Collections;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CHeroData : CObjectData {

		public int heroAttackPoint;
		public float heroAttackSpeed;
		public int heroDefendPoint;
		public int heroHealthPoint;

		public CHeroData (): base ()
		{
			this.heroAttackPoint = 0;
			this.heroAttackSpeed = 0f;
			this.heroDefendPoint = 0;
			this.heroHealthPoint = 0;
		}

	}
}
