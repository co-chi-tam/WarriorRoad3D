using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	public class CUICharacterInfo : MonoBehaviour {

		#region Properties

		[Header ("Control")]
		public Text characterNameText;
		public Text characterLevelText;
		public Text characterUpdateText;
		public Image characterHealthBarImage;

		[Header ("Animator")]
		public Animator characterAnimator;

		[Header("Parent")]
		public CCharacterController characterParent;

		[Header ("Objectives")]
		[SerializeField]	protected GameObject[] m_Objectives;

		protected Transform m_Transform;
		protected bool m_Except = false;

		#endregion

		#region Implementation MonoBehaviour

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

		#endregion

		#region Main methods

		public virtual void SetupInfo(CCharacterController parent, bool isEnemy) {
			var data = parent.GetData() as CCharacterData;
			this.characterNameText.text = data.objectName;
			this.characterNameText.color = isEnemy ? Color.red : Color.green;
			this.characterLevelText.text = "lv: " + data.characterLevel;
			var fitHealth 				= (float) data.characterHealthPoint / data.characterMaxHealthPoint;
			this.characterHealthBarImage.fillAmount = fitHealth;
			this.characterParent = parent;
			this.m_Except = !isEnemy;
			parent.AddAction ("UpdateHealth", this.OnUpdateHealth);
		}

		protected virtual void OnUpdateHealth(object[] prams) {
			var updateHealth 	= (int)prams [0];
			var currentHealth 	= (int)prams[1];
			var maxHealth 		= (int)prams[2];
			var changeHealth 	= updateHealth - currentHealth;
			var fitHealth 		= (float) updateHealth / maxHealth;
			this.characterHealthBarImage.fillAmount = fitHealth;
			if (updateHealth == 0)
				return;
			this.characterUpdateText.text = "" + changeHealth;
			this.characterAnimator.SetTrigger ("IsUpdate");
		}

		public virtual void SetActive(bool value) {
			if (this.m_Except)
				return;
			for (int i = 0; i < this.m_Objectives.Length; i++) {
				this.m_Objectives[i].SetActive (value);
			}
		}

		#endregion

	}
}
