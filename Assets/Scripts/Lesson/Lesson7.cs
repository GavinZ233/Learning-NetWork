using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lesson7 : MonoBehaviour
{
    public Button btn;

    private void Start()
    {
        btn.onClick.AddListener(SendMsg);
        NetMgr.Instance.Connect("127.0.0.1",8080);
    }

    private void SendMsg()
    {
        PlayerMsg msg = new PlayerMsg();
        msg.playerID = 4396;
        msg.playerData = new PlayerData();
        msg.playerData.atk = 31;
        msg.playerData.lev = 31;
        msg.playerData.name = "客户端发来的";


        NetMgr.Instance.Send(msg);
    }



    }
