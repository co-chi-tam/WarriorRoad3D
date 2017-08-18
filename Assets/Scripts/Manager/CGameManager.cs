using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSingleton;
using FSM;

namespace WarriorRoad {
	public class CGameManager : CMonoSingleton<CGameManager>, IGameSimpleContext {

		#region Fields

		[Header("Hero")]
		[SerializeField]	protected CHeroController m_CharacterController;
		[SerializeField]	protected Camera m_MainCamera;

		[Header ("FSM")]
		[SerializeField]	protected TextAsset m_FSMText;
		[SerializeField]	protected string m_FSMState;

		protected CMapManager m_MapManager;
		protected bool m_LoadingCompleted;
		protected int m_LevelPerBlock = 7;
		protected FSMManager m_FSMManager;
		protected CPlayTask m_PlayTask;

		public Action OnEventLoadingCompleted;

		#endregion

		#region Implementation MonoBehaviour

		protected override void Awake() {
			base.Awake ();
			this.m_FSMManager = new FSMManager ();
			this.m_FSMManager.LoadFSM (this.m_FSMText.text);
			this.m_FSMManager.RegisterState ("GameIdleState", 			new FSMGameIdleState (this));
			this.m_FSMManager.RegisterState ("GameActiveState", 		new FSMGameActiveState (this));
			this.m_FSMManager.RegisterState ("GameCompletedState", 		new FSMGameCompletedState (this));
			this.m_FSMManager.RegisterState ("GameEndState", 			new FSMGameEndState (this));

			this.m_FSMManager.RegisterCondition ("IsLoadingCompleted", 	this.IsLoadingCompleted);
			this.m_FSMManager.RegisterCondition ("IsPlayerDeath", 		this.IsPlayerDeath);
			this.m_FSMManager.RegisterCondition ("IsRoundCompleted", 	this.IsRoundCompleted);
		}

		protected virtual void Start() {
			this.m_PlayTask = CRootTask.Instance.GetCurrentTask () as CPlayTask;
		}

		protected virtual void Update() {
			this.m_FSMManager.UpdateState (Time.deltaTime);
			this.m_FSMState = this.m_FSMManager.currentStateName;
			if (this.m_LoadingCompleted == false)
				return;
			this.m_MainCamera.transform.LookAt (this.m_CharacterController.GetPosition ());
		}

		#endregion

		#region Main methods

		public virtual void OnStartGame() {
			this.m_MapManager = CMapManager.GetInstance ();
			this.m_MapManager.OnMapGenerateComplete -= this.SpawnCharacter;
			this.m_MapManager.OnMapGenerateComplete += this.SpawnCharacter;
		}

		public virtual void OnUpdateGame() {
		
		}

		public virtual void OnCompleteGame() {
			this.m_PlayTask.OnClientCompletedMap ();
		}

		public virtual void OnEndGame() {
			this.m_PlayTask.OnClientEndGame ();
		}

		public virtual void OnLoadingCompleted() {
			this.m_PlayTask.OnClientInitMap ();
		}

		public virtual void SpawnCharacter() {
			StartCoroutine (this.HandleSpawnCharacter ());
		}

		protected virtual IEnumerator HandleSpawnCharacter() {
			var heroData = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CHeroData;
			this.m_CharacterController = Instantiate (Resources.Load<CHeroController>("CharacterPrefabs/" + heroData.objectModel));
			yield return this.m_CharacterController;
			// Init Events
			this.m_CharacterController.AddAction ("StartIdleState", this.OnPlayerStartRollDice);
			this.m_CharacterController.AddAction ("MovedBlockState", this.OnPlayerMovedToBlock);
			// INIT DATA
			this.m_CharacterController.SetData (heroData);
			this.m_CharacterController.SetActive (true);
			this.m_CharacterController.Init ();
			// SET CURRENT BLOCK
			var currentBlock = this.m_MapManager.CalculateCurrentBlock (this.m_CharacterController.GetStep());
			this.m_CharacterController.currentBlock = currentBlock;
			this.m_CharacterController.targetBlock = currentBlock;
			this.m_CharacterController.SetPosition (currentBlock.GetMovePointPosition());
			this.m_MapManager.OnMapGenerateComplete -= this.SpawnCharacter;
			// LOADING COMPLETED
			this.m_LoadingCompleted = true;
			if (this.OnEventLoadingCompleted != null) {
				this.OnEventLoadingCompleted ();
			}
			CUIGameManager.Instance.OnLoadCharacterInfo (this.m_CharacterController, false);
			CUIGameManager.Instance.OnUpdateCurrentEnergy (heroData.currentEnergy, heroData.maxEnergy);
			CUIGameManager.Instance.OnUpdateCurrentGold (heroData.currentGold);
		}

		public virtual void OnPlayerStartRollDice() {
			Debug.LogWarning ("OnPlayerStartRollDice ");
			CUIGameManager.Instance.OnStartRoll ();
			if (this.m_CharacterController.GetActive ()) {
				this.m_PlayTask.OnClientUpdateHero (this.m_CharacterController.GetData () as CCharacterData);
			}
		}

		public virtual void OnPlayerMovedToBlock() {
			// DATA
			var heroData = this.m_CharacterController.GetData () as CHeroData;
			// GOLD PER STEP
			var goldPerStep = heroData.goldPerStep * heroData.characterLevel;
			// TOTAL GOLD
			var totalGold = heroData.currentGold + goldPerStep;
			// UPDATE
			heroData.currentGold = totalGold >= heroData.maxGold ? heroData.maxGold : totalGold;
			CUIGameManager.Instance.OnUpdateCurrentGold (heroData.currentGold);
		}

		public virtual void OnPlayerRollDice() {
			if (this.m_CharacterController.HaveEnemy ())
				return;
			if (this.IsRoundCompleted () == true)
				return;
			Debug.LogWarning ("OnPlayerRollDice ");
			this.m_PlayTask.OnClientRollDice ();
		}

		public virtual void OnPlayerUpdateStep (int value, int curEnergy, int maxEnergy) {
			var currentStep = this.m_CharacterController.GetStep ();
			var randomBlock = this.m_MapManager.CalculateCurrentBlock (currentStep + value);
			this.m_CharacterController.targetBlock = randomBlock;
			CUIGameManager.Instance.OnUpdateCurrentEnergy (curEnergy, maxEnergy);
		}

		public virtual void OnPlayerBackTask() {
			// WARNING
			CUserManager.Instance.OnClientInitAccount ();
			CUICustomManager.Instance.ActiveLoading (true);
		}

		#endregion

		#region IGameSimpleContext implementation

		public bool IsLoadingCompleted ()
		{
			return this.m_LoadingCompleted;
		}
		public bool IsPlayerDeath ()
		{
			return this.m_CharacterController.GetActive () == false;
		}
		public bool IsRoundCompleted ()
		{
			return this.m_CharacterController.GetStep() >= this.m_MapManager.GetBlockCount();
		}

		#endregion
	}
}
