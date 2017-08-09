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
		public CCharacterController characterParent;

		protected Transform m_Transform;
		protected bool m_Except = false;

		protected virtual void Awake() {
			this.m_Transform = transform;
		}

		protected virtual void LateUpdate() {
			if (characterParent == null 
				&& characterParent.GetActive() == false)
				return;
			var screenPosition = Camera.main.WorldToScreenPoint (characterParent.transform.position);
			this.m_Transform.position = screenPosition;
			this.gameObject.SetActive (characterParent.gameObject.activeInHierarchy);
			this.SetActive (characterParent.HaveEnemy());
		}

		public virtual void SetupInfo(CCharacterController parent, bool isEnemy) {
			var data = parent.GetData() as CCharacterData;
			this.characterNameText.text = data.objectName;
			this.characterNameText.color = isEnemy ? Color.red : Color.green;
			this.characterLevelText.text = "lv: " + data.characterLevel;
			this.characterParent = parent;
			this.m_Except = !isEnemy;
			parent.AddAction ("UpdateHealth", this.OnUpdateHealth);
		}

		protected virtual void OnUpdateHealth(object[] prams) {
			var health = (int)prams [0];
			this.characterUpdateText.text = "" + health;
			this.characterAnimator.SetTrigger ("IsUpdate");
		}

		public virtual void SetActive(bool value) {
			if (this.m_Except)
				return;
			this.characterNameText.gameObject.SetActive (value);
			this.characterUpdateText.gameObject.SetActive (value);
			this.characterLevelText.gameObject.SetActive (value);
		}

	}
}
