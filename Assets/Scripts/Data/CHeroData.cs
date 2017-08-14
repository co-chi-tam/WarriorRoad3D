using System;
using System.Collections;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CHeroData : CCharacterData {

		public int currentGold;
		public int maxGold;
		public int currentEnergy;
		public int maxEnergy;

		public CHeroData ()
		{
			this.currentGold	= 0;
			this.maxGold 		= 999999999;
			this.currentEnergy 	= 0;
			this.maxEnergy 		= 30;
		}
		
	}
}
