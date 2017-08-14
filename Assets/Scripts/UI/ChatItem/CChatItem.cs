using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	public class CChatItem : MonoBehaviour {

		public Text chatText;

		public virtual void SetChatText(string text) {
			this.chatText.text = text;
		}
		
	}
}
