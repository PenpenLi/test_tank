using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {

    private float _value;
    public float Value
    {
        set {
            _value = value;
            if(_value < 0f)
            {
                _value = 0f;
            }
            else if(_value > 1f)
            {
                _value = 1f;
            }
            m_pValue.localScale = new Vector3(width * _value, m_pValue.localScale.y, m_pValue.localScale.z);
        }
        get { return _value; }
    }
    private static Color _helpColor = new Color();
    private float color;
    public float Color
    {
        set
        {
            color = value;
            var renderer = m_pValue.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                _helpColor.r = color;
                _helpColor.g = 0f;
                _helpColor.b = 0f;
                _helpColor.a = 255f;
                renderer.color = _helpColor;
            }
        }
        get { return color; }
    }

    private float width;
    public Transform m_pValue;
    // Use this for initialization
    void Start () {
        width = m_pValue.localScale.x;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
