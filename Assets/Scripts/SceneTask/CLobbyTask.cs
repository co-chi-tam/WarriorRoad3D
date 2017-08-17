using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CLobbyTask : CSimpleTask {

		#region Properties

		protected CUILobbyManager m_LobbyManager;

		#endregion

		#region Constructor

		public CLobbyTask () : base ()
		{
			this.taskName = "LobbyScene";
			this.nextTask = "PlayScene";
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			var skillList = CTaskUtil.Get (CTaskUtil.SKILL_DATA_LIST) as List<CSkillData>;
			this.m_LobbyManager = CUILobbyManager.GetInstance ();
			this.m_LobbyManager.OnSetupHeroSkill ("Normal Attack", 3, skillList, null);
			this.m_LobbyManager.OnHeroSetupSubmit -= OnHeroSetupSkillCompleted;
			this.m_LobbyManager.OnHeroSetupSubmit += OnHeroSetupSkillCompleted;
		}

		#endregion

		#region Main methods

		protected virtual void OnHeroSetupSkillCompleted(List<CSkillData> skillDatas) {
			var heroData = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CHeroData;
			heroData.characterSkillSlots = new CSkillData[skillDatas.Count];
			for (int i = 0; i < skillDatas.Count; i++) {
				heroData.characterSkillSlots [i] = skillDatas [i];
			}
			CUserManager.Instance.OnClientSetupSkills (heroData.characterSkillSlots);
		}

		#endregion

	}
}
