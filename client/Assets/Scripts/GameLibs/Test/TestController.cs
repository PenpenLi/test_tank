using UnityEngine;
using System.Collections;
using TKGame;
using TKBase;

public class TestController : MonoBehaviour {
    private CharacterLogic m_pCharacterLogic;
	// Use this for initialization
	void Start () {
        m_pCharacterLogic = GameGOW.Get().CharacterMgr.GetCharacterByUid(2);

    }
	
	// Update is called once per frame
	void Update () {
        if (m_pCharacterLogic == null)
        {
            Start();
            return;
        }
        float x = Input.GetAxis("Horizontal");

        if (x == 0)
        {
            m_pCharacterLogic.OnDirectionKeyChanged(0);
        }
        else if (x < 0)
        {
            m_pCharacterLogic.OnDirectionKeyChanged(InputDefine.MOVE_LEFT);
        }
        else
        {
            m_pCharacterLogic.OnDirectionKeyChanged(InputDefine.MOVE_RIGHT);
        }

        float y = Input.GetAxis("Vertical");
        if(y < 0)
        {
            m_pCharacterLogic.OnAttackAngleChange(InputDefine.MOVE_UP);
        }
        else if (y > 0)
        {
            m_pCharacterLogic.OnAttackAngleChange(InputDefine.MOVE_DOWN);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_pCharacterLogic.OnFunctionKeyDown(InputDefine.ATTACK);
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            m_pCharacterLogic.OnFunctionKeyUp(InputDefine.ATTACK);
        }
	}
}
