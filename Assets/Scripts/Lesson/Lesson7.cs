using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Lesson7 : MonoBehaviour
{
    public Button btn;

    public Button nianBtn;
    public Button fenBtn;
    public Button nfBtn;
    private void Start()
    {



        btn.onClick.AddListener(SendMsg);
        nianBtn.onClick.AddListener(()=>
        {
            PlayerMsg msg = new PlayerMsg();
            msg.playerID = 4396;
            msg.playerData = new PlayerData();
            msg.playerData.name = "黏包1";
            byte[] msgB = msg.Writing();
            PlayerMsg msg2 = new PlayerMsg();
            msg2.playerID = 4222;
            msg2.playerData = new PlayerData();
            msg2.playerData.name = "黏包2";
            byte[] msgB2 = msg2.Writing();

            byte[] bytes = new byte[msg.GetBytesNum()+msg2.GetBytesNum()];
            
            Array.Copy(msgB,bytes,msg.GetBytesNum());
            Array.Copy(msgB2,0, bytes,msg.GetBytesNum(), msg2.GetBytesNum());
            NetMgr.Instance.TestSend(bytes);
        });
        fenBtn.onClick.AddListener(async () =>
        {
            PlayerMsg msg = new PlayerMsg();
            msg.playerID = 4396;
            msg.playerData = new PlayerData();
            msg.playerData.name = "分包";
            byte[] msgB = msg.Writing();

            byte[] b1 = new byte[10];
            byte[] b2 = new byte[msgB.Length-10];
            Array.Copy(msgB, 0, b1, 0, 10);
            Array.Copy(msgB, 10, b2, 0, msgB.Length - 10);

            NetMgr.Instance.TestSend(b1);
            await Task.Delay(500);
            NetMgr.Instance.TestSend(b2);



        });
        nfBtn.onClick.AddListener(async() =>
        {
            PlayerMsg nian = new PlayerMsg();
            nian.playerID = 3123;
            nian.playerData = new PlayerData();
            nian.playerData.name = "黏包";
            byte[] nianBytes = nian.Writing();
            PlayerMsg msg = new PlayerMsg();
            msg.playerID = 4396;
            msg.playerData = new PlayerData();
            msg.playerData.name = "分包";
            byte[] msgB = msg.Writing();

            byte[] b1 = new byte[nian.GetBytesNum()+10];
            byte[] b2 = new byte[msgB.Length - 10];
            nianBytes.CopyTo(b1,0);
            Array.Copy(msgB, 0, b1, nian.GetBytesNum(), 10);
            Array.Copy(msgB, 10, b2, 0, msgB.Length - 10);

            NetMgr.Instance.TestSend(b1);
            await Task.Delay(500);
            NetMgr.Instance.TestSend(b2);
        });





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
