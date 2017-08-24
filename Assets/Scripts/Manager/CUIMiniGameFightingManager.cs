using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUIMiniGameFightingManager : CMonoSingleton<CUIMiniGameFightingManager> {

		#region Fields

		[Header ("Result Animation")]
		[SerializeField]	protected Animator m_ResultAnimator;
		[Header ("Winning Box")]
		[SerializeField]	protected Text m_WinningTitleText;
		[SerializeField]	protected Text m_WinningBoxText;
		[Header ("Closing Box")]
		[SerializeField]	protected Text m_ClosingTitleText;
		[SerializeField]	protected Text m_ClosingBoxText;

		protected CMiniGameFightingTask m_MiniGameFightingTask;

		#endregion

		#region Implementation Monobehaviour

		protected virtual void Start() {
			this.m_MiniGameFightingTask = CRootTask.Instance.GetCurrentTask () as CMiniGameFightingTask;
		}

		#endregion

		#region Result box

		public virtual void SetUpWinningBox(string title, string text) {
			this.m_ResultAnimator.SetTrigger ("IsWinning");
			this.m_WinningTitleText.text = title;
			this.m_WinningBoxText.text = text;
		}

		public virtual void SetUpClosingBox(string title, string text) {
			this.m_ResultAnimator.SetTrigger ("IsClosing");
			this.m_ClosingTitleText.text = title;
			this.m_ClosingBoxText.text = text;
		}

		#endregion 

		#region Main methods

		public virtual void OnBackTaskPressed() {
			// WARNING
			CUserManager.Instance.OnClientInitAccount ();
			CUICustomManager.Instance.ActiveLoading (true);
		}

		#endregion

	}
}
