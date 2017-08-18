using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarriorRoad {
	public class CCreateHeroTask : CSimpleClientTask {

		#region Properties

		protected CUICreateHeroManager m_CreateHeroManager;

		#endregion

		#region Constructor

		public CCreateHeroTask () : base ()
		{
			this.taskName = "CreateHeroScene";
			this.nextTask = "LobbyScene";
		}

		#endregion

		#region Implementation Task

		protected override void RegisterEvents() {
			base.RegisterEvents ();
		}

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_CreateHeroManager = CUICreateHeroManager.GetInstance ();
			this.m_CreateHeroManager.OnEventSubmitHero -= this.OnClientSelectedHero;
			this.m_CreateHeroManager.OnEventSubmitHero += this.OnClientSelectedHero;
			this.m_CreateHeroManager.SetupHeroItem ();
		}

		public virtual void OnClientSelectedHero(int index, string name) {
			var heroesTemplate = CTaskUtil.Get (CTaskUtil.HERO_TEMPLATES) as Dictionary<string, CCharacterData>;
			var heroType = heroesTemplate.Keys.ToList () [index];
			this.OnClientCreateHero (heroType, name);
		}

		#endregion

		#region Hero

		public virtual void OnClientCreateHero(string heroType, string heroName) {
			if (this.m_UserManager.IsConnected() == false)
				return;
			var heroSubmitData = new Dictionary<string, string> ();
			var currentUser = CUserManager.Instance.currentUser;
			heroSubmitData.Add ("htype", heroType);
			heroSubmitData.Add ("hname", heroName);
			heroSubmitData.Add ("uname", currentUser.userName);
			heroSubmitData.Add ("token", currentUser.token);
			var jsonCreateHero = JSONObject.Create (heroSubmitData);
			this.m_UserManager.Emit ("clientCreateHero", jsonCreateHero);
		}

		#endregion

	}
}
