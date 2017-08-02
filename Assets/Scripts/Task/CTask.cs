using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarriorRoad {
	public class CTask : ITask {

		#region Properties

		public string taskName {
			get { return this.m_TaskName; }
			protected set { this.m_TaskName = value; }
		}
		public string nextTask {
			get { return this.m_NextTask; }
			set { this.m_NextTask = value; }
		}

		public Action OnCompleteTask;

		protected bool m_IsLoadingTask = true;
		protected bool m_IsCompleteTask = false;

		protected string m_TaskName;
		protected string m_NextTask;

		#endregion

		#region Constructor

		public CTask ()
		{
			this.taskName = string.Empty;
			this.nextTask = string.Empty;
		}

		#endregion

		#region Implementation Task

		public virtual void StartTask() {
			
		}

		public virtual void UpdateTask(float dt) {
			
		}

		public virtual void EndTask() {
			this.OnCompleteTask = null;
			this.m_IsCompleteTask = false;
		}

		public virtual void Transmission() {
			
		}

		public virtual void OnTaskCompleted() {
			this.m_IsCompleteTask = true;
			if (this.OnCompleteTask != null) {
				this.OnCompleteTask ();
			}
		}

		public virtual void OnTaskFail() {
			this.m_IsCompleteTask = false;
		}

		public virtual void SaveTask() {
		
		}

		public virtual void OnSceneLoading() {

		}

		public virtual void OnSceneLoaded() {

		}

		public virtual bool IsCompleteTask() {
			return this.m_IsCompleteTask;
		}

		public virtual bool IsLoadingTask() {
			return this.m_IsLoadingTask;
		}

		public virtual bool IsHiddenTask() {
			return false;
		}

		public virtual string GetTaskName() {
			return this.taskName;
		}

		#endregion

	}
}
