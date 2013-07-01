using System;
using UnityEngine;

public class Breadcrumb : MonoBehaviour {		
	private Play play;
	
	private float angle;
	private Vector3 startSize;
	private Vector3 sizeFrom;
	private Vector3 sizeTo;

	private static float SIZE_MIN = 1.0f;
	private static float SIZE_MAX = 1.5f;

	public void Initialize(Play play_) {
		play = play_;
		angle = 0f;
	}
	
	void Start() {
		startSize = transform.localScale;
		sizeFrom = SIZE_MIN * startSize;
		sizeTo = SIZE_MAX * startSize;
	}
	
	void FixedUpdate() {
		if (transform.renderer.isVisible) {
//		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
//		if (isShipVisible != Vector3.zero) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.up, angle);
			if (angle >= 360f) {
				angle -= 360f;
			}
			transform.localScale = Vector3.Lerp(sizeFrom, sizeTo, Math.Abs(Mathf.Sin(Time.fixedTime*2f)));
		}
	}	
}

