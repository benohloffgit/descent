using UnityEngine;
using System.Collections;

public class Mana : MonoBehaviour {	
	
	void Start() {
		rigidbody.AddForce(new Vector3(200.0f, 0, 0));
	}
	
}



