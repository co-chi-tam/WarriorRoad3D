using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	public class CUIHeroItem : MonoBehaviour {

		public Image heroAvatarImage;

		public virtual void SetupHeroItem(string avatar) {
			var avatarSprite = Resources.Load<Sprite> ("Avatar/" + avatar);
			if (avatarSprite != null) {
				this.heroAvatarImage.sprite = avatarSprite;
			}
		}
		
	}
}
