using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

public class CResult {

	public string text;

	public CResult (WWW www)
	{
		this.text = www.text;
	}

	public Dictionary<string, object> ToJSONObject () {
		return Json.Deserialize (this.text) as Dictionary<string, object>;
	}

}
