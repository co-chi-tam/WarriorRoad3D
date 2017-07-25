using UnityEngine;
using System.Collections;

namespace Singleton {
	public class CMonoSingleton<T>: MonoBehaviour where T : MonoBehaviour {

		#region Singleton

		protected static T m_Instance;
		private static object m_SingletonObject = new object();
		public static T Instance {
			get { 
				lock (m_SingletonObject) {
					if (m_Instance == null) {
						var resourceLoads = Resources.LoadAll<T> ("");
						GameObject go = null;
						if (resourceLoads.Length == 0) {
							go = new GameObject ();
							m_Instance = go.AddComponent<T> ();
						} else {
							go = Instantiate (resourceLoads [0].gameObject);
							m_Instance = go.GetComponent<T> ();
						}
						go.SetActive (true);
						go.name = typeof(T).Name;
					}
					return m_Instance;
				}
			}
		}

		public static T GetInstance() {
			return Instance;
		}

		#endregion

		#region Implementation Monobehaviour

		private void Awake() {
			m_Instance = this as T;
		} 

		#endregion
	}
}
