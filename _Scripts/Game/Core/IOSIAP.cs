#if UNITY_IPHONE || UNITY_IOS
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.UI;
using UnityEngine.Purchasing.Security;
using MsgContainer;
#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

public class IOSIAP : IStoreListener
{
	
	private static IOSIAP instance = null;
	public static IOSIAP  GetIns () 
	{
		if ( instance == null ) { instance = new IOSIAP (); }                
		return instance;        
	}

	

	private IStoreController mController    = null;
	private IAppleExtensions mExtensions    = null;
	//private bool 		     mPurchaseState = false;
	//private bool 			 mInitState     = false;

	private string productName;
	private CrossPlatformValidator validator;
#if RECEIPT_VALIDATION
	private CrossPlatformValidator validator;
#endif


	public void InitIOSPurchaseItem()
	{
		Debug.Log("iap init");

		if (GameManager.Instance.mGameMode == EGameMode.EAppleOnLine) {
			var module = StandardPurchasingModule.Instance ();
			module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
			var builder = ConfigurationBuilder.Instance (module);
			builder.AddProduct ("fk3", ProductType.Consumable);
			builder.AddProduct ("fk9", ProductType.Consumable);
			builder.AddProduct ("fk30", ProductType.Consumable);

			UnityPurchasing.Initialize (this, builder);
		}

	}


	public bool PurchaseItem( uint itemID )
	{
		Debug.Log( "IOSIAP: CallBack InitiatePurchase Item " + itemID );
		for ( int idx = 0; idx < mController.products.all.Length; idx++ )
		{
			uint PurchaseID = Convert.ToUInt32( mController.products.all[idx].definition.id );
			if ( itemID != PurchaseID ) { continue; }
			mController.InitiatePurchase( mController.products.all[idx] );
			//mPurchaseState = true;
			Debug.Log( "IOSIAP: Start InitiatePurchase ItemID: " + itemID );
			return true;
		}
		return false;
	}


	public bool PurchaseItem( string itemID )
	{
		Debug.Log( "IOSIAP: CallBack InitiatePurchase Item " + itemID );
		Debug.Log (mController.products.all.Length);

		if (mController != null) {
			var product = mController.products.WithID (itemID);
			if (product != null) {
				if (product.availableToPurchase) {
					mController.InitiatePurchase (product);
					return true;
				} else {
					
					Debug.Log ("no available product");
					return false;
				}
			}
			
		} else {
			Debug.Log ("mcontroller is null");
		}



		return false;
	}
		

	public void OnInitialized( IStoreController controller , IExtensionProvider extensions )
	{
		Debug.Log( " IOSIAP: Initialized Succeed " );
		mController = controller;
		mExtensions = extensions.GetExtension<IAppleExtensions>();
		//mInitState  = true;
		if ( null == mController || null == mExtensions )
		{
			Debug.Log( "IOSIAP: Initialized Controller Is Null " );
			Debug.Log( "IOSIAP: Initialized Extensions Is Null " );
			return;
		}
		// On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature;
		// On non-Apple platforms this will have no effect; OnDeferred will never be called;
		mExtensions.RegisterPurchaseDeferredListener( OnDeferred );
	}
	private void OnDeferred( Product item )
	{
		if ( null == item ) { return; }
		Debug.Log( " IOSIAP: Purchase Deferred: " + item.definition.id );
	}
	
	public void OnInitializeFailed( InitializationFailureReason error )
	{
		Debug.Log( " IOSIAP: Initialized Failed " );
		if ( error == InitializationFailureReason.AppNotKnown )
		{
			Debug.Log( " IOSIAP: Is your App correctly uploaded on the relevant publisher console ? " );
			return;
		}
		if ( error == InitializationFailureReason.PurchasingUnavailable )
		{
			Debug.Log( " IOSIAP: Billing disabled " );
			return;
		}
		if ( error == InitializationFailureReason.NoProductsAvailable )
		{
			Debug.Log( " IOSIAP: No Products Available for Purchase !" );
			return;
		}
	}



	public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs s )
	{
		Debug.Log( " IOSIAP: Purchase OK: " + s.purchasedProduct.definition.id );
		//Debug.Log (" IOSIAP: transactionID: " + s.purchasedProduct.transactionID);
		Debug.Log("IOSIAP hasReceipt: " +  s.purchasedProduct.hasReceipt );
		//Debug.Log("IOSIAP Receipt: " +  s.purchasedProduct.receipt );
		//Debug.Log("IOSIAP ReceiptLength: " +  s.purchasedProduct.receipt.Length );

		//mPurchaseState = false;
		//Debug.Log ("base64 s.receipt:" +  Base64Encode (s.purchasedProduct.receipt));
		//Debug.Log ("base64 s.receipt:" +  Base64Decode (s.purchasedProduct.receipt));
#if RECEIPT_VALIDATION
		if ( Application.platform == RuntimePlatform.Android || 
		     Application.platform == RuntimePlatform.IPhonePlayer ||
		     Application.platform == RuntimePlatform.OSXPlayer )
		{
			try
			{
				var result = validator.Validate( s.purchasedProduct.receipt );
				foreach( IPurchaseReceipt pp in result )
				{
					AppleInAppPurchaseReceipt apple = pp as AppleInAppPurchaseReceipt;
					if ( null != apple )
					{
						Debug.Log( "IOSIAP: originalTransactionIdentifier" + apple.originalTransactionIdentifier );
						Debug.Log( "IOSIAP: cancellationDate" + apple.cancellationDate );
						Debug.Log( "IOSIAP: quantity" + apple.quantity );
					}
				}
			}
			catch( IAPSecurityException )
			{
				Debug.Log( "IOSIAP: Invalid receipt, not unlocking content " );
				return PurchaseProcessingResult.Complete;
			}
		}
#endif
		Debug.Log ("IOS IAP httprequest =====>  data ======" + s.purchasedProduct.receipt + "      " + s.purchasedProduct.definition.id);



		try
		{
			//Hashtable hs = (Hashtable)MiniJSON.jsonDecode(s.purchasedProduct.receipt);
			JsonData js = JsonMapper.ToObject(s.purchasedProduct.receipt);

			Debug.Log("Start background check, payload:[" + js["Payload"].ToString() + "][ id: " + s.purchasedProduct.definition.id);
			SDKManager.Instance.iosiapCallback(Base64Encode(js["Payload"].ToString()), s.purchasedProduct.definition.id);
				
		}
		catch(Exception e) {
			Debug.Log (e.Message);		
		}
		//GetHttpRequest (Base64Encode (s.purchasedProduct.receipt), s.purchasedProduct.definition.id);
		return PurchaseProcessingResult.Complete;
	}


	public string Base64Encode(string message)
	{
		byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(message);  
		return Convert.ToBase64String(bytes);  
	}


	public string Base64Decode(string message)
	{
		string str = string.Empty;
		str = Encoding.GetEncoding ("utf-8").GetString (Convert.FromBase64String (message));
		return str;  
	}



	public void OnPurchaseFailed( Product item, PurchaseFailureReason r )
	{
		if ( null == item ) { return; }
		Debug.Log( "IOSIAP: Purchase failed: " + item.definition.id );
		Debug.Log( "IOSIAP: Purchase failed: " + r );
		//UIManager.GetInstance ().CloseUI (EnumUIType.ChrysanthemumGUI);
		//mPurchaseState = false;
	}


}

#endif