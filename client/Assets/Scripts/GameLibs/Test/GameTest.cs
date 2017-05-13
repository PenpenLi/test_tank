using UnityEngine;
using System.Collections;
using TKBase;

class ClassTest
{
    public string name;
}

public class GameTest : MonoBehaviour {
    public GameObject m_stTestBomb;
    // Use this for initialization

    void ChangeString(string str)
    {
        Debug.Log("ChangeString Before: " + str);
        str = "ChangeString";
        Debug.Log("ChangeString After: " + str);
    }

    void ChangeClassString(ClassTest strClass)
    {
        Debug.Log("ChangeClassString Before: " + strClass.name);
        strClass = new ClassTest();
        strClass.name = "ChangeString";
        Debug.Log("ChangeClassString After: " + strClass.name);
    }

    void ChangeStringByRef(ref string str)
    {
        Debug.Log("ChangeStringByRef Before: " + str);
        str = "ChangeStringByRef";
        Debug.Log("ChangeStringByRef After: " + str);
    }

    void ChangeClassStringByRef(ref ClassTest strClass)
    {
        Debug.Log("ChangeClassStringByRef Before: " + strClass.name);
        strClass = new ClassTest();
        strClass.name = "ChangeString";
        Debug.Log("ChangeClassStringByRef After: " + strClass.name);
    }

    private int index;
    IEnumerator PerFrameLoad()
    {
        while (index < 3)
        {
            index++;
            yield return new WaitForSeconds(1f);

            LOG.Log("after yield index = " + index);
        }
    }

    void Start () {
        Debug.Log("Start");
        index = 0;

        StartCoroutine(PerFrameLoad());

        string testStr = "testStr";
        ClassTest testClass = new ClassTest();
        testClass.name = "testClass.name";
        ChangeString(testStr);
        Debug.Log("Start|ChangeString:" + testStr);

        ChangeClassString(testClass);
        Debug.Log("Start|ChangeClassString:" + testClass.name);

        ChangeStringByRef(ref testStr);
        Debug.Log("Start|ChangeStringByRef:" + testStr);

        ChangeClassStringByRef(ref testClass);
        Debug.Log("Start|ChangeClassStringByRef:" + testClass.name);

        PerFrameLoad();
        LOG.Log("after PerFrameLoad");
    }

    

    // Update is called once per frame
    void Update () {
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        //    Vector3 mousePositionOnScreen = Input.mousePosition;
        //    mousePositionOnScreen.z = screenPosition.z;
        //    Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionOnScreen);
        //    GameObject abc = Instantiate(m_stTestBomb, mousePositionInWorld, Quaternion.identity) as GameObject;
        //}
	}
}
