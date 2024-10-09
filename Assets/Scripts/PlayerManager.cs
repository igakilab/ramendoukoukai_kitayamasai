using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRadius;
    public Transform highShotPoint;
    public Transform lowShotPoint;
    public Transform shotPoint;
    public GameObject bulletPrefab;
    public GameObject bulletPrefabFire;
    public GameObject bulletPrefabBallt;
    public float jumpForce = 5f;
    public float jump_cnt = 0;

    Rigidbody2D rb;
    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Shot();
        Jump();
        Movement();
    }

    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal"); //方向キー横
        if (x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        animator.SetFloat("Speed", Mathf.Abs(x));
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
    }

    void Shot()
    {
        float flag = 0;
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            shotPoint = highShotPoint;
            bulletPrefab = bulletPrefabFire;
            flag = 1;
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            shotPoint = lowShotPoint;
            bulletPrefab = bulletPrefabFire;
            flag = 1;
        }

        if (Input.GetKeyDown(KeyCode.N) && Input.GetKey("up"))
        {
            shotPoint = highShotPoint;
            bulletPrefab = bulletPrefabBallt;
            flag = 1;
        }
        if(Input.GetKeyDown(KeyCode.N) && Input.GetKey("down"))
        {
            shotPoint = lowShotPoint;
            bulletPrefab = bulletPrefabBallt;
            flag = 1;
        }
        
        if(flag == 1)
        {
            animator.SetTrigger("attack");
            Instantiate(bulletPrefab, shotPoint.position, transform.rotation);//三つ目は自分の向き
        }

    }

    void Jump()
    {
        Vector3 pos = rb.transform.position;
        if (jump_cnt < 1)
        {
            //スペースキーを押されたかどうかを判定
            if (Input.GetButtonDown("Jump"))
            {
                //キャラクターにジャンプさせる
                animator.SetTrigger("jump");
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                jump_cnt++;
            }
        }
        if (-2.65f < pos.y && pos.y < -2.50f)
        {
            jump_cnt = 0;
        }
    }
}


