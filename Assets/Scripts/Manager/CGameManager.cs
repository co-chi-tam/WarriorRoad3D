using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSingleton;

namespace WarriorRoad {
	public class CGameManager : CMonoSingleton<CGameManager> {

		[Header("Hero")]
		[SerializeField]	protected CCharacterController m_CharacterPrefabs;
		[SerializeField]	protected CCharacterController m_CharacterController;
		[SerializeField]	protected Camera m_MainCamera;

		protected CMapManager m_MapManager;
		protected bool m_LoadingCompleted;

		protected void Start() {
			this.m_MapManager = CMapManager.GetInstance ();
			this.m_MapManager.OnMapGenerateComplete -= this.SpawnCharacter;
			this.m_MapManager.OnMapGenerateComplete += this.SpawnCharacter;
			this.m_MapManager.GenerateRoadMap ();
		}

		protected void Update() {
			if (this.m_LoadingCompleted == false)
				return;
			if (Input.GetKeyDown (KeyCode.G)) {
				this.m_MapManager.GenerateRoadMap ();
			}
			if (Input.GetKeyDown (KeyCode.L)) {
				this.m_MapManager.ClearMap ();
			}
			this.m_MainCamera.transform.LookAt (this.m_CharacterController.GetPosition ());
		}

		public virtual void SpawnCharacter() {
			StartCoroutine (this.HandleSpawnCharacter ());
		}

		protected virtual IEnumerator HandleSpawnCharacter() {
			this.m_CharacterController = Instantiate (this.m_CharacterPrefabs);
			yield return this.m_CharacterController;
			var currentBlock = this.m_MapManager.CalculateCurrentBlock (this.m_CharacterController.blockIndex);
			this.m_CharacterController.currentBlock = currentBlock;
			this.m_CharacterController.targetBlock = currentBlock;
			this.m_CharacterController.SetPosition (currentBlock.GetMovePointPosition());
			this.m_CharacterController.SetActive (true);
			this.m_LoadingCompleted = true;
		}
		
	}
}
