using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace WarriorRoad {
	public class CMiniGameFightingTask : CSimpleClientTask {

		#region Properties

		protected CMiniGameFightingManager m_MiniGameFightingManager;

		#endregion

		#region Constructor

		public CMiniGameFightingTask () : base ()
		{
			this.taskName = "MiniGameFightingScene";
			this.nextTask = "LobbyScene";
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
			this.m_MiniGameFightingManager = CMiniGameFightingManager.GetInstance ();
			this.m_MiniGameFightingManager.OnLoadMiniGameCompleted -= this.OnLoadControllerCompleted;
			this.m_MiniGameFightingManager.OnLoadMiniGameCompleted += this.OnLoadControllerCompleted;
			this.m_MiniGameFightingManager.StartLoading ();
		}

		#endregion

		#region Fighting

		public virtual void OnLoadControllerCompleted() {
			this.m_MiniGameFightingManager.SetupTargets ();
		}


		#endregion

	}
}
