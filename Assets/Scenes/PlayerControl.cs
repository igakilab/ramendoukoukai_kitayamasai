using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //このスプリクトが読まれた時に一番最初に一回だけ実行される

    }

    // Update is called once per frame
    void Update()
    {
        //毎フレーム、永久に実行される
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
