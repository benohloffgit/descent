using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class JNIBridge : MonoBehaviour {
#if UNITY_ANDROID
	AndroidJavaClass jc;
	AndroidJavaObject jo;
#endif
#if UNITY_IPHONE
	[DllImport("__Internal") ]
	static extern float _GetBatteryLevel();
#endif
	
	public bool isProduct0Acquired;
	public bool isProcessingPurchase;
	public bool isAllowedToBuy;
		
	public void Initialize() {
		isProduct0Acquired = false;
		isProcessingPurchase = false;
		isAllowedToBuy = true;
		
		// limit demo versions by date/time
//		DateTime dateLimit = new DateTime(2011,09,30);
//		if (DateTime.Now > dateLimit) {
			//Debug.Log("date limit reached");
//			Application.Quit();
//		}
		
#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			jc = new AndroidJavaClass("com.andruids.babyphone.Babyphone");
			jo = jc.GetStatic<AndroidJavaObject>("instance");
			isAllowedToBuy = jo.Call<bool>("isAllowedToBuy");
		}
#endif
#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			isAllowedToBuy = StoreKitBinding.canMakePayments();
			if (isAllowedToBuy) {
				string[] products = new string[1];
				products[0] = "com.andruids.babyphone.product0";
				StoreKitBinding.requestProductData(products);
			}
		}
#endif
	}
	
	public void SwitchScreenOff() {
#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			jo.Call("switchScreenOff"); 
		}
#endif
	}

	public void SwitchScreenOn() {
#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			jo.Call("switchScreenOn");
		}
#endif
	}
	
	public bool IsProductAcquired(int id) {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		isProduct0Acquired = true;
		return isProduct0Acquired;
#endif
#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			switch (id) {
				case 0 :
					if (!isProduct0Acquired) {
						isProduct0Acquired = jo.Call<bool>("isProductAcquired", id);
						isProcessingPurchase = jo.Call<bool>("isProcessingPurchase");
					}
					break;
			}	
		}
#endif
#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if (id == 0 && isProduct0Acquired) return true;
			List<StoreKitTransaction> transactions = StoreKitBinding.getAllSavedTransactions();
			foreach (StoreKitTransaction trans in transactions) {
				//Debug.Log("restoring transaction " + trans);
				if (trans.productIdentifier	== "com.andruids.babyphone.product0") {
					isProduct0Acquired = true;
				}
			}
		}
#endif
		return isProduct0Acquired;
	}

	public void BuyProduct(int id) {
#if UNITY_ANDROID
	if (Application.platform == RuntimePlatform.Android) {
			jo.Call("buyProduct", id);
	}
#endif
#if UNITY_IPHONE
		isProcessingPurchase = true;
		StoreKitBinding.purchaseProduct("com.andruids.babyphone.product" + id, 1);
#endif
	}
		
	public void ShowMarketDialog() {
#if UNITY_ANDROID
	if (Application.platform == RuntimePlatform.Android) {
		jo.Call("showMarketDialog");
	}
#endif
#if UNITY_IPHONE
		EtceteraBinding.askForReview("Rate This App", "If you like this app please rate it on iTunes!", "itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=571672483");
#endif
	}

	public void HideAd() {
#if UNITY_ANDROID
#endif
#if UNITY_IPHONE
#endif
	}
	
	public void ShowAd() {
#if UNITY_ANDROID
#endif
#if UNITY_IPHONE		
#endif
	}

	public void TrackByFlurry(int id) {
#if UNITY_ANDROID
	if (Application.platform == RuntimePlatform.Android) {
		jo.Call("trackByFlurry", id);
	}
#endif
	}

	public void TrackLevelByFlurry(int level) {
#if UNITY_ANDROID
	if (Application.platform == RuntimePlatform.Android) {
		jo.Call("trackLevelByFlurry", level);
	}
#endif
	}

	public void ProductAcquiredCallback(String item) {
		gameObject.SendMessage("SwitchToProductAcquiredBySend", item);
	}
	
	public float GetBatteryLevel() {
		float result = 1.0f;
#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			result = jo.Call<float>("getBatteryLevel");
		}
#endif
#if UNITY_IPHONE		
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			result = _GetBatteryLevel();
		}
#endif
//		Debug.Log ("Battery Level is " + result);
		return result;
	}

	public bool IsBatteryCharging() {
		bool result = false;
#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			result = jo.Call<bool>("isBatteryCharging");
		}
#endif
#if UNITY_IPHONE		
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
		}
#endif
		return result;
	}
}


