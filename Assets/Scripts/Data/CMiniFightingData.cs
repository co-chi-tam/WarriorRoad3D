using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	[Serializable]
	public class CMiniFightingData {

		public string isoTime;
		public CHeroData playerData;
		public CHeroData enemyData;
		public int randomSeed;

		public CMiniFightingData ()
		{
			this.isoTime	= string.Empty;
			this.playerData = null;
			this.enemyData 	= null;
			this.randomSeed = 999;
		}
		
	}


}
