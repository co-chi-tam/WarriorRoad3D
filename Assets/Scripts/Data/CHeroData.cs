using System;
using System.Collections;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CHeroData : CCharacterData {

		public int currentGold;
		public int maxGold;
		public int goldPerStep;
		public int currentEnergy;
		public int maxEnergy;
		public int currentGlory;
		public int maxGlory;

		public CHeroData ()
		{
			this.currentGold	= 0;
			this.maxGold 		= 999999999;
			this.goldPerStep	= 50;
			this.currentEnergy 	= 0;
			this.maxEnergy 		= 30;
			this.currentGlory	= 0;
			this.maxGlory 		= 999999999;
		}
		
	}
}
