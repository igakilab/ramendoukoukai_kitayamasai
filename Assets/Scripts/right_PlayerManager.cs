using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;      //�V�[���̐؂�ւ��ɕK�v


public class right_PlayerManager : MonoBehaviour
{
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;


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
    public float coolTime = 0.3f; //�ҋ@����
    public float leftCoolTime; //�ҋ@���Ă��鎞��
    public float forceAmount = 1f; // �ӂ��Ƃ΂��͂̋���
    public Vector2 direction; // �ӂ��Ƃ΂�����
    public int stock = 3; // �L�����N�^�[�̎c�@�i�X�g�b�N�j
    public Vector3 respawnPosition; // ���X�|�[������ꏊ
    public float respawnDelay = 3f; // ���X�|�[������܂ł̑ҋ@����
    public bool action_dead = false;
    public bool action_guard = false;
    // HpGauge�֘A�̃t�B�[���h
    [SerializeField] private Image healthImage;
    [SerializeField] private Image burnImage;
    [SerializeField] private Image CriticalImage;
    public float duration = 0.5f;
    public float strength = 20f;
    public int vibrate = 100;
    private float healthcurrentRate = 1f;
    private float criticalcurrentRate = 1f;
    // ���̐F��ۑ����邽�߂̕ϐ�
    private Color originalColor;

    // �ύX����F
    public Color newColor = Color.red;

    private Renderer renderer;

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

        respawnPosition = transform.position;
        originalColor = renderer.material.color;
    }

    private void Update()
    {
        //JoyControll();
        Movement(); // �ړ�����
        if (!action_dead)
        {
            Jump(); // �W�����v����
            action_guard = false;
            Guard(); // �K�[�h����
            if (!action_guard)
            {
                Shot();
                if (CriticalPoint >= 5)
                {
                    CriticalShot();
                }
            }
        }
    }

    void Movement()
    {
        float x = 0f;
        if (m_joyconL != null)
        {
            x += m_joyconR.GetStick()[0]; // �EJoy-Con�̉������͂��擾
        }

        if (!isRight && x > 0)
        {
            transform.Rotate(0f, 180f, 0f); // �E�����ɉ�]
            isRight = true;
        }
        if (isRight && x < 0)
        {
            transform.Rotate(0f, 180f, 0f); // �������ɉ�]
            isRight = false;
        }

        if (!action_dead)
        {
            animator.SetFloat("Speed", Mathf.Abs(x)); // �A�j���[�V�����̑��x��ݒ�
        }
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y); // �ړ����x��ݒ�
    }

    void Shot()
    {
        leftCoolTime -= Time.deltaTime;
        float flag = 0;
        if (m_joyconR.GetButton(Joycon.Button.DPAD_RIGHT))
        {
            shotPoint = highShotPoint; // �����ʒu����V���b�g
            flag = 1;
        }
        if (m_joyconR.GetButton(Joycon.Button.DPAD_DOWN))
        {
            shotPoint = lowShotPoint; // �Ⴂ�ʒu����V���b�g
            flag = 1;
        }
        if (flag == 1 && leftCoolTime <= 0)
        {
            animator.SetTrigger("attack"); // �U���A�j���[�V�������Đ�
            seAudioSource.Play();
            Instantiate(bulletPrefab, shotPoint.position, transform.rotation); // �e�𐶐�
            leftCoolTime = coolTime;
        }
    }

    void CriticalShot()
    {
        if (m_joyconR.GetAccel()[0] > 2)
        {
            animator.SetTrigger("attack"); // �U���A�j���[�V�������Đ�
            int j;
            for (j = 0; j < 10; j++)
            {
                Instantiate(CriticalbulletPrefab, CriticalShotPoint.position, transform.rotation); // �e�𐶐�
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
            if (m_joyconR.GetButton(Joycon.Button.STICK))
            {
               
                    animator.SetTrigger("jump"); // �W�����v�A�j���[�V�������Đ�
                    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // �W�����v�͂�������
                    jump_cnt++;
                
            }
        }
        if (-2.65f < pos.y && pos.y < -2.50f)
        {
            jump_cnt = 0; // �W�����v�񐔂����Z�b�g
        }
    }

    void Guard()
    {
        float flag = 0;

        if (m_joyconR.GetButton(Joycon.Button.SHOULDER_1))
        {
            shotPoint = highShotPoint; // �����ʒu�ŃK�[�h
            flag = 1;
        }
        if (m_joyconR.GetButton(Joycon.Button.SHOULDER_2))
        {
            shotPoint = lowShotPoint; // �Ⴂ�ʒu�ŃK�[�h
            flag = 1;
        }

        if (flag == 1)
        {
            action_guard = true;
            animator.SetTrigger("guard"); // �K�[�h�A�j���[�V�������Đ�
            Instantiate(GuardObject, shotPoint.position, transform.rotation); // �K�[�h�I�u�W�F�N�g�𐶐�
        }
    }

    public void OnDamage()
    {
        Debug.Log("�_���[�W���󂯂�"); // �f�o�b�O���O��ǉ�
        hp -= 1; // �q�b�g�|�C���g������
        TakeDamage(0.1f); // HpGauge�̍X�V
        rb.velocity = Vector3.zero;
        ChangeColor();
        Invoke("ResetColor", 0.1f);
        //Fling(new Vector2(1, 1));
        if (hp <= 0)
        {
            stock--; // �c�@������

            if (stock > 0)
            {
                hp = 10f;
                CriticalPoint = 0;
                HealthSetGauge(1f); // HpGauge�̏�����
                CriticalSetGauge(0f);
                // �v���C���[�����X�|�[���ʒu�Ɉړ�
                Destroy(stockObject[i]);
                i++;
                StartCoroutine(Respawn(rb));
            }
            else
            {
                Instantiate(deathEffectPrefab, transform.position, transform.rotation); // ���S�G�t�F�N�g�𐶐�
                Destroy(gameObject); // �v���C���[�I�u�W�F�N�g��j��
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

        healthcurrentRate = value;
    }
    
    public void CriticalSetGauge(float value)
    {
        //DoTween��A�����ē�����
        CriticalImage.DOFillAmount(value, duration);

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

    public void Fling(Vector2 direction)
    {
        if (rb != null)
        {
            // �͂�������i2D�̃��[���h���W�Ɋ�Â��j
            Debug.Log("������΂�����: " + direction);

            animator.SetTrigger("addforce");
            rb.AddForce(direction.normalized * 10f, ForceMode2D.Impulse);
        }
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
        if (collision.gameObject.tag == "Dead")
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

    // ���̐F�ɖ߂����\�b�h
    public void ResetColor()
    {
        if (renderer != null)
        {
            renderer.material.color = originalColor;
        }
    }
}
