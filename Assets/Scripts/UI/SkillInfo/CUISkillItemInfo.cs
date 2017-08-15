using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	public class CUISkillItemInfo : MonoBehaviour {

		[Header ("Info")]
		public Text skillNameText;
		public Image skillAvatarImage;
		public Button skillSelectButton;
		public GameObject skillSelected;
		[Header ("Selected")]
		public bool selected;

		public void SetupItem(string skillName, Sprite skillAvatar, Action buttonSubmit) {
			this.skillNameText.text = skillName;
			if (skillAvatar != null) {
				this.skillAvatarImage.sprite = skillAvatar;
			}
			this.skillSelectButton.onClick.RemoveAllListeners ();
			this.skillSelectButton.onClick.AddListener (() => {
				if (buttonSubmit != null) {
					buttonSubmit ();
				}
			});
		}
		
	}
}
