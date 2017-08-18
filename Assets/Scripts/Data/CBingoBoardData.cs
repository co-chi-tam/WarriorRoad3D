using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	[Serializable]
	public class CBingoBoardData {

		public int[] bingoBoard;
		public int[] roomFakeResult;
		public float resultPerSecond;
		public int awardPerBoard;

		public CBingoBoardData ()
		{
			this.resultPerSecond 	= 15f;
			this.awardPerBoard 		= 150;
		}
		
	}
}
