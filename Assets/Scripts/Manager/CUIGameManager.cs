using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUIGameManager : CMonoSingleton<CUIGameManager> {

		[Header("Character Info")]
		[SerializeField]	protected CUICharacterInfo m_CharacterInfoPrefab;
		[SerializeField]	protected GameObject m_CharacterInfoRoot;

		protected virtual void Start() {
			this.m_CharacterInfoPrefab.gameObject.SetActive (false);
		}
	
		public virtual void OnStartRoll() {
			// TODO
		}

		public virtual void OnRollPressed() {
			CGameManager.Instance.OnPlayerRollDice ();
		}

		public virtual void OnLoadCharacterInfo(CCharacterController parent, bool isEnemy) {
			var charInfo = GameObject.Instantiate (this.m_CharacterInfoPrefab);
			charInfo.SetupInfo (parent, isEnemy);
			charInfo.transform.SetParent (this.m_CharacterInfoRoot.transform);
			charInfo.gameObject.SetActive (true);
		}
		
	}
}
