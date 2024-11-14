using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class right_MagicManager : MonoBehaviour
{
   
    public float speed = -10f; // �e�̑��x
    public float damege;
    public float flag = 0;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D���擾
        rb.velocity = transform.right * speed; // �e���E�����ɔ���
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("left_shield"))
        {
            Debug.Log("�K�[�h����right");
         
            Vector3 pos = rb.transform.position;
            transform.position = new Vector3(pos.x, pos.y, -100);
            flag = 1;
        }
        if (collision.CompareTag("Player1"))
        {
            left_PlayerManager Player1 = collision.GetComponent<left_PlayerManager>(); // PlayerManager���擾
            Debug.Log("flag = " + flag);
            if (flag == 1)
            {
                Player1.GuardClear(0.2f);
                flag = 0;
            }
            else if (flag == 0) 
            {
                Debug.Log("�U��right");
                Player1.OnDamage(damege); // �_���[�W��^����
            }
            Destroy(gameObject); // �e��j��
        }
    }

}

