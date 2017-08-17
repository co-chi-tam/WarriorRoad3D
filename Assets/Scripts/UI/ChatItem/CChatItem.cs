using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	public class CChatItem : MonoBehaviour {

		public Text chatText;

		public virtual void SetChatText(string text, bool isMine) {
			this.chatText.text = text;
			this.chatText.alignment = isMine ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
		}
		
	}
}
