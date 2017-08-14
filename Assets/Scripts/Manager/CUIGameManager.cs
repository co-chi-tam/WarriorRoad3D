using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUIGameManager : CMonoSingleton<CUIGameManager> {

		#region Properties

		[Header("Character Info")]
		[SerializeField]	protected CUICharacterInfo m_CharacterInfoPrefab;
		[SerializeField]	protected GameObject m_CharacterInfoRoot;

		[Header("Chat panel")]
		[SerializeField]	protected GameObject m_ChatNotice;
		[SerializeField]	protected Text m_NoticeText;
		[SerializeField]	protected CChatItem[] m_ChatItems;

		protected string m_CurrentChat = string.Empty;
		protected List<string> m_CurrentChatList;

		#endregion

		#region Implementation Monobehaviour

		protected override void Awake ()
		{
			base.Awake ();
			this.m_CurrentChatList = new List<string> ();
		}

		protected virtual void Start() {
			this.m_CharacterInfoPrefab.gameObject.SetActive (false);
			for (int i = 0; i < this.m_ChatItems.Length; i++) {
				this.m_ChatItems [i].gameObject.SetActive (false);
			}
		}

		#endregion

		#region Roll dice
	
		public virtual void OnStartRoll() {
			// TODO
		}

		public virtual void OnRollPressed() {
			CGameManager.Instance.OnPlayerRollDice ();
		}

		#endregion

		#region Character Info

		public virtual void OnLoadCharacterInfo(CCharacterController parent, bool isEnemy) {
			var charInfo = GameObject.Instantiate (this.m_CharacterInfoPrefab);
			charInfo.SetupInfo (parent, isEnemy);
			charInfo.transform.SetParent (this.m_CharacterInfoRoot.transform);
			charInfo.gameObject.SetActive (true);
		}

		#endregion

		#region Chat

		public virtual void ReceiveChatText (string text) {
			this.m_CurrentChatList.Add (text);
			var min = this.m_CurrentChatList.Count - 20 < 0 ? 0 : this.m_CurrentChatList.Count - 20;
			var max = this.m_CurrentChatList.Count;
			for (int i = 0; i < this.m_ChatItems.Length; i++) {
				var index = i + min;
				this.m_ChatItems [i].SetChatText (this.m_CurrentChatList[index]);
				this.m_ChatItems [i].gameObject.SetActive (true);
			}
			// NOTICE SHOW
			this.m_ChatNotice.gameObject.SetActive (true);
			this.m_NoticeText.text = this.m_CurrentChatList.Count.ToString();
		}

		public virtual void ChatText (InputField input) {
			this.m_CurrentChat = input.text;
			input.text = string.Empty;
		}

		public virtual void SubmitChat() {
			if (string.IsNullOrEmpty (this.m_CurrentChat) == false) {
				CUserManager.Instance.OnClientSendChat (this.m_CurrentChat);
			}
			this.m_CurrentChat = string.Empty;
		}

		#endregion
		
	}
}
