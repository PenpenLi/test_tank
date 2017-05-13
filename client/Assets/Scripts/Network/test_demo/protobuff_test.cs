using UnityEngine;
using System.Collections;
using ProtoTest;
using System.IO;

public class protobuff_test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //TestInfo msg = new TestInfo();
        //TestInfo msg2 = new TestInfo();

        //msg.num = 1;
        //msg.test = "xue";

        //string x = msg.test;
        //int y = msg.num;
        //Debug.Log(x + "---------" + y);

        //byte[] msgBytes;
        //using (MemoryStream stream = new MemoryStream())
        //{
        //    ProtoBuf.Serializer.Serialize(stream, msg);
        //    msgBytes = stream.ToArray();
        //}

        //int len = msgBytes.Length;
        //Debug.Log("len == " + len);

        //using (MemoryStream stream = new MemoryStream(msgBytes))
        //{
        //    msg2 = ProtoBuf.Serializer.Deserialize<TestInfo>(stream);
        //}
        ////msg2 = ProtoBuf.Serializer.Deserialize<TestInfo>(msgBytes);

        //Debug.Log(msg2.num + "---------" + msg2.test);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
