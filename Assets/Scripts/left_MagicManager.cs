using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class left_MagicManager : MonoBehaviour
{
    public float speed = -10f; // �e�̑��x
    public float flag = 0;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D���擾
        rb.velocity = transform.right * speed; // �e���E�����ɔ���
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("right_shield"))
        {
            Debug.Log("�K�[�h����right");
            Vector3 pos = rb.transform.position;
            transform.position = new Vector3(pos.x, pos.y, -100);
            flag = 1;
        }
        if (collision.CompareTag("Player2"))
        {
            right_PlayerManager Player2 = collision.GetComponent<right_PlayerManager>(); // PlayerManager���擾
            if (flag == 1)
            {
                Player2.GuardClear(0.2f);
                flag = 0;
            }
            else if (flag == 0)
            {
                Debug.Log("�U��right");
                Player2.OnDamage(); // �_���[�W��^����
            }
            Destroy(gameObject); // �e��j��
        }
        if (collision.CompareTag("right_magic"))
        {
            Debug.Log("���@���m���ՓˁI");
            Destroy(collision.gameObject); // �E���̖��@�e��j��
            Destroy(gameObject);           // �����̖��@�e��j��
        }
    }
}


