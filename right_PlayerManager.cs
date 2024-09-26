using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class right_PlayerManager : MonoBehaviour
{
    public float moveSpeed = 3f; // �ړ����x
    public float attackRadius; // �U���͈�
    public Transform highShotPoint; // �����ʒu�̃V���b�g�|�C���g
    public Transform lowShotPoint; // �Ⴂ�ʒu�̃V���b�g�|�C���g
    public Transform shotPoint; // �V���b�g�|�C���g
    public GameObject deathEffectPrefab; // ���S�G�t�F�N�g�̃v���n�u
    public GameObject GuardObject; // �K�[�h�I�u�W�F�N�g
    public GameObject bulletPrefab; // �e�̃v���n�u
    public float jumpForce = 5f; // �W�����v��
    public float jump_cnt = 0; // �W�����v��
    public bool isRight; // �E�������ǂ���
    public float hp = 10; // �q�b�g�|�C���g
    public float coolTime = 0.3f; //�ҋ@����
    public float leftCoolTime; //�ҋ@���Ă��鎞��
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


    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D���擾
        animator = GetComponent<Animator>(); // Animator���擾
        HealthSetGauge(1f); // HpGauge�̏�����
        CriticalSetGauge(0f);
    }

    private void Update()
    {
        //Movement(); // �ړ�����
        Shot(); // �V���b�g����
        //Jump(); // �W�����v����
        Guard(); // �K�[�h����
    }

    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal"); // �����L�[���̓��͂��擾
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
        animator.SetFloat("Speed", Mathf.Abs(x)); // �A�j���[�V�����̑��x��ݒ�
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y); // �ړ����x��ݒ�
    }

    void Shot()
    {
        leftCoolTime -= Time.deltaTime;
        float flag = 0;
        if (Input.GetKeyDown(KeyCode.N) && Input.GetKey("up"))
        {
            shotPoint = highShotPoint; // �����ʒu����V���b�g
            flag = 1;
        }
        if (Input.GetKeyDown(KeyCode.N) && Input.GetKey("down"))
        {
            shotPoint = lowShotPoint; // �Ⴂ�ʒu����V���b�g
            flag = 1;
        }
        if (flag == 1 && leftCoolTime <= 0)
        {
            animator.SetTrigger("attack"); // �U���A�j���[�V�������Đ�
            Instantiate(bulletPrefab, shotPoint.position, transform.rotation); // �e�𐶐�
            leftCoolTime = coolTime;
        }
    }

    void Jump()
    {
        Vector3 pos = rb.transform.position;
        if (jump_cnt < 2)
        {
            if (Input.GetButtonDown("Jump"))
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

        if (Input.GetKey(KeyCode.V) && Input.GetKey("up"))
        {
            shotPoint = highShotPoint; // �����ʒu�ŃK�[�h
            flag = 1;
        }
        if (Input.GetKey(KeyCode.V) && Input.GetKey("down"))
        {
            shotPoint = lowShotPoint; // �Ⴂ�ʒu�ŃK�[�h
            flag = 1;
        }

        if (flag == 1)
        {
            animator.SetTrigger("guard"); // �K�[�h�A�j���[�V�������Đ�
            Instantiate(GuardObject, shotPoint.position, transform.rotation); // �K�[�h�I�u�W�F�N�g�𐶐�
        }
    }

    public void OnDamage()
    {
        Debug.Log("�_���[�W���󂯂�"); // �f�o�b�O���O��ǉ�
        hp -= 1; // �q�b�g�|�C���g������
        TakeDamage(0.1f); // HpGauge�̍X�V
        if (hp <= 0)
        {
            Instantiate(deathEffectPrefab, transform.position, transform.rotation); // ���S�G�t�F�N�g�𐶐�
            Destroy(gameObject); // �v���C���[�I�u�W�F�N�g��j��
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
        //DoTween��A�����ē�����
        CriticalImage.DOFillAmount(value, duration);
        //.OnComplete(() =>
        //   {
        //      burnImage
        //           .DOFillAmount(value, duration / 2f)
        //           .SetDelay(0.5f);
        //  });
        //transform.DOShakePosition(
        //    duration / 2f,
        //   strength, vibrate);

        // �V�F�[�_�[�̊m�F
        Material material = CriticalImage.material;
        Debug.Log("�g�p���Ă���V�F�[�_�[: " + material.shader.name);

        // �}�e���A���̃v���p�e�B�m�F
        Debug.Log("�}�e���A���̃v���p�e�B: " + material.GetTexture("_MainTex"));

        criticalcurrentRate = value;
    }

    public void TakeDamage(float rate)
    {
        HealthSetGauge(healthcurrentRate - rate);
    }

    public void GuardClear(float rate)
    {
        CriticalSetGauge(criticalcurrentRate + rate);
    }

}
