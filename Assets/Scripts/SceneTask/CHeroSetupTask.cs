using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CHeroSetupTask : CSimpleTask {

		#region Properties

		protected CUIHeroSetupManager m_HeroSetupManager;

		#endregion

		#region Constructor

		public CHeroSetupTask () : base ()
		{
			this.taskName = "HeroSetupScene";
			this.nextTask = "PlayScene";
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			var skillList = CTaskUtil.Get (CTaskUtil.SKILL_DATA_LIST) as List<CSkillData>;
			this.m_HeroSetupManager = CUIHeroSetupManager.GetInstance ();
			this.m_HeroSetupManager.OnSetupHeroSkill ("Normal Attack", 3, skillList, null);
			this.m_HeroSetupManager.OnHeroSetupSubmit -= OnHeroSetupSkillCompleted;
			this.m_HeroSetupManager.OnHeroSetupSubmit += OnHeroSetupSkillCompleted;
		}

		#endregion

		#region Main methods

		protected virtual void OnHeroSetupSkillCompleted(List<CSkillData> skillDatas) {
			var heroData = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CHeroData;
			heroData.characterSkillSlots = new CSkillData[skillDatas.Count];
			for (int i = 0; i < skillDatas.Count; i++) {
				heroData.characterSkillSlots [i] = skillDatas [i];
			}
			this.OnTaskCompleted ();
		}

		#endregion

	}
}
