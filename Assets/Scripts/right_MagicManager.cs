using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class right_MagicManager : MonoBehaviour
{
   
    public float speed = -10f; // ’e‚Ì‘¬“x
    public float damege;
    public float flag = 0;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D‚ğæ“¾
        rb.velocity = transform.right * speed; // ’e‚ğ‰E•ûŒü‚É”­Ë
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("left_shield"))
        {
            Debug.Log("ƒK[ƒh¬Œ÷right");
         
            Vector3 pos = rb.transform.position;
            transform.position = new Vector3(pos.x, pos.y, -100);
            flag = 1;
        }
        if (collision.CompareTag("Player1"))
        {
            left_PlayerManager Player1 = collision.GetComponent<left_PlayerManager>(); // PlayerManager‚ğæ“¾
            Debug.Log("flag = " + flag);
            if (flag == 1)
            {
                Player1.GuardClear(0.2f);
                flag = 0;
            }
            else if (flag == 0) 
            {
                Debug.Log("UŒ‚right");
                Player1.OnDamage(damege); // ƒ_ƒ[ƒW‚ğ—^‚¦‚é
            }
            Destroy(gameObject); // ’e‚ğ”j‰ó
        }
    }

}

