using System;

public interface Focussable {
	void LostFocus();
	bool IsBlocking(); // blocks all other clicks while focussed
}

