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

		public virtual void LoadBingoBoard(string[] board) {
			for (int i = 0; i < this.m_BingoItems.Length; i++) {
				var number = board [i];
				this.m_BingoItems [i].SetupBingoItem (number, () => {
					Debug.Log (number);
				});
			}
		}

		public virtual void OnReadyPressed() {
			CUserManager.Instance.OnBingoRoomPlayerReady ();
		}

		public void onBingoPressed() {
			
		}

		public void OnBackPressed () {
			// WARNING
			CUserManager.Instance.OnClientRequestLeaveBingoRoom ();
			CUserManager.Instance.OnClientInitAccount ();
			CUICustomManager.Instance.ActiveLoading (true);
		}

	}
}
