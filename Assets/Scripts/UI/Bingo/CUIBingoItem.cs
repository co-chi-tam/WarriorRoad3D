using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarriorRoad {
	public class CUIBingoItem : MonoBehaviour {

		public Text bingoText;
		public Button bingoButton;
		public GameObject selectedItemGO;
		public GameObject bingoItemGO;

		protected bool m_Selected = false;

		public void SelectedItem() {
			this.m_Selected = !this.m_Selected;
			this.selectedItemGO.SetActive (this.m_Selected);
		}

		public void BingoItem() {
			this.bingoItemGO.SetActive (true);
		}

		public void SetupBingoItem (string text, Action submit) {
			this.bingoText.text = text;
			this.bingoButton.onClick.RemoveAllListeners ();
			this.bingoButton.onClick.AddListener (() => {
				if (submit != null) {
					submit ();
				}
			});
		}
		
	}
}
