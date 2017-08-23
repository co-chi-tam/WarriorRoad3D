using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CPlayTask : CSimpleClientTask {

		#region Properties

		protected CGameManager m_GameManager;
		protected List<CCharacterData> m_MapObjects;

		#endregion

		#region Constructor

		public CPlayTask () : base ()
		{
			this.taskName = "PlayScene";
			this.nextTask = "LoginScene";
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			// MAP
			this.RegisterEvent  ("clientInitMap", 		this.OnClientReceiveMapObjects);
			// DICE
			this.RegisterEvent ("clientReceiveDice", 	this.OnClientReceiveDice); 
			// UPDATE CLIENT
			this.RegisterEvent ("clientUpdated", 		this.OnClientUpdate);
			// RESET RANDOM SEED.
			UnityEngine.Random.InitState ((int) DateTime.Now.Ticks);
			this.m_IsLoadingTask = false;
			this.m_GameManager = CGameManager.GetInstance ();
			this.m_GameManager.OnEventLoadingCompleted -= this.OnLoadTaskCompleted;
			this.m_GameManager.OnEventLoadingCompleted += this.OnLoadTaskCompleted;
			this.m_GameManager.OnStartGame ();
		}

		#endregion

		#region Main methods

		protected virtual void OnLoadTaskCompleted() {
			this.m_IsLoadingTask = true;
		}

		#endregion

		#region Map

		public virtual void OnClientInitMap() {
			if (this.m_UserManager.IsConnected() == false)
				return;
			this.m_UserManager.Emit ("clientInitMap", new JSONObject());
		}

		public virtual void OnClientReceiveMapObjects(SocketIOEvent obj) {
			Debug.LogWarning ("OnClientReceiveMapObjects " + obj.data.ToString());	
			var mapList = obj.data.GetField ("mapBlocks").list;
			this.m_MapObjects = new List<CCharacterData> ();
			for (int i = 0; i < mapList.Count; i++) {
				var objectStr = mapList [i].ToString ();
				if (objectStr.Equals ("null") == false) {
					var objectData = TinyJSON.JSON.Load (objectStr).Make<CCharacterData> ();
					this.m_MapObjects.Add (objectData);
				} else {
					this.m_MapObjects.Add (null);
				}
			}
			var mapPath = obj.data.GetField ("mapPath").ToString ().Replace ("\"", string.Empty);
			CMapManager.Instance.GenerateRoadMap (mapPath, 7);
			CMapManager.Instance.OnMapGenerateComplete -= OnLoadBlockMapCompleted;
			CMapManager.Instance.OnMapGenerateComplete += OnLoadBlockMapCompleted;
		}

		private void OnLoadBlockMapCompleted() {
			CMapManager.Instance.LoadMapObject (this.m_MapObjects);
		}

		public virtual void OnClientUpdate(SocketIOEvent obj) {
			Debug.LogWarning ("clientUpdated " + obj.data.ToString());	
		}

		public virtual void OnClientCompletedMap() {
			if (this.m_UserManager.IsConnected() == false)
				return;
			this.m_UserManager.Emit ("clientCompletedMap", new JSONObject());
		}

		public virtual void OnClientEndGame() {
			if (this.m_UserManager.IsConnected() == false)
				return;
			this.m_UserManager.Emit ("clientEndGame", new JSONObject());
		}

		#endregion

		#region Hero

		public virtual void OnClientUpdateHero(CCharacterData clientData) {
			if (this.m_UserManager.IsConnected() == false)
				return;
			if (clientData == null)
				return;
			var dictData = new Dictionary<string, string> ();
			dictData ["hhealth"] = clientData.characterHealthPoint.ToString();
			var jsonSend = JSONObject.Create (dictData);
			this.m_UserManager.Emit ("clientUpdateHero", jsonSend);
		}

		#endregion

		#region Dice

		public virtual void OnClientRollDice() {
			if (this.m_UserManager.IsConnected() == false)
				return;
			this.m_UserManager.Emit ("clientRollDice", new JSONObject());
		}

		public virtual void OnClientReceiveDice(SocketIOEvent obj) {
			Debug.LogWarning ("OnClientReceiveDice " + obj.ToString ());
			var step 		= int.Parse (obj.data.GetField ("diceStep").ToString());
			var curEnergy 	= int.Parse (obj.data.GetField ("currentEnergy").ToString());
			var maxEnergy 	= int.Parse (obj.data.GetField ("maxEnergy").ToString());
			CGameManager.Instance.OnPlayerUpdateStep (step, curEnergy, maxEnergy);
		}

		#endregion

	}
}
