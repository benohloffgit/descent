using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public class CoverFindThreadState {
	public GridPosition coverPosition;
	
//	private float startTime;
	private bool isProcessing = false;
	private bool isFinished = true;
	
	public CoverFindThreadState() {
	}
	
	public void Start() {
//		startTime = Time.realtimeSinceStartup;
		isProcessing = true;
		isFinished = false;
	}
	
	public void Finish() {
		isFinished = true;
	}
	
	public void Complete() {
		isProcessing = false;
//		Debug.Log("CoverFind completed in " + (Time.realtimeSinceStartup-startTime));
	}
	
	public bool IsFinishedNow() {
		return (isFinished && isProcessing);
	}
}


