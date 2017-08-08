using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	public class CUICharacterInfo : MonoBehaviour {

		[Header ("Control")]
		public Text characterNameText;
		public Text characterLevelText;
		public Text characterUpdateText;

		[Header ("Animator")]
		public Animator characterAnimator;

		[Header("Parent")]
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

		public virtual void SetupInfo(string name, string level, CCharacterController parent, bool isEnemy) {
			this.characterNameText.text = name;
			this.characterNameText.color = isEnemy ? Color.red : Color.green;
			this.characterLevelText.text = "lv: " + level;
			this.characterParent = parent.gameObject;
			parent.AddAction ("UpdateHealth", this.OnUpdateHealth);
		}

		protected virtual void OnUpdateHealth(object[] prams) {
			var health = (int)prams [0];
			this.characterUpdateText.text = "" + health;
			this.characterAnimator.SetTrigger ("IsUpdate");
		}
		
	}
}
