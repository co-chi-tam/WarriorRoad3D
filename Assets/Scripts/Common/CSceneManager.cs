using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleSingleton;

public class CSceneManager: CMonoSingleton<CSceneManager> {

	#region Properties


	[SerializeField]	private Color m_ScreenLoadingColor = Color.white;
	[Range(0.25f, 5f)]
	[SerializeField]	private float m_ScreenLoadingTime = 1f;

	public Action<Scene, Scene> activeSceneChanged;

	private Texture2D m_LoadingScreenTexture;
	private Rect m_FullScreenRect;
	private bool m_IsFadeOut = false;
	private bool m_NeedDraw = false;

	#endregion

	#region Implementation MonoBehavious

	protected override void Awake ()
	{
		base.Awake ();
		DontDestroyOnLoad (this.gameObject);
		m_FullScreenRect = new Rect (0f, 0f, Screen.width, Screen.height);
		SceneManager.activeSceneChanged += delegate(Scene arg0, Scene arg1) {
			if (activeSceneChanged != null) {
				activeSceneChanged (arg0, arg1);
			}	
		};
		m_LoadingScreenTexture = new Texture2D (1, 1);
		m_LoadingScreenTexture.SetPixels (new Color[] { m_ScreenLoadingColor });
		m_LoadingScreenTexture.Apply ();
	}

	protected virtual void Start() {
		OnFadeOutScreen();
	}

	protected virtual void OnGUI() {
		if (Event.current.type.Equals (EventType.Repaint) && m_NeedDraw) {
			GUI.DrawTexture (m_FullScreenRect, m_LoadingScreenTexture, ScaleMode.StretchToFill);
			var currentColor = m_LoadingScreenTexture.GetPixels () [0];
			var fadeAlpha = 1f / m_ScreenLoadingTime * Time.deltaTime; 
			var currentAlpha = m_IsFadeOut ? currentColor.a - fadeAlpha : currentColor.a + fadeAlpha;
			PaintAlphaTexture (currentAlpha);
			m_NeedDraw = m_IsFadeOut ? currentColor.a > 0f : true;
		}
	}

	#endregion

	#region Main methods

	/// <summary>
	/// Repairs the texture.
	/// </summary>
	private void RepairTexture(float alpha) {
		m_LoadingScreenTexture = new Texture2D (1, 1);
		m_ScreenLoadingColor.a = alpha;
		m_LoadingScreenTexture.SetPixels (new Color[] { m_ScreenLoadingColor });
		m_LoadingScreenTexture.Apply ();
	}

	/// <summary>
	/// Paints the alpha texture.
	/// </summary>
	private void PaintAlphaTexture(float alpha) {
		var currentColor = m_LoadingScreenTexture.GetPixels () [0];
		currentColor.a = alpha;
		m_LoadingScreenTexture.SetPixel (0, 0, currentColor);
		m_LoadingScreenTexture.Apply ();
	}

	/// <summary>
	/// Fade out screen.
	/// </summary>
	private void OnFadeOutScreen() {
		PaintAlphaTexture (1f);
		m_IsFadeOut = true;
		m_NeedDraw = true;
	}

	/// <summary>
	/// Fade in screen.
	/// </summary>
	private void OnFadeInScreen() {
		PaintAlphaTexture (0f);
		m_IsFadeOut = false;
		m_NeedDraw = true;
	}

	/// <summary>
	/// Loads the scene async.
	/// </summary>
	public IEnumerator HandleLoadSceneAsync(string sceneName, Func<bool> onComplete = null) {
		this.OnFadeInScreen ();
		yield return WaitHelper.WaitForShortSeconds;
		var sceneNameWithoutExtension = Path.GetFileNameWithoutExtension (sceneName);
		yield return SceneManager.LoadSceneAsync (sceneNameWithoutExtension);
		if (onComplete != null) {
			while (onComplete () == false) {
				yield return WaitHelper.WaitFixedUpdate;
			}
		}
		this.OnFadeOutScreen ();
	}

	#endregion

	#region Getter && Setter

	/// <summary>
	/// Gets the name of the active scene.
	/// </summary>
	public string GetActiveSceneName () {
		return SceneManager.GetActiveScene ().name;
	}

	#endregion

}
