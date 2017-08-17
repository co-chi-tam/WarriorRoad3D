using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	[Serializable]
	public class CBingoRoomData {
		[Serializable]
		public class CBingoRoomMemberData
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
		public CBingoRoomMemberData[] playerInRoom;

		public CBingoRoomData ()
		{
			this.roomId 		= string.Empty;
			this.roomName 		= string.Empty;
			this.maxPlayer 		= 4;
			this.playerInRoom 	= new CBingoRoomMemberData[0];
		}
		
	}


}
