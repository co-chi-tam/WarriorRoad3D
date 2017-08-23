using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CMiniGameFightingManager : CMonoSingleton<CMiniGameFightingManager> {

		#region Fields

		[Header ("Player")]
		[SerializeField]	protected GameObject m_PlayerSpawnPoint;
		[SerializeField]	protected CHeroController m_PlayerController;
		[Header ("Enemy")]
		[SerializeField]	protected GameObject m_EnemySpawnPoint;
		[SerializeField]	protected CHeroController m_EnemyController;

		protected bool m_BattleEnd = false;
		protected CMiniGameFightingTask m_MiniGameTask;
		// EVENT
		public Action OnLoadMiniGameCompleted;

		#endregion

		#region Implementation Monobehaviour

		protected virtual void Start() {
			this.m_MiniGameTask = CRootTask.Instance.GetCurrentTask () as CMiniGameFightingTask;
		}

		#endregion

		#region Main methods

		// START LOAD CHARACTER
		public virtual void StartLoading() {
			var miniFightingData	= CTaskUtil.Get (CTaskUtil.MINI_FIGHTING_DATA) as CMiniFightingData;
			// LOAD PLAYER CONTROLLER
			StartCoroutine (this.HandleSpawnCharacter (miniFightingData.playerData, 
				this.m_PlayerSpawnPoint, (ctrl) => {
					this.m_PlayerController = ctrl;
				}));
			// LOAD ENEMY CONTROLLER
			StartCoroutine (this.HandleSpawnCharacter (miniFightingData.enemyData, 
				this.m_EnemySpawnPoint, (ctrl) => {
					this.m_EnemyController = ctrl;
				}));
			// LOAD SEED
			UnityEngine.Random.InitState (miniFightingData.randomSeed);
		}

		// SET UP ALL TARGET
		public virtual void SetupTargets() {
			// SET ENEMY
			this.m_PlayerController.SetTargetEnemy (this.m_EnemyController);
			this.m_EnemyController.SetTargetEnemy  (this.m_PlayerController);
			// ROTATION
			this.m_PlayerController.SetRotation (this.m_EnemyController.GetPosition ());
			this.m_EnemyController.SetRotation  (this.m_PlayerController.GetPosition ());
		}

		// HANDLE LOAD CHARACTER
		protected virtual IEnumerator HandleSpawnCharacter(CHeroData charData, GameObject spawnPoint, Action<CHeroController> completed) {
			var charCtrl = Instantiate (Resources.Load<CHeroController>("CharacterPrefabs/" + charData.objectModel));
			yield return charCtrl;
			// INIT DATA
			charCtrl.SetData (charData);
			charCtrl.SetActive (true);
			charCtrl.Init ();
			// EVENTS
			charCtrl.AddAction ("StartInactiveState", this.OnCharacterInactive);
			// SET CURRENT BLOCK
			charCtrl.SetPosition (spawnPoint.transform.position);
			// COMPLETED
			if (completed != null) {
				completed (charCtrl);
			}
			// EVENT
			if (this.m_PlayerController != null && this.m_EnemyController != null) {
				if (this.OnLoadMiniGameCompleted != null) {
					this.OnLoadMiniGameCompleted ();
				}
				this.m_BattleEnd = false;
			}
		}

		// ON CHARACTER DEATH OR INACTIVE
		protected virtual void OnCharacterInactive(object[] args) {
			var currentHero = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CHeroData;
			var closerCtrl = args [0] as CCharacterController;
			Debug.LogWarning ("OnCharacterInactive " + closerCtrl.name);
			this.PrintBattleLog (closerCtrl.GetData().uID);
		}

		// PRINT BATTLE LOG WARNING
		protected virtual void PrintBattleLog(string closerId) {
			if (this.m_BattleEnd == true)
				return;
			this.m_BattleEnd = true;
			var playerId 	= this.m_PlayerController.GetData ().uID;
			var enemyId 	= this.m_EnemyController.GetData ().uID;
			var winnerId 	= playerId != closerId ? playerId : enemyId;
			if (winnerId != closerId) {
				this.m_MiniGameTask.OnClientEndFighting (winnerId, closerId);
			}
		}

		#endregion
		
	}
}
