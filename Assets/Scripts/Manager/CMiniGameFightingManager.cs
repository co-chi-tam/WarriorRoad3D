using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CMiniGameFightingManager : CMonoSingleton<CMiniGameFightingManager> {

		[Header ("Player")]
		[SerializeField]	protected GameObject m_PlayerSpawnPoint;
		[SerializeField]	protected CHeroController m_PlayerController;
		[Header ("Enemy")]
		[SerializeField]	protected GameObject m_EnemySpawnPoint;
		[SerializeField]	protected CHeroController m_EnemyController;

		public Action OnLoadMiniGameCompleted;

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

		public virtual void SetupTargets() {
			// SET ENEMY
			this.m_PlayerController.SetTargetEnemy (this.m_EnemyController);
			this.m_EnemyController.SetTargetEnemy  (this.m_PlayerController);
			// ROTATION
			this.m_PlayerController.SetRotation (this.m_EnemyController.GetPosition ());
			this.m_EnemyController.SetRotation  (this.m_PlayerController.GetPosition ());
		}

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
			}
		}

		protected virtual void OnCharacterInactive(object[] args) {
//			var charCtrl = args [0] as CCharacterController;
//			Debug.LogError ("OnCharacterInactive " + charCtrl.name);
		}
		
	}
}
