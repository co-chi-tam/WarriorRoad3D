using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPool;

namespace WarriorRoad {
	public class CObjectPoolManager {

		protected static GameObject m_ObjectPoolRoot;
		protected static Dictionary<string, object> m_ObjectPool = new Dictionary<string, object>();

		public static T Get<T> (string name) where T : MonoBehaviour {
			if (m_ObjectPool.ContainsKey (name)) {
				var objPool = m_ObjectPool [name] as ObjectPool<T>;
				var objGet = objPool.Get ();
				return objGet;
			} 
			return default (T);
		}

		public static void Set<T> (string name, T obj) where T : MonoBehaviour {
			var root = GetRoot ();
			if (m_ObjectPool.ContainsKey (name)) {
				var objPool = m_ObjectPool [name] as ObjectPool<T>;
				objPool.Set (obj);
				obj.transform.SetParent (root.transform);
			} else {
				var newObjPool = new ObjectPool<T>();
				m_ObjectPool.Add (name, newObjPool);
				newObjPool.Set (obj);
				obj.transform.SetParent (root.transform);
			}
		}

		protected static GameObject GetRoot () {
			if (m_ObjectPoolRoot == null) {
				m_ObjectPoolRoot = new GameObject ("ObjectPoolRoot");
			}
			return m_ObjectPoolRoot;
		}
		
	}
}
