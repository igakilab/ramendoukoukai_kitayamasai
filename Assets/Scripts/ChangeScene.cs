using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeOnClick : MonoBehaviour
{
    public string sceneName;  // 遷移するシーンの名前
    public Button myButton;   // ボタンの参照


    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private void Start()
    {
        // ボタンクリック時にOnButtonClickメソッドを呼び出す
        myButton.onClick.AddListener(OnButtonClick);

        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    private void Update()
    {
        JoyControll();
        PushJoyCon();
    }

    private void PushJoyCon()
    {
        if (m_joyconL.GetButton(Joycon.Button.DPAD_RIGHT) || m_joyconL.GetButton(Joycon.Button.DPAD_DOWN) || m_joyconL.GetButton(Joycon.Button.SHOULDER_1) 
            || m_joyconL.GetButton(Joycon.Button.SHOULDER_2) || m_joyconL.GetButton(Joycon.Button.DPAD_LEFT) || m_joyconL.GetButton(Joycon.Button.DPAD_UP) || m_joyconL.GetButton(Joycon.Button.STICK))
        {
            SceneManager.LoadScene(sceneName);
        }
        else if(m_joyconR.GetButton(Joycon.Button.DPAD_RIGHT) || m_joyconR.GetButton(Joycon.Button.DPAD_DOWN) || m_joyconR.GetButton(Joycon.Button.SHOULDER_1)
            || m_joyconR.GetButton(Joycon.Button.SHOULDER_2) || m_joyconR.GetButton(Joycon.Button.DPAD_LEFT) || m_joyconR.GetButton(Joycon.Button.DPAD_UP) || m_joyconR.GetButton(Joycon.Button.STICK))
        {
            SceneManager.LoadScene(sceneName);
        }

        
    }
    private void OnButtonClick()
    {
        // シーンをロードする
        SceneManager.LoadScene(sceneName);
    }

    private void JoyControll()
    {
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        foreach (var button in m_buttons)
        {
            if (m_joyconL.GetButton(button))
            {
                m_pressedButtonL = button;
            }
            if (m_joyconR.GetButton(button))
            {
                m_pressedButtonR = button;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_joyconL.SetRumble(160, 320, 0.6f, 200);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_joyconR.SetRumble(160, 320, 0.6f, 200);
        }
    }


}
