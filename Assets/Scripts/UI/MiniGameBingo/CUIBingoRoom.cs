using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	public class CUIBingoRoom : MonoBehaviour {

		public Text roomNameText;
		public Text roomMemberText;
		public Button roomSelectButton;
		public CBingoRoomData roomData;
		[HideInInspector]
		public int index;
		public void SetupRoom(int i, CBingoRoomData data, System.Action<int> submit) {
			this.index = i;
			this.roomData = data;
			this.roomNameText.text = data.roomName;
			this.roomMemberText.text = data.playerInRoom.Length + "/" + data.maxPlayer;
			this.roomSelectButton.onClick.RemoveAllListeners ();
			this.roomSelectButton.onClick.AddListener (() => {
				if (submit != null) {
					submit (this.index);
				}
			});
		}
			
	}
}
