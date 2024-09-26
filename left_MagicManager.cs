using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class left_MagicManager : MonoBehaviour
{
    public float speed = -10f; // 弾の速度
    public float flag = 0;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dを取得
        rb.velocity = transform.right * speed; // 弾を右方向に発射
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("衝突検出right"); // デバッグログを追加
        if (collision.CompareTag("shield"))
        {
            Debug.Log("ガード成功right");
            Vector3 pos = rb.transform.position;
            transform.position = new Vector3(pos.x, pos.y, -100);
            flag = 1;
        }
        if (collision.CompareTag("Player2"))
        {
            right_PlayerManager Player2 = collision.GetComponent<right_PlayerManager>(); // PlayerManagerを取得
            Debug.Log("flag = " + flag);
            if (flag == 1)
            {
                Player2.GuardClear(0.2f);
                flag = 0;
            }
            else if (flag == 0)
            {
                Debug.Log("攻撃right");
                Player2.OnDamage(); // ダメージを与える
            }
            Destroy(gameObject); // 弾を破壊
        }
    }
}


