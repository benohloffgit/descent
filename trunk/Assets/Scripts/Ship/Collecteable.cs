using System;
using UnityEngine;

public abstract class Collecteable : MonoBehaviour {		
	protected Play play;
	public abstract void DispatchFixedUpdate();
	
	public void DispatchFixedUpdateGeneral() {
		DispatchFixedUpdate();
	}
}

