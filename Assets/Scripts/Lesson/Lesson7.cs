using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lesson7 : MonoBehaviour
{
    public InputField input;

    private void Start()
    {
        input.onValueChanged.AddListener(SendMsg);
        NetMgr.Instance.Connect("127.0.0.1",8080);
    }

    private void SendMsg(string s)
    {
        NetMgr.Instance.Send(s);
    }



    }
