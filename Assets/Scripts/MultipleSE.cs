using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public AudioSource[] audioSources;

    // �C�ӂ̃^�C�~���O��SE���Đ����郁�\�b�h
    public void PlaySE(int index)
    {
        if (index >= 0 && index < audioSources.Length)
        {
            audioSources[index].Play();
        }
    }
}
