using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	public class CUICharacterInfo : MonoBehaviour {

		public Text characterNameText;
		public Text characterLevelText;
		public GameObject characterParent;

		protected Transform m_Transform;

		protected virtual void Awake() {
			this.m_Transform = transform;
		}

		protected virtual void LateUpdate() {
			if (characterParent == null 
				&& characterParent.activeInHierarchy == false)
				return;
			var screenPosition = Camera.main.WorldToScreenPoint (characterParent.transform.position);
			this.m_Transform.position = screenPosition;
			this.gameObject.SetActive (characterParent.activeInHierarchy);
		}

		public virtual void SetupInfo(string name, string level, GameObject parent, bool isEnemy) {
			this.characterNameText.text = name;
			this.characterNameText.color = isEnemy ? Color.red : Color.green;
			this.characterLevelText.text = "lv: " + level;
			this.characterParent = parent;
		}
		
	}
}
