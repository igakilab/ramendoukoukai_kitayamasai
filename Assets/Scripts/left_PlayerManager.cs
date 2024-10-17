using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;      //�V�[���̐؂�ւ��ɕK�v

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

    public string SceneName;    //�ǂݍ��ރV�[����


    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D���擾
        animator = GetComponent<Animator>(); // Animator���擾
        HealthSetGauge(1f); // HpGauge�̏�����
        CriticalSetGauge(0f);

        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    private void Update()
    {
        JoyControll();
        Movement(); // �ړ�����
        Shot(); // �V���b�g����
        Jump(); // �W�����v����
        Guard(); // �K�[�h����
    }

    void Movement_1()
    {
        float x = Input.GetAxis("JoystickHorizontal"); // �����L�[���̓��͂��擾
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

    void Movement()
    {
        // Joy-Con�̃X�e�B�b�N���͂��擾
        float x = 0f;
        //if (m_joyconL != null)
        //{
        //    x += m_joyconL.GetStick()[0]; // ��Joy-Con�̉������͂��擾
        //}
        if (m_joyconL != null)
        {
            x += m_joyconL.GetStick()[0]; // �EJoy-Con�̉������͂��擾
        }

        if (!isRight && x < 0)
        {
            transform.Rotate(0f, 180f, 0f); // �E�����ɉ�]
            isRight = true;
        }
        if (isRight && x > 0)
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
        if (m_joyconL.GetButton(Joycon.Button.DPAD_RIGHT))
        {
            shotPoint = highShotPoint; // �����ʒu����V���b�g
            flag = 1;
        }
        if (m_joyconL.GetButton(Joycon.Button.DPAD_DOWN))
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

        if (m_joyconL.GetButton(Joycon.Button.DPAD_UP))
        {
            shotPoint = highShotPoint; // �����ʒu�ŃK�[�h
            flag = 1;
        }
        if (m_joyconL.GetButton(Joycon.Button.DPAD_LEFT))
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
            m_joyconL.SetRumble(160, 320, 0.6f, 200);
            Load();
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


    private void JoyControll()
    {
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        foreach (var button in m_buttons)
        {
            if (m_joyconL.GetButton(button))
            {
                m_pressedButtonL = button;
            }
            if (m_joyconR.GetButton(button))
            {
                m_pressedButtonR = button;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_joyconL.SetRumble(160, 320, 0.6f, 200);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_joyconR.SetRumble(160, 320, 0.6f, 200);
        }
    }


    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 24;

        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con ���ڑ�����Ă��܂���");
            return;
        }

        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) ���ڑ�����Ă��܂���");
            return;
        }

        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) ���ڑ�����Ă��܂���");
            return;
        }

        GUILayout.BeginHorizontal(GUILayout.Width(960));

        foreach (var joycon in m_joycons)
        {
            var isLeft = joycon.isLeft;
            var name = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";
            var key = isLeft ? "Z �L�[" : "X �L�[";
            var button = isLeft ? m_pressedButtonL : m_pressedButtonR;
            var stick = joycon.GetStick();
            var gyro = joycon.GetGyro();
            var accel = joycon.GetAccel();
            var orientation = joycon.GetVector();

            GUILayout.BeginVertical(GUILayout.Width(480));
            GUILayout.Label(name);
            GUILayout.Label(key + "�F�U��");
            GUILayout.Label("������Ă���{�^���F" + button);
            GUILayout.Label(string.Format("�X�e�B�b�N�F({0}, {1})", stick[0], stick[1]));
            GUILayout.Label("�W���C���F" + gyro);
            GUILayout.Label("�����x�F" + accel);
            GUILayout.Label("�X���F" + orientation);
            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();
    }

    //�V�[����ǂݍ���
    public void Load()
    {
        SceneManager.LoadScene(SceneName);
    }

}