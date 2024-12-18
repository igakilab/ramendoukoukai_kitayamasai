using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class left_PlayerManager : MonoBehaviour
{
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    public AudioSource[] audioSources;
    public float moveSpeed = 3f;
    public float attackRadius;
    public Transform highShotPoint;
    public Transform lowShotPoint;
    public Transform CriticalShotPoint;
    public Transform shotPoint;
    public GameObject deathEffectPrefab;
    public GameObject GuardObject;
    public GameObject bulletPrefab;
    public GameObject CriticalbulletPrefab;
    public GameObject[] stockObject;
    public float jumpForce = 5f;
    public float jump_cnt = 0;
    public bool isRight;
    public float hp = 10;
    public int CriticalPoint = 0;
    public int i = 0;
    public int cnt = 0;
    public float coolTime = 0.3f;
    public float leftCoolTime;
    public float forceAmount = 5f;
    public Vector2 direction;
    public int stock = 3;
    public Vector3 respawnPosition;
    public float respawnDelay = 3f;
    public bool action_dead = false;
    public bool action_guard = false;
    public bool action_c_1 = false;
    public bool action_c_2 = false;
    public bool action_c_3 = false;


    private Color originalColor;
    public Color newColor = Color.red;

    private Renderer renderer;

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

    public string SceneName;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<Renderer>();
        HealthSetGauge(1f);
        CriticalSetGauge(0f);

        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);

        respawnPosition = transform.position;
        originalColor = renderer.material.color;

        // シーンアンロード時のリソース解放のためのイベントリスナーを追加
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Update()
    {
        if (m_joyconL.GetButton(Joycon.Button.DPAD_UP))
        {
            action_c_1 = true;
        }
        if (m_joyconL.GetButton(Joycon.Button.DPAD_LEFT))
        {
            action_c_2 = true;
        }
        if (m_joyconL.GetButton(Joycon.Button.DPAD_UP) && m_joyconL.GetButton(Joycon.Button.DPAD_LEFT))
        {
            action_c_3 = true;
        }

        if (m_joyconL.GetButton(Joycon.Button.SR) && m_joyconL.GetButton(Joycon.Button.SL) && action_c_1 == true && action_c_2 == true && action_c_3 == true)
        {
            GuardClear(0.2f);
        }
            Movement();
        if (!action_dead)
        {
            Jump();
            action_guard = false;
            Guard();
            if (!action_guard)
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
            PlaySound(0);
            animator.SetTrigger("attack");
            Instantiate(bulletPrefab, shotPoint.position, transform.rotation);
            leftCoolTime = coolTime;
        }
    }

    void CriticalShot()
    {
        if (m_joyconL.GetAccel()[2] > 2)
        {
            PlaySound(4);
            animator.SetTrigger("attack");
            Instantiate(CriticalbulletPrefab, CriticalShotPoint.position, transform.rotation);
            leftCoolTime = coolTime;
            CriticalPoint = 0;
            CriticalSetGauge(0f);
            action_c_1 = false;
            action_c_2 = false;
            action_c_3 = false;
        }
    }

    void Jump()
    {
        Vector3 pos = rb.transform.position;
        //if (jump_cnt < 2)
        //{
            if (m_joyconL.GetButton(Joycon.Button.STICK))
            {
                PlaySound(5);
                animator.SetTrigger("jump");
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                jump_cnt++;
            }
        //}
        /*if (-2.65f < pos.y && pos.y < -2.50f)
        {
            jump_cnt = 0;
        }*/
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

    public void OnDamage(float damage)
{
    hp -= damage;
    TakeDamage(0.1f);
    ChangeColor();
    Invoke("ResetColor", 0.1f);
    PlaySound(2);
    if (hp <= 0)
    {
        stock--;

        if (stock > 0)
        {
            PlaySound(1);
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
                //PlaySound(6);
                //Invoke("DelayedLoad", 2f); // 3秒後にDelayedLoadメソッドを呼び出す
                Load();
        }
    }
}

private void DelayedLoad()
{
        
        Load();
}


    public void HealthSetGauge(float value)
    {
        healthImage.DOFillAmount(value, duration)
            .OnComplete(() =>
            {
                burnImage
                    .DOFillAmount(value, duration / 2f)
                    .SetDelay(0.5f);
            });

        Material material = healthImage.material;
        Debug.Log("使用しているシェーダー: " + material.shader.name);

        Debug.Log("マテリアルのプロパティ: " + material.GetTexture("_MainTex"));

        healthcurrentRate = value;
    }

    public void CriticalSetGauge(float value)
    {
        CriticalImage.DOFillAmount(value, duration);

        Material material = CriticalImage.material;
        Debug.Log("使用しているシェーダー: " + material.shader.name);

        Debug.Log("マテリアルのプロパティ: " + material.GetTexture("_MainTex"));

        criticalcurrentRate = value;
    }

    public void TakeDamage(float rate)
    {
        HealthSetGauge(healthcurrentRate - rate);
    }

    public void GuardClear(float rate)
    {
        PlaySound(3);
        CriticalPoint++;
        CriticalSetGauge(criticalcurrentRate + rate);
    }

    private IEnumerator Respawn(Rigidbody2D rb)
    {
        Debug.Log("リスポーン待機中...");

        Physics2D.gravity = Vector2.zero;
        action_dead = true;
        transform.position = respawnPosition;
        yield return new WaitForSeconds(respawnDelay);
        action_dead = false;
        Physics2D.gravity = new Vector2(0, -9.81f);

        Debug.Log("リスポーン完了");
    }

    public void Load()
    {
        SceneManager.LoadScene(SceneName);
    }

    private void OnSceneUnloaded(Scene current)
    {
        DOTween.KillAll();
        Resources.UnloadUnusedAssets();
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    //接触開始
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Dead")
        {
            hp = 1;
            OnDamage(1f);
        }
    }

    public void PlaySound(int n)
    {
        audioSources[n].Play();
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