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
        // �G�f�B�^�[�ł̎��s���~
        UnityEditor.EditorApplication.isPlaying = false;
        // �r���h���ꂽ�Q�[�����I��
        Application.Quit();
    }
}
