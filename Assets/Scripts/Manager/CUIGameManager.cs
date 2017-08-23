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
		[SerializeField]	protected Text m_CurrentEnergyText;
		[SerializeField]	protected Text m_CurrentGold;

		[Header ("Animator")]
		[SerializeField]	protected Animator m_Animator;
		[SerializeField]	protected Animator m_ResultAnimator;

		protected CUICharacterInfo m_CurrentCharacterInfo;

		#endregion

		#region Implementation Monobehaviour

		protected override void Awake ()
		{
			base.Awake ();
		}

		protected virtual void Start() {
			this.m_CharacterInfoPrefab.gameObject.SetActive (false);
		}

		#endregion

		#region Main methods

		public virtual void OnBackTaskPressed () {
			CGameManager.Instance.OnPlayerBackTask ();
		}

		#endregion

		#region WINNING && CLOSING

		public virtual void PlayWinningAnimation() {
			this.m_ResultAnimator.SetTrigger ("IsWinning");
		}

		public virtual void PlayClosingAnimation() {
			this.m_ResultAnimator.SetTrigger ("IsClosing");
		}

		#endregion

		#region Roll dice
	
		public virtual void OnStartRoll() {
			this.m_Animator.SetBool ("IsActiveDice", true);
		}

		public virtual void OnRollPressed() {
			CGameManager.Instance.OnPlayerRollDice ();
			this.m_Animator.SetBool ("IsActiveDice", false);
		}

		#endregion

		#region Character Info

		public virtual void OnLoadCharacterInfo(CCharacterController parent, bool isEnemy) {
			this.m_CurrentCharacterInfo = GameObject.Instantiate (this.m_CharacterInfoPrefab);
			this.m_CurrentCharacterInfo.SetupInfo (parent, isEnemy);
			this.m_CurrentCharacterInfo.transform.SetParent (this.m_CharacterInfoRoot.transform);
			this.m_CurrentCharacterInfo.gameObject.SetActive (true);
		}

		public virtual void OnUpdateCurrentEnergy (int curEnergy, int maxEnergy) {
			this.m_CurrentEnergyText.text = curEnergy + "/" + maxEnergy;
		}

		public virtual void OnUpdateCurrentGold (int gold) {
			this.m_CurrentGold.text = gold.ToString ();
		}

		#endregion
		
	}
}
