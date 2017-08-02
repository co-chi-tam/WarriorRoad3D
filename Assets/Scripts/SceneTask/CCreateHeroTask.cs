using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarriorRoad {
	public class CCreateHeroTask : CSimpleTask {

		#region Properties

		protected CUICreateHeroManager m_CreateHeroManager;
		protected CUserManager m_UserManager;

		#endregion

		#region Constructor

		public CCreateHeroTask () : base ()
		{
			this.taskName = "CreateHeroScene";
			this.nextTask = "PlayScene";
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_CreateHeroManager = CUICreateHeroManager.GetInstance ();
			this.m_CreateHeroManager.OnEventSubmitHero -= this.OnClientSelectedHero;
			this.m_CreateHeroManager.OnEventSubmitHero += this.OnClientSelectedHero;
			this.m_CreateHeroManager.SetupHeroItem ();

			this.m_UserManager = CUserManager.GetInstance ();
		}

		public virtual void OnClientSelectedHero(int index, string name) {
			var heroesTemplate = CTaskUtil.Get (CTaskUtil.HERO_TEMPLATES) as Dictionary<string, CHeroData>;
			var heroType = heroesTemplate.Keys.ToList () [index];
			this.m_UserManager.OnClientCreateHero (heroType, name);
		}

		#endregion

	}
}
