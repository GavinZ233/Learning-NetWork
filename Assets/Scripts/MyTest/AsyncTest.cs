using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetAsyncMgr.Instance.Connect("127.0.0.1",8080);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NetAsyncMgr.Instance.Send("发送内容打算啊飞洒啊发十大高手给撒打发给");
        }
    }
}
