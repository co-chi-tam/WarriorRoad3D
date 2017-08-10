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

		public void SetupItem(string skillName, string skillAvatar, Action buttonSubmit) {
			this.skillNameText.text = skillName;
			var avatarSprite = Resources.Load<Sprite> ("Avatar/" + skillAvatar);
			if (avatarSprite != null) {
				this.skillAvatarImage.sprite = avatarSprite;
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
