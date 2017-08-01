using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUICustomManager : CMonoSingleton<CUICustomManager> {

		[Header ("Loading")]
		[SerializeField]	protected GameObject m_LoadingGo;

		[Header ("Message box")]
		[SerializeField]	protected GameObject m_MessageBoxGo;
		[SerializeField]	protected Text m_MessageBoxText;

		protected override void Awake ()
		{
			base.Awake ();
			DontDestroyOnLoad (this.gameObject);
			this.m_LoadingGo.SetActive (false);
		}

		public virtual void ActiveLoading (bool value) {
			this.m_LoadingGo.SetActive (value);
		}

		public virtual void ActiveMessage(bool value, string text) {
			this.ActiveLoading (false);
			this.m_MessageBoxGo.SetActive (value);
			this.m_MessageBoxText.text = text;
		}

	}
}
