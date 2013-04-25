using UnityEngine;
using System.Collections;

public class Mana : MonoBehaviour {	
	
	private static float FORCE = 50.0f;
	
	void Start() {
		rigidbody.AddForce(new Vector3(FORCE, 0, 0));
	}
	
	void OnCollisionEnter(Collision collision) {
		Vector3 newForce = Vector3.Reflect(rigidbody.velocity, collision.contacts[0].normal).normalized;
//		Debug.Log (rigidbody.velocity.magnitude);
		rigidbody.AddForce(newForce * Mathf.Max(0f, FORCE-rigidbody.velocity.magnitude));
		
	}
}



