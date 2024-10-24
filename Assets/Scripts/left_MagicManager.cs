using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class left_MagicManager : MonoBehaviour
{
    public float speed = -10f; // ’e‚Ì‘¬“x
    public float flag = 0;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D‚ğæ“¾
        rb.velocity = transform.right * speed; // ’e‚ğ‰E•ûŒü‚É”­Ë
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("right_shield"))
        {
            Debug.Log("ƒK[ƒh¬Œ÷right");
            Vector3 pos = rb.transform.position;
            transform.position = new Vector3(pos.x, pos.y, -100);
            flag = 1;
        }
        if (collision.CompareTag("Player2"))
        {
            right_PlayerManager Player2 = collision.GetComponent<right_PlayerManager>(); // PlayerManager‚ğæ“¾
            if (flag == 1)
            {
                Player2.GuardClear(0.2f);
                flag = 0;
            }
            else if (flag == 0)
            {
                Debug.Log("UŒ‚right");
                Player2.OnDamage(); // ƒ_ƒ[ƒW‚ğ—^‚¦‚é
            }
            Destroy(gameObject); // ’e‚ğ”j‰ó
        }
        if (collision.CompareTag("right_magic"))
        {
            Debug.Log("–‚–@“¯m‚ªÕ“ËI");
            Destroy(collision.gameObject); // ‰E‘¤‚Ì–‚–@’e‚ğ”j‰ó
            Destroy(gameObject);           // ©•ª‚Ì–‚–@’e‚ğ”j‰ó
        }
    }
}


