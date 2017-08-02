using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CObjectController : MonoBehaviour {

		#region Properties

		protected Transform m_Transform;
		protected bool m_Active;
		protected List <CComponent> m_ListComponents;

		#endregion

		#region Implementation MonoBehaviour

		protected virtual void Awake() {
			this.m_Transform = this.transform;
			this.m_ListComponents = new List<CComponent> ();
			this.RegisterComponent ();
			this.m_Active = true;
		}

		protected virtual void Start() {
			this.OnStartComponent ();
		}

		protected virtual void Update() {
			this.OnUpdateComponent (Time.deltaTime);
		}

		protected virtual void OnDestroy () {
			this.OnEndComponent ();
		}

		#endregion

		#region Component

		protected virtual void RegisterComponent() {
		
		}

		private void OnStartComponent() {
			for (int i = 0; i < this.m_ListComponents.Count; i++) {
				this.m_ListComponents [i].StartComponent ();
			}
		}

		private void OnUpdateComponent(float dt) {
			for (int i = 0; i < this.m_ListComponents.Count; i++) {
				this.m_ListComponents [i].UpdateComponent (dt);
			}
		}

		private void OnEndComponent() {
			for (int i = 0; i < this.m_ListComponents.Count; i++) {
				this.m_ListComponents [i].EndComponent ();
			}
		}

		#endregion

		#region Getter && Setter

		public virtual void SetData(CObjectData value) {
			
		}

		public virtual CObjectData GetData() {
			return null;
		}

		public virtual void SetActive(bool value) {
			this.m_Active = value;
			if (value == false) {
				this.OnEndComponent ();
			}
		}

		public virtual bool GetActive() {
			return this.m_Active;
		}

		public virtual void SetPosition(Vector3 value) {
			this.m_Transform.position = value;
		}

		public virtual Vector3 GetPosition() {
			return this.m_Transform.position;
		}

		public virtual void SetRotation(Vector3 value) {
			var direction = value - this.m_Transform.position;
			var angle = Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
			this.m_Transform.rotation = Quaternion.AngleAxis (angle, Vector3.up);
		}

		public virtual Vector3 GetRotation() {
			return this.m_Transform.rotation.eulerAngles;
		}

		public virtual void SetAnimation(string name, object param) {
			
		} 

		#endregion
			
	}
}
