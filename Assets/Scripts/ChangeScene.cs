using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;  // 遷移するシーンの名前
    //public Button myButton;   // ボタンの参照


    /*private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];*/

    void Start()
    {
        // ボタンクリック時にOnButtonClickメソッドを呼び出す
        //myButton.onClick.AddListener(OnButtonClick);

        /*m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);*/
    }

    void Update()
    {
        //PushJoyCon();
    }

    /*private void PushJoyCon()
    {
        if (m_joyconL.GetButton(Joycon.Button.DPAD_RIGHT) || m_joyconL.GetButton(Joycon.Button.DPAD_DOWN) || m_joyconL.GetButton(Joycon.Button.SHOULDER_1) 
            || m_joyconL.GetButton(Joycon.Button.SHOULDER_2) || m_joyconL.GetButton(Joycon.Button.DPAD_LEFT) || m_joyconL.GetButton(Joycon.Button.DPAD_UP) || m_joyconL.GetButton(Joycon.Button.STICK))
        {
            Resources.UnloadUnusedAssets();
            SceneManager.LoadScene(sceneName);
        }
        else if(m_joyconR.GetButton(Joycon.Button.DPAD_RIGHT) || m_joyconR.GetButton(Joycon.Button.DPAD_DOWN) || m_joyconR.GetButton(Joycon.Button.SHOULDER_1)
            || m_joyconR.GetButton(Joycon.Button.SHOULDER_2) || m_joyconR.GetButton(Joycon.Button.DPAD_LEFT) || m_joyconR.GetButton(Joycon.Button.DPAD_UP) || m_joyconR.GetButton(Joycon.Button.STICK))
        {
            Resources.UnloadUnusedAssets();
            SceneManager.LoadScene(sceneName);
        }

        
    }*/
    public void Load()
    {
        SceneManager.LoadScene(sceneName);
    }

}
