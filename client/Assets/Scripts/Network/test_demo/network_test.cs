using UnityEngine;
using System.Collections;

public class network_test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Network.Instance.ConnectToServer();

		//Network.Instance.Disonnect();
		//Debug.Log("socket close()");
        //Network.Instance.Send_Login();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	private void FixedUpdate()
	{
		Network.Instance.Update();
    }
}
