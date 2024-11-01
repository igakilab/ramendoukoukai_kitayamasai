using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;      //�V�[���̐؂�ւ��ɕK�v

public class left_PlayerManager : MonoBehaviour
{
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    public float moveSpeed = 3f; // �ړ����x
    public AudioSource seAudioSource;
    public float attackRadius; // �U���͈�
    public Transform highShotPoint; // �����ʒu�̃V���b�g�|�C���g
    public Transform lowShotPoint; // �Ⴂ�ʒu�̃V���b�g�|�C���g
    public Transform CriticalShotPoint; // �Ⴂ�ʒu�̃V���b�g�|�C���g
    public Transform shotPoint; // �V���b�g�|�C���g
    public GameObject deathEffectPrefab; // ���S�G�t�F�N�g�̃v���n�u
    public GameObject GuardObject; // �K�[�h�I�u�W�F�N�g
    public GameObject bulletPrefab; // �e�̃v���n�u
    public GameObject CriticalbulletPrefab; // �e�̃v���n�u
    public GameObject[] stockObject;
    public float jumpForce = 5f; // �W�����v��
    public float jump_cnt = 0; // �W�����v��
    public bool isRight; // �E�������ǂ���
    public float hp = 10; // �q�b�g�|�C���g
    public int CriticalPoint = 0;
    public int i = 0;
    public int cnt = 0;
    public float coolTime = 0.3f; //�ҋ@����
    public float leftCoolTime; //�ҋ@���Ă��鎞��
    public float forceAmount = 5f; // �ӂ��Ƃ΂��͂̋���
    public Vector2 direction; // �ӂ��Ƃ΂�����
    public int stock = 3; // �L�����N�^�[�̎c�@�i�X�g�b�N�j
    public Vector3 respawnPosition; // ���X�|�[������ꏊ
    public float respawnDelay = 3f; // ���X�|�[������܂ł̑ҋ@����
    public bool action_dead = false;
    public bool action_guard = false;
    // ���̐F��ۑ����邽�߂̕ϐ�
    private Color originalColor;

    // �ύX����F
    public Color newColor = Color.red;

    private Renderer renderer;


    // HpGauge�֘A�̃t�B�[���h
    [SerializeField] private Image healthImage;
    [SerializeField] private Image burnImage;
    [SerializeField] private Image CriticalImage;
    public float duration = 0.5f;
    public float strength = 20f;
    public int vibrate = 100;
    private float healthcurrentRate = 1f;
    private float criticalcurrentRate = 1f;
    Rigidbody2D rb;
    Animator animator;

    public string SceneName;    //�ǂݍ��ރV�[����


    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D���擾
        animator = GetComponent<Animator>(); // Animator���擾
        renderer = GetComponent<Renderer>();
        HealthSetGauge(1f); // HpGauge�̏�����
        CriticalSetGauge(0f);

        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);

        respawnPosition  = transform.position;
        originalColor = renderer.material.color;
    }

    private void Update()
    {
        Movement(); // �ړ�����
        if(!action_dead)
        {
            Jump(); // �W�����v����
            action_guard = false;
            Guard(); // �K�[�h����
            if(!action_guard)
            {
                Shot();
                if (CriticalPoint >= 5)
                {
                    CriticalShot();
                }
            }
            
        }
        cnt = 0;
    }


    void Movement()
    {
        float x = 0f;
        if (m_joyconL != null)
        {
            x += m_joyconL.GetStick()[0]; 
        }

        if (!isRight && x > 0)
        {
            transform.Rotate(0f, 180f, 0f); 
            isRight = true;
        }
        if (isRight && x < 0)
        {
            transform.Rotate(0f, 180f, 0f); 
            isRight = false;
        }
        if (!action_dead)
        {
            animator.SetFloat("Speed", Mathf.Abs(x)); 
        }
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y); 
    }

    void Shot()
    {
        leftCoolTime -= Time.deltaTime;
        float flag = 0;
        if (m_joyconL.GetButton(Joycon.Button.DPAD_RIGHT))
        {
            shotPoint = highShotPoint; 
            flag = 1;
        }
        if (m_joyconL.GetButton(Joycon.Button.DPAD_DOWN))
        {
            shotPoint = lowShotPoint; 
            flag = 1;
        }
        if (flag == 1 && leftCoolTime <= 0)
        {
            PlaySound();
            animator.SetTrigger("attack"); 
            Instantiate(bulletPrefab, shotPoint.position, transform.rotation); 
            leftCoolTime = coolTime;
        }
    }

    void CriticalShot()
    {
        if (m_joyconL.GetAccel()[0] > 2)
        {
            animator.SetTrigger("attack"); 
            int j;
            for(j = 0; j < 10; j++)
            {
                Instantiate(CriticalbulletPrefab, CriticalShotPoint.position, transform.rotation); 
            }
            leftCoolTime = coolTime;
            CriticalPoint = 0;
            CriticalSetGauge(0f);
        }
    }

    void Jump()
    {
        Vector3 pos = rb.transform.position;
        if (jump_cnt < 2)
        {
            if (m_joyconL.GetButton(Joycon.Button.STICK))
            {
                
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

    void Guard()
    {
        float flag = 0;

        if (m_joyconL.GetButton(Joycon.Button.SHOULDER_1))
        {
            shotPoint = highShotPoint; 
            flag = 1;
        }
        if (m_joyconL.GetButton(Joycon.Button.SHOULDER_2))
        {
            shotPoint = lowShotPoint; 
            flag = 1;
        }

        if (flag == 1)
        {
            action_guard = true;
            animator.SetTrigger("guard"); 
            Instantiate(GuardObject, shotPoint.position, transform.rotation); 
        }
    }

    public void OnDamage()
    {
        
        hp -= 1; 
        TakeDamage(0.1f); 
        ChangeColor();
        Invoke("ResetColor", 0.1f);
        if (hp <= 0)
        {
            stock--; 

            if (stock > 0)
            {
                hp = 10f;
                CriticalPoint = 0;
                HealthSetGauge(1f); 
                CriticalSetGauge(0f);
     
                Destroy(stockObject[i]);
                i++;
                rb.velocity = Vector3.zero;
                StartCoroutine(Respawn(rb));

            }
            else
            {
                Instantiate(deathEffectPrefab, transform.position, transform.rotation); 
                Destroy(gameObject);
                Load();
            }
        }
    }

    // HpGauge�֘A�̃��\�b�h
    public void HealthSetGauge(float value)
    {
        //DoTween��A�����ē�����
        healthImage.DOFillAmount(value, duration)
            .OnComplete(() =>
            {
                burnImage
                    .DOFillAmount(value, duration / 2f)
                    .SetDelay(0.5f);
            });
        //transform.DOShakePosition(
        //    duration / 2f,
        //   strength, vibrate);

        // �V�F�[�_�[�̊m�F
        Material material = healthImage.material;
        Debug.Log("�g�p���Ă���V�F�[�_�[: " + material.shader.name);

        // �}�e���A���̃v���p�e�B�m�F
        Debug.Log("�}�e���A���̃v���p�e�B: " + material.GetTexture("_MainTex"));

        healthcurrentRate = value;
    }

    public void CriticalSetGauge(float value)
    {
        CriticalImage.DOFillAmount(value, duration);
        
        Material material = CriticalImage.material;
        Debug.Log("�g�p���Ă���V�F�[�_�[: " + material.shader.name);

        Debug.Log("�}�e���A���̃v���p�e�B: " + material.GetTexture("_MainTex"));

        criticalcurrentRate = value;
    }

    public void TakeDamage(float rate)
    {
        HealthSetGauge(healthcurrentRate - rate);
    }

    public void GuardClear(float rate)
    {
        CriticalPoint++;
        CriticalSetGauge(criticalcurrentRate + rate);
    }

    private IEnumerator Respawn(Rigidbody2D rb)
    {
        Debug.Log("���X�|�[���ҋ@��...");
        
        Physics2D.gravity = Vector2.zero;
        // ���b�ԑҋ@
        action_dead = true;
        transform.position = respawnPosition;
        yield return new WaitForSeconds(respawnDelay);
        action_dead = false;
        Physics2D.gravity = new Vector2(0, -9.81f);

        Debug.Log("���X�|�[������");
    }

    //�V�[����ǂݍ���
    public void Load()
    {
        SceneManager.LoadScene(SceneName);
    }

    //�ڐG�J�n
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Dead")
        {
            hp = 1;
            OnDamage();
        }
    }

    public void PlaySound()
    {
        seAudioSource.Play();
    }

    public void ChangeColor()
    {
        if (renderer != null)
        {
            renderer.material.color = newColor;
        }
    }
    public void ResetColor()
    {
        if (renderer != null)
        {
            renderer.material.color = originalColor;
        }
    }

}