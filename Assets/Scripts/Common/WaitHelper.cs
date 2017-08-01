using UnityEngine;
using System;
using System.Collections;

public class WaitHelper {

	private static WaitForFixedUpdate m_WaitForFixedUpdate = new WaitForFixedUpdate ();
	public static WaitForFixedUpdate WaitFixedUpdate {
		get { 
			return m_WaitForFixedUpdate;
		}
	}

	private static WaitForEndOfFrame m_WaitForEndOfFrame = new WaitForEndOfFrame ();
	public static WaitForEndOfFrame WaitEndOfFrame {
		get { 
			return m_WaitForEndOfFrame;
		}
	}

	private static WaitForSeconds m_WaitForSortSeconds = new WaitForSeconds (1f);
	public static WaitForSeconds WaitForShortSeconds {
		get { 
			return m_WaitForSortSeconds;
		}
	}

	private static WaitForSeconds m_WaitForLongSeconds = new WaitForSeconds (3f);
	public static WaitForSeconds WaitForLongSeconds {
		get { 
			return m_WaitForLongSeconds;
		}
	}

}
