using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	[Serializable]
	public class CRoomData {
		[Serializable]
		public class CRoomMemberData
		{
			public string userId;
			public string playerId;
			public string objectName;
			public string objectModel;
			public string objectAvatar;
			public int currentGold;
		}

		public string roomId;
		public string roomName;
		public int maxPlayer;
		public CRoomMemberData[] playerInRoom;
		public string eventResponseCode;

		public CRoomData ()
		{
			this.roomId 		= string.Empty;
			this.roomName 		= string.Empty;
			this.maxPlayer 		= 4;
			this.playerInRoom 	= new CRoomMemberData[0];
			this.eventResponseCode = "9a05";
		}
		
	}


}
