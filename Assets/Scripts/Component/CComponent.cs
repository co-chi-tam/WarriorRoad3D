using System;
using UnityEngine;

namespace WarriorRoad {
	[Serializable]
	public class CComponent  {

		protected bool m_Inited = false;

		public virtual void Init() {
			this.m_Inited = true;
		}

		public virtual void StartComponent() {
			if (this.m_Inited == false)
				return;
		}

		public virtual void UpdateComponent(float dt) {
			if (this.m_Inited == false)
				return;
		}

		public virtual void EndComponent() {
			if (this.m_Inited == false)
				return;
		}

	}
}
