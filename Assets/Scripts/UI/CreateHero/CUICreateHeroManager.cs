using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUICreateHeroManager : CMonoSingleton<CUICreateHeroManager> {

		[Header("Create Hero Item")]
		[SerializeField]	protected GameObject m_HeroItemRoot;
		[SerializeField]	protected CUIScrollRectSnapPage m_HeroPage;
		[SerializeField]	protected CUIHeroItem m_HeroItemPrefab;
		[SerializeField]	protected InputField m_HeroNameInputField;
		[SerializeField]	protected Button m_HeroSubmitButton;

		protected Dictionary<string, CHeroData> m_HeroTemplates;

		public Action<int, string> OnEventSubmitHero;

		public virtual void SetupHeroItem() {
			this.m_HeroTemplates = CTaskUtil.Get (CTaskUtil.HERO_TEMPLATES) as Dictionary<string, CHeroData>;
			this.m_HeroPage.gameObject.SetActive (false);
			var guild = Guid.NewGuid ().ToString ();
			this.m_HeroNameInputField.text = "Player" + guild.Substring(guild.Length - 5)	;
			if (this.m_HeroTemplates.Count > 0) {
				foreach (var item in this.m_HeroTemplates) {
					var itemInstant = Instantiate (this.m_HeroItemPrefab);
					var itemData = item.Value;
					itemInstant.SetupHeroItem (itemData.objectAvatar);
					itemInstant.gameObject.SetActive (true);
					itemInstant.transform.SetParent (this.m_HeroItemRoot.transform);
				}
			}
			this.m_HeroPage.gameObject.SetActive (true);
			this.m_HeroItemPrefab.gameObject.SetActive (false);
			this.m_HeroSubmitButton.onClick.RemoveAllListeners ();
			this.m_HeroSubmitButton.onClick.AddListener (() => {
				var heroIndex = this.m_HeroPage.CurrentPage;
				var heroName = this.m_HeroNameInputField.text;
				if (this.OnEventSubmitHero != null) {
					this.OnEventSubmitHero (heroIndex, heroName);
					CUICustomManager.Instance.ActiveLoading (true);
				}
			});
		}
		
	}
}
