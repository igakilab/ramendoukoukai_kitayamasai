using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;      //シーンの切り替えに必要

public class right_PlayerManager : MonoBehaviour
{
    public float moveSpeed = 3f; // 移動速度
    public float attackRadius; // 攻撃範囲
    public Transform highShotPoint; // 高い位置のショットポイント
    public Transform lowShotPoint; // 低い位置のショットポイント
    public Transform shotPoint; // ショットポイント
    public GameObject deathEffectPrefab; // 死亡エフェクトのプレハブ
    public GameObject GuardObject; // ガードオブジェクト
    public GameObject bulletPrefab; // 弾のプレハブ
    public float jumpForce = 5f; // ジャンプ力
    public float jump_cnt = 0; // ジャンプ回数
    public bool isRight; // 右向きかどうか
    public float hp = 10; // ヒットポイント
    public float coolTime = 0.3f; //待機時間
    public float leftCoolTime; //待機している時間
    // HpGauge関連のフィールド
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

    public string SceneName;    //読み込むシーン名

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dを取得
        animator = GetComponent<Animator>(); // Animatorを取得
        HealthSetGauge(1f); // HpGaugeの初期化
        CriticalSetGauge(0f);
    }

    private void Update()
    {
        //Movement(); // 移動処理
        Shot(); // ショット処理
        //Jump(); // ジャンプ処理
        Guard(); // ガード処理
    }

    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal"); // 方向キー横の入力を取得
        if (!isRight && x > 0)
        {
            transform.Rotate(0f, 180f, 0f); // 右向きに回転
            isRight = true;
        }
        if (isRight && x < 0)
        {
            transform.Rotate(0f, 180f, 0f); // 左向きに回転
            isRight = false;
        }
        animator.SetFloat("Speed", Mathf.Abs(x)); // アニメーションの速度を設定
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y); // 移動速度を設定
    }

    void Shot()
    {
        leftCoolTime -= Time.deltaTime;
        float flag = 0;
        if (Input.GetKeyDown(KeyCode.N) && Input.GetKey("up"))
        {
            shotPoint = highShotPoint; // 高い位置からショット
            flag = 1;
        }
        if (Input.GetKeyDown(KeyCode.N) && Input.GetKey("down"))
        {
            shotPoint = lowShotPoint; // 低い位置からショット
            flag = 1;
        }
        if (flag == 1 && leftCoolTime <= 0)
        {
            animator.SetTrigger("attack"); // 攻撃アニメーションを再生
            Instantiate(bulletPrefab, shotPoint.position, transform.rotation); // 弾を生成
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
                animator.SetTrigger("jump"); // ジャンプアニメーションを再生
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // ジャンプ力を加える
                jump_cnt++;
            }
        }
        if (-2.65f < pos.y && pos.y < -2.50f)
        {
            jump_cnt = 0; // ジャンプ回数をリセット
        }
    }

    void Guard()
    {
        float flag = 0;

        if (Input.GetKey(KeyCode.V) && Input.GetKey("up"))
        {
            shotPoint = highShotPoint; // 高い位置でガード
            flag = 1;
        }
        if (Input.GetKey(KeyCode.V) && Input.GetKey("down"))
        {
            shotPoint = lowShotPoint; // 低い位置でガード
            flag = 1;
        }

        if (flag == 1)
        {
            animator.SetTrigger("guard"); // ガードアニメーションを再生
            Instantiate(GuardObject, shotPoint.position, transform.rotation); // ガードオブジェクトを生成
        }
    }

    public void OnDamage()
    {
        Debug.Log("ダメージを受けた"); // デバッグログを追加
        hp -= 1; // ヒットポイントを減少
        TakeDamage(0.1f); // HpGaugeの更新
        if (hp <= 0)
        {
            Instantiate(deathEffectPrefab, transform.position, transform.rotation); // 死亡エフェクトを生成
            Destroy(gameObject); // プレイヤーオブジェクトを破壊
            Load();
        }
    }

    // HpGauge関連のメソッド
    public void HealthSetGauge(float value)
    {
        //DoTweenを連結して動かす
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

        healthcurrentRate = value;
    }
    
    public void CriticalSetGauge(float value)
    {
        //DoTweenを連結して動かす
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

    //シーンを読み込む
    public void Load()
    {
        SceneManager.LoadScene(SceneName);
    }

}
