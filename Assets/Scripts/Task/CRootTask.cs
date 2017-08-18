using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleSingleton;

namespace WarriorRoad {
	public class CRootTask : CMonoSingleton<CRootTask> {

		#region Properties

		[SerializeField]	private string m_CurrentTaskName;	

		private CTask m_CurrentTask;
		private CMapTask m_MapTask;
		private string m_PreviousTask;

		private string m_NextTaskInProcess = string.Empty;

		#endregion

		#region Implementation MonoBehavious

		protected override void Awake ()
		{
			base.Awake ();
			DontDestroyOnLoad (this.gameObject);
			this.m_MapTask = new CMapTask ();
			this.m_CurrentTask = this.m_MapTask.GetFirstTask ();
			this.m_CurrentTaskName = this.m_CurrentTask.GetTaskName ();
			CLog.Init ();
		}

		protected virtual void Start ()
		{
			// First load
			this.m_CurrentTask.Transmission ();
			this.m_PreviousTask = this.m_CurrentTask.GetTaskName();
			this.SetupTask();
			// Other load
			CSceneManager.Instance.activeSceneChanged += (Scene oldScene, Scene currentScene) => {
				this.SetupTask();
			};
		}

		protected virtual void Update ()
		{
			if (this.m_CurrentTask != null && this.m_CurrentTask.IsStartedTask) {
				this.m_CurrentTask.UpdateTask (Time.deltaTime);
			}
		}

		protected virtual void OnApplicationQuit() {
			this.SaveTask ();
		}

		protected virtual void OnApplicationPause(bool value) {
#if !UNITY_EDITOR
			if (value) {
				this.SaveTask ();
			}
#endif
		}

		protected virtual void OnApplicationFocus(bool value) {
#if !UNITY_EDITOR
			if (value == false) {
				this.SaveTask ();
			}
#endif
		}

		#endregion

		#region Main methods

		public virtual void NextTask() {
			if (string.IsNullOrEmpty (this.m_NextTaskInProcess)) { 
				this.TransmissionTask (this.m_CurrentTask.nextTask);
			} else {
				this.TransmissionTask (this.m_NextTaskInProcess);
			}
			this.m_NextTaskInProcess = string.Empty;
		}

		public virtual void PreviousTask() {
			this.TransmissionTask (this.m_PreviousTask);
		}

		public virtual void ProcessNextTask(string name) {
			this.m_NextTaskInProcess = name;
		}

		private void SetupTask() {
			this.m_CurrentTask.OnCompleteTask -= NextTask;
			this.m_CurrentTask.OnCompleteTask += NextTask;
			this.m_CurrentTask.StartTask ();
			this.m_CurrentTaskName = this.m_CurrentTask.GetTaskName ();
		}

		private void TransmissionTask(string taskName) {
			this.m_PreviousTask = this.m_CurrentTask.GetTaskName();
			this.m_CurrentTask.EndTask ();
			this.m_CurrentTask = this.m_MapTask.GetTask (taskName);
			if (this.m_CurrentTask != null) {
				this.m_CurrentTask.Transmission ();
				if (this.m_CurrentTask.taskName != CSceneManager.Instance.GetActiveSceneName ()) {
					StartCoroutine (this.LoadScene (this.m_CurrentTask.taskName));	
				}
			} else {
				Debug.LogError ("[TASK ERROR] " + taskName);
			}
			this.m_CurrentTaskName = this.m_CurrentTask.GetTaskName ();
		}

		protected virtual IEnumerator LoadScene(string name) {
			var sceneLoading = CSceneManager.Instance.HandleLoadSceneAsync (name, () => {
				return this.m_CurrentTask.IsLoadingTask();
			});
			this.m_CurrentTask.OnSceneLoading ();
			yield return sceneLoading;
			this.m_CurrentTask.OnSceneLoaded ();
		}

		public virtual CTask GetCurrentTask() {
			return this.m_CurrentTask;
		}

		private void SaveTask() {

		}

		#endregion

	}

}
