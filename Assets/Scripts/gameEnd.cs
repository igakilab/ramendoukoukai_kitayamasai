using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameend : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void gameEnd()
    {
        // エディターでの実行を停止
        UnityEditor.EditorApplication.isPlaying = false;
        // ビルドされたゲームを終了
        Application.Quit();
    }
}
