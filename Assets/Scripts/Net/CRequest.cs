using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRequest {

	#region Fields

	private WWW m_WWW;

	public string URL;
	public Dictionary<string, string> headers;

	#endregion

	#region Contructor

	public CRequest () {
		this.URL 		= string.Empty;
		this.headers 	= new Dictionary<string, string> ();
	}

	public CRequest (string url)
	{
		this.URL 		= url;
		this.headers 	= new Dictionary<string, string> ();
	}

	public CRequest (string url, Dictionary<string, string> verifyHeaders)
	{
		this.URL 		= url;
		this.headers 	= verifyHeaders;
	}

	#endregion

	#region Get method

	public void Get (string url, Action<CResult> complete, Action<string> error, Action<float> process) {
		this.URL = url;
		CHandleEvent.Instance.AddEvent (this.HandleGet (this.URL, complete, error, process), null);
	}

	public IEnumerator HandleGet(string url, Action<CResult> complete, Action<string> error, Action<float> process) {
		if (Application.internetReachability != NetworkReachability.NotReachable) {
			this.m_WWW = new WWW (url);
			while (this.m_WWW.isDone == false) {
				if (process != null) {
					process (this.m_WWW.progress);
					yield return WaitHelper.WaitFixedUpdate;
				}
			}
			yield return this.m_WWW;
			if (string.IsNullOrEmpty (this.m_WWW.error) == false) {
				if (error != null) {
					error (this.m_WWW.error);
				}
				this.m_WWW.Dispose ();
			} else {
				if (complete != null) {
					complete (new CResult (this.m_WWW));
				}
			}
		} else {
			if (error != null) {
				error ("Error: Connect error, please check connect internet.");
			}
			this.m_WWW.Dispose ();
		}
		yield return WaitHelper.WaitFixedUpdate;
	}

	#endregion

	#region Post method

	public void Post (string url, Dictionary<string, string> param, Action<CResult> complete, Action<string> error, Action<float> process) {
		this.URL = url;
		CHandleEvent.Instance.AddEvent (this.HandlePost (this.URL, param, complete, error, process), null);
	}

	public IEnumerator HandlePost(string url, Dictionary<string, string> param, Action<CResult> complete, Action<string> error, Action<float> process) {
		if (Application.internetReachability != NetworkReachability.NotReachable) {
			var wForm = new WWWForm ();
			foreach (var item in param) {
				wForm.AddField (item.Key, item.Value);
			}
			this.m_WWW = new WWW (url, wForm.data, this.headers);
			while (this.m_WWW.isDone == false) {
				if (process != null) {
					process (this.m_WWW.progress);
					yield return WaitHelper.WaitFixedUpdate;
				}
			}
			yield return this.m_WWW;
			if (string.IsNullOrEmpty (this.m_WWW.error) == false) {
				if (error != null) {
					error (this.m_WWW.error);
				}
				this.m_WWW.Dispose ();
			} else {
				if (complete != null) {
					complete (new CResult (this.m_WWW));
				}
			}
		} else {
			if (error != null) {
				error ("Error: Connect error, please check connect internet.");
			}
			this.m_WWW.Dispose ();
		}
		yield return WaitHelper.WaitFixedUpdate;
	}

	#endregion

}
