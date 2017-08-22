using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUICustomManager : CMonoSingleton<CUICustomManager> {

		#region Fields

		[Header ("Loading")]
		[SerializeField]	protected GameObject m_LoadingGo;

		[Header ("Notice box")]
		[SerializeField]	protected GameObject m_NoticeGo;
		[SerializeField]	protected Text m_NoticeText;

		[Header ("Message box")]
		[SerializeField]	protected GameObject m_MessageGo;
		[SerializeField]	protected Text m_MessageTitle;
		[SerializeField]	protected Text m_MessageText;
		[SerializeField]	protected Button m_MessageSubmitButton;
		[SerializeField]	protected Button m_MessageCancelButton;

		#endregion

		#region Implementation MonoBehaviour

		protected override void Awake ()
		{
			base.Awake ();
			DontDestroyOnLoad (this.gameObject);
			this.m_LoadingGo.SetActive (false);
			this.m_NoticeGo.SetActive (false);
			this.m_MessageGo.SetActive (false);
		}

		#endregion

		#region Loading

		public virtual void ActiveLoading (bool value) {
			// LOADING GO
			this.m_LoadingGo.SetActive (value);
		}

		#endregion

		#region Notice

		public virtual void ActiveNotice(bool value, string text) {
			this.ActiveLoading (false);
			// NOTICE GO
			this.m_NoticeGo.SetActive (value);
			// NOICE TEXT
			this.m_NoticeText.text = text;
		}

		#endregion

		#region Message

		public virtual void ActiveMessage(bool value, string title, string text, Action submit, Action cancel) {
			this.ActiveLoading (false);
			this.ActiveNotice (false, string.Empty);
			// MESSAGE GO
			this.m_MessageGo.SetActive (value);
			// MESSGE TITLE
			this.m_MessageTitle.text = title;
			// MESSAGE TEXT
			this.m_MessageText.text = text;
			// SUBMIT BUTTON
			this.m_MessageSubmitButton.onClick.RemoveAllListeners ();
			this.m_MessageSubmitButton.onClick.AddListener (() => {
				this.m_MessageGo.SetActive (false);
				if (submit != null) {
					submit ();
				}
			});
			// CANCEL BUTTON
			this.m_MessageCancelButton.onClick.RemoveAllListeners ();
			this.m_MessageCancelButton.onClick.AddListener (() => {
				this.m_MessageGo.SetActive (false);
				if (cancel != null) {
					cancel ();
				}
			});
		}

		#endregion

	}

	public class CUICustom {
		public static void ActiveLoading(bool value) {
			CUICustomManager.Instance.ActiveLoading (value);
		}

		public static void ActiveNotice(bool value, string text)  {
			CUICustomManager.Instance.ActiveNotice (value, text);
		}

		public static void ActiveMessage(bool value, string title, string text, Action submit, Action cancel) {
			CUICustomManager.Instance.ActiveMessage (value, title, text, submit, cancel);
		}

		public static void CloseMessage() {
			CUICustomManager.Instance.ActiveMessage (false, string.Empty, string.Empty, null, null);
		}
	}
}
