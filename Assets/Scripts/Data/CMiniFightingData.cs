using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	[Serializable]
	public class CMiniFightingData {
		
		public CHeroData playerData;
		public CHeroData enemyData;
		public int randomSeed;

		public CMiniFightingData ()
		{
			this.playerData = null;
			this.enemyData 	= null;
			this.randomSeed = 999;
		}
		
	}


}
