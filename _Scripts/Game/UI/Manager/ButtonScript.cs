using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonHandler
{
	private event OnTouchButtonHandler mHandler = null;
	private object mParam1 = 0;
	private object mParam2 = 0;
	public void DestroyHandler()
	{
		if(mHandler != null)
		{
			mHandler -= mHandler;
			mHandler = null;
		}
	}
	
	public void SetHander(OnTouchButtonHandler handler, object param1, object param2)
	{
		DestroyHandler();

		mHandler += handler;
		mParam1 = param1;
		mParam2 = param2;
	}
	
	public void HandlerCallback(ButtonScript buttonScript, object obj)
	{
		if(mHandler != null)
		{
			mHandler(buttonScript, obj, (int)mParam1, (int)mParam2);
		}
	}
}

public class ButtonScript : MonoBehaviour 
{	
//	private event OnTouchButtonHandler mOnTouchButtonHandler = null;
//	private int mOnTouchButtonHandlerParam1;
//	private int mOnTouchButtonHandlerParam2;
	
	public int m_HandlerParam1;
	public int m_HandlerParam2;
	
	public event OnTouchButtonHandler mOnTouchButtonHandler = null;
	public event OnTouchButtonHandler mOnPressButtonHandler = null;
	public event OnTouchButtonHandler mOnDropButtonHandler = null;
	
	public event OnTouchButtonHandler mCheckOnButtonHandler = null;
	public event OnTouchButtonHandler mCheckOffButtonHandler = null;

	public event OnTouchButtonHandler mLongTapHandler = null;
	////////////////////////////////////////////////////////////
	// new 
	private ButtonHandler mOnClick = null;
	private ButtonHandler mOnClick4Tab = null;
	private ButtonHandler mOnDoubleClick = null;	
	private ButtonHandler mOnPress = null;
	private ButtonHandler mOnDrop = null;
	
	private ButtonHandler mCheckOn = null;
	private ButtonHandler mCheckOff = null;
	
	private ButtonHandler mTouchDown = null;
	private ButtonHandler mTouchUp = null;

	private ButtonHandler mLongTap = null;

	private long lastClickTime  = 0;
	private int interval       = 100;
	
	////////////////////////////////////////////////////////////
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	void OnDestroy()
	{
		RemoveAllHandler();
	}
	
//	// Update is called once per frame
//	void Update () 
//	{
//	
//	}
	
	void RemoveAllHandler()
	{
		if (mOnTouchButtonHandler != null)
		{
			mOnTouchButtonHandler -= mOnTouchButtonHandler;
		}
		
		if(mOnPressButtonHandler != null)
		{
			mOnPressButtonHandler -= mOnPressButtonHandler;
		}
		
		if(mOnDropButtonHandler != null)
		{
			mOnDropButtonHandler -= mOnDropButtonHandler;
		}
		
		if(mCheckOnButtonHandler != null)
		{
			mCheckOnButtonHandler -= mCheckOnButtonHandler;
		}
		
		if(mCheckOffButtonHandler != null)
		{
			mCheckOffButtonHandler -= mCheckOffButtonHandler;
		}		

		if(mLongTapHandler != null)
		{
			mLongTapHandler -= mLongTapHandler;
		}
		////////////////////////////////////////////////////////////
		//new
		if(mOnClick != null)
		{
			mOnClick.DestroyHandler();
			mOnClick = null;
		}

		if(mOnClick4Tab != null)
		{
			mOnClick4Tab.DestroyHandler();
			mOnClick4Tab = null;
		}

		if(mOnDoubleClick != null)
		{
			mOnDoubleClick.DestroyHandler();
			mOnDoubleClick = null;
		}

		if(mOnPress != null)
		{
			mOnPress.DestroyHandler();
			mOnPress = null;
		}
		
		if(mCheckOn != null)
		{
			mCheckOn.DestroyHandler();
			mCheckOn = null;
		}
		
		if(mCheckOff != null)
		{
			mCheckOff.DestroyHandler();
			mCheckOff = null;
		}
		
		if(mOnDrop != null)
		{
			mOnDrop.DestroyHandler();
			mOnDrop = null;
		}
		
		if(mTouchDown != null)
		{
			mTouchDown.DestroyHandler();
			mTouchDown = null;
		}
		
		if(mTouchUp != null)
		{
			mTouchUp.DestroyHandler();
			mTouchUp = null;
		}

		if(mLongTap != null)
		{
			mLongTap.DestroyHandler();
			mLongTap = null;
		}
		////////////////////////////////////////////////////////////
	}
		
	public void OnClick()
	{
      
		if(mOnTouchButtonHandler != null)
		{
			mOnTouchButtonHandler(this, null, m_HandlerParam1, m_HandlerParam2);
		}

        Toggle checkbox = GetComponent<Toggle>();
		if(checkbox != null)
		{
			if(checkbox.isOn)
			{
				if(mCheckOnButtonHandler != null)
				{
					mCheckOnButtonHandler(this, null, m_HandlerParam1, m_HandlerParam2);
				}
			}
			else
			{
				if(mCheckOffButtonHandler != null)
				{
					mCheckOffButtonHandler(this, null, m_HandlerParam1, m_HandlerParam2);
				}
			}
		}
		
		////////////////////////////////////////////////////////////
		//new
		if( mOnClick != null )
		{
		//	AudioManager.Instance.PlayGameAudio( 10002012 );
			mOnClick.HandlerCallback( this, null );
		}

		if( mOnClick4Tab != null )
		{
		//	AudioManager.Instance.PlayGameAudio( 10002018 );
			mOnClick4Tab.HandlerCallback( this, null );
		}


		//UIToggle checkbox = GetComponent<UIToggle>();
		if( checkbox != null )
		{
			if(checkbox.isOn)
			{
				if(mCheckOn != null)
				{
					mCheckOn.HandlerCallback(this, null);
				}
			}
			else
			{
				if(mCheckOff != null)
				{
					mCheckOff.HandlerCallback(this, null);
				}
			}
		}

		////////////////////////////////////////////////////////////
	}
		
	public void OnDoubleClick()
	{
		if(mOnDoubleClick != null)
		{
			mOnDoubleClick.HandlerCallback(this, null);
		}
	}

	public void OnPress(bool isPressed)
	{
		if(mOnPressButtonHandler != null)
		{
			mOnPressButtonHandler(this, isPressed, m_HandlerParam1, m_HandlerParam2);
		}
		
		////////////////////////////////////////////////////////////
		//new
		
		if(mOnPress != null)
		{
			mOnPress.HandlerCallback(this, isPressed);
		}
		
		if(isPressed)
		{
			if(mTouchDown != null)
			{
				mTouchDown.HandlerCallback(this, null);
			}
		}
		else
		{
			if(mTouchUp != null)
			{
				mTouchUp.HandlerCallback(this, null);
			}
		}
		////////////////////////////////////////////////////////////
	}
	
	public void OnDrop(GameObject go)
	{
		if(mOnDropButtonHandler != null)
		{
			mOnDropButtonHandler(this, go, m_HandlerParam1, m_HandlerParam2);
		}
		
		////////////////////////////////////////		
		//new
		if(mOnDrop != null)
		{
			mOnDrop.HandlerCallback(this, go);
		}
		
		//////////////////////////////////////////
	}

	public void OnLongPress()
	{
		if(mLongTap != null)
		{
			mLongTap.HandlerCallback(this, null);
		}
	}

	public void SetButtonScriptHandler(EnumButtonEvent buttonEvent, OnTouchButtonHandler handler, object param1, object param2)
	{	
		switch(buttonEvent)
		{
		case EnumButtonEvent.OnClick:
			if(mOnClick == null)
			{
				mOnClick = new ButtonHandler();
			}
			mOnClick.SetHander(handler, param1, param2);
			break;
		case EnumButtonEvent.OnClick4Tab:
			if(mOnClick4Tab == null)
			{
				mOnClick4Tab = new ButtonHandler();
			}
			mOnClick4Tab.SetHander(handler, param1, param2);
			break;
		case EnumButtonEvent.OnDoubleClick:
		{
			if(mOnDoubleClick == null)
			{
				mOnDoubleClick = new ButtonHandler();
			}
			mOnDoubleClick.SetHander(handler, param1, param2);
		}
			break;
		case EnumButtonEvent.OnPress:
			if(mOnPress == null)
			{
				mOnPress = new ButtonHandler();
			}
			mOnPress.SetHander(handler, param1, param2);
			break;
		case EnumButtonEvent.OnDrop:
			if(mOnDrop == null)
			{
				mOnDrop = new ButtonHandler();
			}
			mOnDrop.SetHander(handler, param1, param2);
			break;
		case EnumButtonEvent.CheckOn:
			if(mCheckOn == null)
			{
				mCheckOn = new ButtonHandler();
			}
			mCheckOn.SetHander(handler, param1, param2);
			break;
		case EnumButtonEvent.CheckOff:
			if(mCheckOff == null)
			{
				mCheckOff = new ButtonHandler();
			}
			mCheckOff.SetHander(handler, param1, param2);
			break;
		case EnumButtonEvent.TouchDown:
			if(mTouchDown == null)
			{
				mTouchDown = new ButtonHandler();
			}
			mTouchDown.SetHander(handler, param1, param2);
			break;
		case EnumButtonEvent.TouchUp:
			if(mTouchUp == null)
			{
				mTouchUp = new ButtonHandler();
			}
			mTouchUp.SetHander(handler, param1, param2);
			break;
		case EnumButtonEvent.OnLongPress:
			if(mLongTap == null)
			{
				mLongTap = new ButtonHandler();
			}
			mLongTap.SetHander(handler, param1, param2);
			break;
		}
	}
	
	public void RemoveButtonScriptHandler(EnumButtonEvent buttonEvent)
	{
		switch(buttonEvent)
		{
		case EnumButtonEvent.OnClick:
			if(mOnClick != null)
			{
				mOnClick.DestroyHandler();
				mOnClick = null;
			}
			break;
		case EnumButtonEvent.OnClick4Tab:
			if(mOnClick4Tab != null)
			{
				mOnClick4Tab.DestroyHandler();
				mOnClick4Tab = null;
			}
			break;
		case EnumButtonEvent.OnDoubleClick:
		{
			if(mOnDoubleClick != null)
			{
				mOnDoubleClick.DestroyHandler();
				mOnDoubleClick = null;
			}
		}
			break;
		case EnumButtonEvent.OnPress:
			if(mOnPress != null)
			{
				mOnPress.DestroyHandler();
				mOnPress = null;
			}
			break;
		case EnumButtonEvent.OnDrop:
			if(mOnDrop != null)
			{
				mOnDrop.DestroyHandler();
				mOnDrop = null;
			}
			break;
		case EnumButtonEvent.CheckOn:
			if(mCheckOn != null)
			{
				mCheckOn.DestroyHandler();
				mCheckOn = null;
			}
			break;
		case EnumButtonEvent.CheckOff:
			if(mCheckOff != null)
			{
				mCheckOff.DestroyHandler();
				mCheckOff = null;
			}
			break;
		case EnumButtonEvent.TouchDown:	
			if(mTouchDown != null)
			{
				mTouchDown.DestroyHandler();
				mTouchDown = null;
			}
			break;
		case EnumButtonEvent.TouchUp:
			if(mTouchUp != null)
			{
				mTouchUp.DestroyHandler();
				mTouchUp = null;
			}
			break;
		case EnumButtonEvent.OnLongPress:
			if(mTouchUp != null)
			{
				mTouchUp.DestroyHandler();
				mTouchUp = null;
			}
			break;
		}
	}
	
	public void RemoveButtonScriptAllHandler()
	{
		RemoveAllHandler();
	}
}

