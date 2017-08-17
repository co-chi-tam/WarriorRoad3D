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

		[Header ("Mini game Bingo")]
		[SerializeField] 	protected CUIBingoRoom[] m_BingoRooms;

		public Action<List<CSkillData>> OnHeroSetupSubmit;

		protected Sprite[] m_AvatarSprites;

		#endregion

		#region Implementation MonoBehaviour

		protected override void Awake() {
			base.Awake ();
			this.m_SkillSelected 	= new List<CSkillData> ();
			this.m_AvatarSprites 	= Resources.LoadAll <Sprite> ("Avatar");
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
			CUserManager.Instance.OnClientGetBingoRoomList ();
		}

		public virtual void OnBingoLeaveRoomPressed () {
			CUserManager.Instance.OnClientRequestLeaveBingoRoom ();
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

	}
}
