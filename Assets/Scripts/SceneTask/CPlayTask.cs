using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CPlayTask : CSimpleTask {

		#region Properties

		protected CGameManager m_GameManager;

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
			this.m_GameManager = CGameManager.GetInstance ();
			this.m_GameManager.OnStartGame ();
		}

		#endregion

	}
}
