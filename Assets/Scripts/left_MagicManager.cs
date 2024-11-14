using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class left_MagicManager : MonoBehaviour
{
   
    public float speed = -10f; // eΜ¬x
    public float damage;
    public float flag = 0;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2DπζΎ
        rb.velocity = transform.right * speed; // eπEϋόΙ­Λ
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("right_shield"))
        {
            Debug.Log("K[h¬χright");
            Vector3 pos = rb.transform.position;
            transform.position = new Vector3(pos.x, pos.y, -100);
            flag = 1;
        }
        if (collision.CompareTag("Player2"))
        {
            right_PlayerManager Player2 = collision.GetComponent<right_PlayerManager>(); // PlayerManagerπζΎ
            if (flag == 1)
            {
                Player2.GuardClear(0.2f);
                flag = 0;
            }
            else if (flag == 0)
            {
                Debug.Log("Uright");
                Player2.OnDamage(damage); // _[Wπ^¦ι
            }
            Destroy(gameObject); // eπjσ
        }
        if (collision.CompareTag("right_magic"))
        {
            Debug.Log("@―mͺΥΛI");
            Destroy(collision.gameObject); // E€Μ@eπjσ
            Destroy(gameObject);           // ©ͺΜ@eπjσ
        }
    }
}


