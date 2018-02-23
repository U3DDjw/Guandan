using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopCheckItem : MonoBehaviour {

    [SerializeField]
    GameObject checkBtn;
    [SerializeField]
    Image iconCheck;
	// Use this for initialization
	void Start () {
        EventTriggerListener.Get(checkBtn).onClick = OnCheckClick;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    bool isSelect = false;
    void OnCheckClick(GameObject g)
    {
        isSelect = !isSelect;
        iconCheck.enabled = isSelect;

        
    }
}
