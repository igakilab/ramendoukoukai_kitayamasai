using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //���̃X�v���N�g���ǂ܂ꂽ���Ɉ�ԍŏ��Ɉ�񂾂����s�����

    }

    // Update is called once per frame
    void Update()
    {
        //���t���[���A�i�v�Ɏ��s�����
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            if (this.transform.position.x > -4)
                this.transform.position += Vector3.left * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (this.transform.position.x < 4)
                this.transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }
}
