using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {

	void Awake() {
		CenterOnScreen();	
	}
	
	public void CenterOnScreen() {
		// center x,y
		Vector3 pos = transform.position;
		pos.x = Camera.main.transform.position.x;
		pos.y = Camera.main.transform.position.y;
		transform.position = pos;
	}
}