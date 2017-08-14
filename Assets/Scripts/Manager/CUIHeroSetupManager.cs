using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUIHeroSetupManager : CMonoSingleton<CUIHeroSetupManager> {

		#region Properties

		[Header ("Skill Info")]
		[SerializeField]	protected CUISkillItemInfo m_SkillInfoPrefab;
		[SerializeField]	protected GameObject m_SkillInfoRoot;
		[Header ("Skill Selected")]
		[SerializeField]	protected List<CSkillData> m_SkillSelected;

		public Action<List<CSkillData>> OnHeroSetupSubmit;

		#endregion

		#region Implementation MonoBehaviour

		protected override void Awake() {
			base.Awake ();
			this.m_SkillSelected 	= new List<CSkillData> ();
		}

		#endregion

		#region Main methods

		public virtual void OnSetupHeroSkill(string defaultItem, int maxSelectItem, List<CSkillData> skillList, Action<List<CSkillData>> onSelectSkills) {
			for (int i = 0; i < skillList.Count; i++) {
				var skillData = skillList [i];
				if (skillData.objectName == defaultItem) {
					this.m_SkillSelected.Add (skillData);
					continue;
				}
				skillData.uID = Guid.NewGuid ().ToString ();
				var skillItem = GameObject.Instantiate (this.m_SkillInfoPrefab);
				skillItem.transform.SetParent (this.m_SkillInfoRoot.transform);
				skillItem.gameObject.SetActive (true);
				skillItem.SetupItem (skillData.objectName, skillData.objectAvatar, () => {
					if (skillItem.selected == false
						&& this.m_SkillSelected.Count >= maxSelectItem + (string.IsNullOrEmpty(defaultItem) ? 0 : 1)) {
						return;
					} 
					var tmpData = skillData;
					skillItem.selected = !skillItem.selected;
					skillItem.skillSelected.gameObject.SetActive (skillItem.selected);
					if (skillItem.selected) {
						this.m_SkillSelected.Add (tmpData);
					} else {
						this.m_SkillSelected.Remove (tmpData);
					}
					if (onSelectSkills != null) {
						onSelectSkills (this.m_SkillSelected);
					}
				});
			}
			this.m_SkillInfoPrefab.gameObject.SetActive (false);
		}


		public virtual void OnSubmitPressed() {
			if (this.OnHeroSetupSubmit != null) {
				this.OnHeroSetupSubmit (this.m_SkillSelected);
			}
		}

		#endregion

	}
}
