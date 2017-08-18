using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUILobbyManager : CMonoSingleton<CUILobbyManager> {

		#region Properties

		[Header ("Skill Info")]
		[SerializeField]	protected CUISkillItemInfo m_SkillInfoPrefab;
		[SerializeField]	protected GameObject m_SkillInfoRoot;
		[SerializeField]	protected List<CSkillData> m_SkillSelected;

		[Header("Chat panel")]
		[SerializeField]	protected GameObject m_ChatNotice;
		[SerializeField]	protected Text m_NoticeText;
		[SerializeField]	protected CChatItem[] m_ChatItems;

		[Header ("Mini game Bingo")]
		[SerializeField] 	protected CUIBingoRoom[] m_BingoRooms;

		public Action<List<CSkillData>> OnHeroSetupSubmit;

		protected string m_CurrentChat = string.Empty;
		protected List<CChatData> m_CurrentChatList;
		protected Sprite[] m_AvatarSprites;
		protected CLobbyTask m_LobbyTask;

		#endregion

		#region Implementation MonoBehaviour

		protected override void Awake() {
			base.Awake ();
			this.m_SkillSelected 	= new List<CSkillData> ();
			this.m_AvatarSprites 	= Resources.LoadAll <Sprite> ("Avatar");
			this.m_CurrentChatList 	= new List<CChatData> ();
		}

		protected virtual void Start() {
			this.m_LobbyTask = CRootTask.Instance.GetCurrentTask () as CLobbyTask;
			for (int i = 0; i < this.m_ChatItems.Length; i++) {
				this.m_ChatItems [i].gameObject.SetActive (false);
			}
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
				Sprite avatarSprite = null;
				for (int x = 0; x < this.m_AvatarSprites.Length; x++) {
					if (this.m_AvatarSprites [x].name == skillData.objectAvatar) {
						avatarSprite = this.m_AvatarSprites [x];
					}
				}
				skillItem.SetupItem (skillData.objectName, avatarSprite, () => {
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

		public virtual void OnBingoButtonPressed() {
			this.m_LobbyTask.OnClientGetBingoRoomList ();
		}

		public virtual void OnBingoLeaveRoomPressed () {
			this.m_LobbyTask.OnClientRequestLeaveBingoRoom ();
		}

		public virtual void SetUpBingoRoom (List<CBingoRoomData> listRoom, Action<int, CBingoRoomData> roomSelected) {
			for (int i = 0; i < this.m_BingoRooms.Length; i++) {
				var roomData = this.m_BingoRooms[i].roomData;
				if (listRoom [i] == null) {
					// TODO
				} else {
					roomData = listRoom [i];
				}
				this.m_BingoRooms [i].SetupRoom (i, roomData, ((index) => {
					if (roomSelected != null) {
						roomSelected (index, roomData);
					}
				}));
			}
		}

		#endregion

		#region Chat

		public virtual void ReceiveChatText (CChatData chat) {
			this.m_CurrentChatList.Add (chat);
			var min = this.m_CurrentChatList.Count - 20 < 0 ? 0 : this.m_CurrentChatList.Count - 20;
			var max = this.m_CurrentChatList.Count;
			for (int i = 0; i < this.m_ChatItems.Length; i++) {
				var index = i + min;
				var chatStr = this.m_CurrentChatList[index].chatStr;
				var isMine = this.m_CurrentChatList [index].isMine;
				this.m_ChatItems [i].SetChatText (chatStr, isMine);
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
				this.m_LobbyTask.OnClientSendChat (this.m_CurrentChat);
			}
			this.m_CurrentChat = string.Empty;
		}

		#endregion

	}
}
