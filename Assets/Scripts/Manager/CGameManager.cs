﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSingleton;

namespace WarriorRoad {
	public class CGameManager : CMonoSingleton<CGameManager> {

		[Header("Hero")]
		[SerializeField]	protected CHeroController m_CharacterController;
		[SerializeField]	protected Camera m_MainCamera;

		protected CMapManager m_MapManager;
		protected CUserManager m_UserManager;
		protected bool m_LoadingCompleted;
		protected int m_LevelPerBlock = 4;

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

		public virtual void OnStartGame() {
			var heroData = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CCharacterData;
			this.m_MapManager = CMapManager.GetInstance ();
			this.m_MapManager.OnMapGenerateComplete -= this.SpawnCharacter;
			this.m_MapManager.OnMapGenerateComplete += this.SpawnCharacter;
			this.m_MapManager.GenerateRoadMap (heroData.characterLevel + this.m_LevelPerBlock);

			this.m_UserManager = CUserManager.GetInstance();
		}

		public virtual void OnUpdateGame() {
		
		}

		public virtual void OnEndGame() {
			
		}

		public virtual void OnLoadingCompleted() {
			this.m_LoadingCompleted = true;
			this.m_UserManager.OnClientInitMap ();
		}

		public virtual void SpawnCharacter() {
			StartCoroutine (this.HandleSpawnCharacter ());
		}

		protected virtual IEnumerator HandleSpawnCharacter() {
			var heroData = CTaskUtil.Get (CTaskUtil.HERO_DATA) as CCharacterData;
			this.m_CharacterController = Instantiate (Resources.Load<CHeroController>("CharacterPrefabs/" + heroData.objectModel));
			yield return this.m_CharacterController;
			// INIT DATA
			this.m_CharacterController.SetActive (true);
			this.m_CharacterController.SetData (heroData);
			this.m_CharacterController.Init ();
			// SET CURRENT BLOCK
			var currentBlock = this.m_MapManager.CalculateCurrentBlock (this.m_CharacterController.GetStep());
			this.m_CharacterController.currentBlock = currentBlock;
			this.m_CharacterController.targetBlock = currentBlock;
			this.m_CharacterController.SetPosition (currentBlock.GetMovePointPosition());
			this.OnLoadingCompleted ();
		}
		
	}
}
