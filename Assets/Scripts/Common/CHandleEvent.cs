using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CHandleEvent : MonoBehaviour {

	#region Singleton

	private static CHandleEvent m_Instance;
	private static object m_SingletonObject = new object();

	public static CHandleEvent Instance {
		get { 
			lock (m_SingletonObject) {
				if (m_Instance == null) {
					var go = new GameObject ("HandleEvent");
					m_Instance = go.AddComponent<CHandleEvent> ();
				}
				return m_Instance;
			}
		}
	}

	public static CHandleEvent GetInstance() {
		return Instance;
	}

	#endregion

	#region Properties

	private LinkedList <EventEntry> m_HandleList;

	#endregion

	#region Internal class

	public class EventEntry
	{
		public IEnumerator eventMethod;
		public Action eventCallBack;
		public Action<float> eventProcessing;
		public Func<bool> evenBreak;
		public float delayTime = 0;
		public float timeCreated = 0;
		public float timeUpdate = 0;
	}

	#endregion

	#region MonoBehaviour

	protected virtual void Awake ()
	{
		DontDestroyOnLoad (this.gameObject);
		m_Instance = this;
		m_HandleList = new LinkedList<EventEntry> ();
	}

	protected virtual void Update ()
	{
		this.UpdateEvent ();
	}

	protected virtual void OnDestroy() {
		
	}

	#endregion

	#region Main methods

	private void UpdateEvent() {
		if (m_HandleList == null)
			return;
		var count = m_HandleList.Count;
		for (int i = 0; i < count; i++) {
			var eventEntry = m_HandleList.Last.Value;
			if (eventEntry.evenBreak != null) {
				if (eventEntry.evenBreak ()) {
					m_HandleList.RemoveLast ();
					count--;
					continue;
				}
			}
			if (eventEntry.timeUpdate - eventEntry.timeCreated >= eventEntry.delayTime) {
				if (eventEntry.eventMethod != null) {
					StartCoroutine (this.HandleEvent (eventEntry.eventMethod, eventEntry.eventCallBack));
				} else {
					if (eventEntry.eventCallBack != null) {
						eventEntry.eventCallBack ();
					}
				}
				m_HandleList.RemoveLast ();
				count--;
			} else {
				if (eventEntry.eventProcessing != null) {
					eventEntry.eventProcessing ((eventEntry.timeUpdate - eventEntry.timeCreated) / eventEntry.delayTime);
				}
				eventEntry.timeUpdate = Time.time;
				m_HandleList.RemoveLast ();
				m_HandleList.AddFirst (eventEntry);
			}
		}
	}

	public void AddEvent(float delay, IEnumerator handleObject, Action complete = null, Action<float> processing = null, Func<bool> broke = null) {
		var eventEntry = new EventEntry ();
		eventEntry.eventMethod = handleObject;
		eventEntry.eventCallBack = complete;
		eventEntry.eventProcessing = processing;
		eventEntry.evenBreak = broke;
		eventEntry.delayTime = delay;
		eventEntry.timeCreated = eventEntry.timeUpdate = Time.time;
		m_HandleList.AddFirst (eventEntry);
	}

	public void AddEvent(IEnumerator handleObject, Action complete = null) {
		AddEvent (0f, handleObject, complete, null, null);
	}

	public void DoEventImmediately(IEnumerator handleObject) {
		StartCoroutine (handleObject);
	}

	public void RemoveEvent(CHandleEvent.EventEntry eventEntry) {
		m_HandleList.Remove (eventEntry);
	}

	private IEnumerator HandleEvent(IEnumerator handleObject, Action complete = null) {
		yield return handleObject;
		if (complete != null) {
			complete ();
		}
	}

	#endregion

}
