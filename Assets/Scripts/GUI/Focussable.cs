using System;
using UnityEngine;

public interface Focussable {
	void LostFocus();
	bool IsBlocking(); // blocks all other clicks while focussed
	bool IsSameAs(GameObject gO);
}

