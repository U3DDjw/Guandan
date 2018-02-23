using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;

public class TestRsa : MonoBehaviour {

    [SerializeField]
    Text  lab;

    public string key = "";
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	if(Input.GetKeyDown(KeyCode.Q))
        {

           // lab.text=SHA1withRSA.encrypt(key, "hello");
            string content = "hello";
       
         //   lab.text = RSAVerify.SignatureFormatter(content);
        }

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    lab.text = RSAVerify.RSADecrypt(lab.text);
        //}


    }
}
