using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarriorRoad {
	public class CUIMiniGameBingoManager : CMonoSingleton<CUIMiniGameBingoManager> {

		[Header ("BINGO ITEMS")]
		[SerializeField]	protected CUIBingoItem[] m_BingoItems;

		protected CMiniGameBingoTask m_MiniGameBingoTask;
		protected List<int> m_CurrentActiveList;
		protected int[] m_Board;

		protected virtual void Start() {
			this.m_MiniGameBingoTask = CRootTask.Instance.GetCurrentTask () as CMiniGameBingoTask;
			this.m_CurrentActiveList = new List<int> ();
		}

		public virtual void LoadBingoBoard(int[] board) {
			this.m_Board = new int[board.Length];
			Array.Copy (board, this.m_Board, board.Length);
			for (int i = 0; i < this.m_BingoItems.Length; i++) {
				var number = board [i].ToString ();
				this.m_BingoItems [i].SetupBingoItem (number, () => {
					Debug.Log (number);
				});
			}
		}

		public virtual void ActiveANumber(int value) {
			this.m_CurrentActiveList.Add (value);
			var itemIndex = Array.IndexOf<int> (this.m_Board, value);
			if (itemIndex != -1) {
				this.m_BingoItems [itemIndex].BingoItem ();
			}
		}

		public virtual void OnReadyPressed() {
			this.m_MiniGameBingoTask.OnBingoRoomPlayerReady ();
		}

		public void onBingoPressed() {
			
		}

		public void OnBackPressed () {
			// WARNING
			this.m_MiniGameBingoTask.OnClientRequestLeaveBingoRoom ();
			CUserManager.Instance.OnClientInitAccount ();
			CUICustomManager.Instance.ActiveLoading (true);
		}

	}
}
