using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;      //シーンの切り替えに必要

public class left_PlayerManager : MonoBehaviour
{
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    public float moveSpeed = 3f; // 移動速度
    public AudioSource seAudioSource;
    public float attackRadius; // 攻撃範囲
    public Transform highShotPoint; // 高い位置のショットポイント
    public Transform lowShotPoint; // 低い位置のショットポイント
    public Transform CriticalShotPoint; // 低い位置のショットポイント
    public Transform shotPoint; // ショットポイント
    public GameObject deathEffectPrefab; // 死亡エフェクトのプレハブ
    public GameObject GuardObject; // ガードオブジェクト
    public GameObject bulletPrefab; // 弾のプレハブ
    public GameObject CriticalbulletPrefab; // 弾のプレハブ
    public GameObject[] stockObject;
    public float jumpForce = 5f; // ジャンプ力
    public float jump_cnt = 0; // ジャンプ回数
    public bool isRight; // 右向きかどうか
    public float hp = 10; // ヒットポイント
    public int CriticalPoint = 0;
    public int i = 0;
    public int cnt = 0;
    public float coolTime = 0.3f; //待機時間
    public float leftCoolTime; //待機している時間
    public float forceAmount = 5f; // ふっとばす力の強さ
    public Vector2 direction; // ふっとばす方向
    public int stock = 3; // キャラクターの残機（ストック）
    public Vector3 respawnPosition; // リスポーンする場所
    public float respawnDelay = 3f; // リスポーンするまでの待機時間
    public bool action_dead = false;
    public bool action_guard = false;
    // 元の色を保存するための変数
    private Color originalColor;

    // 変更する色
    public Color newColor = Color.red;

    private Renderer renderer;


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
        renderer = GetComponent<Renderer>();
        HealthSetGauge(1f); // HpGaugeの初期化
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
        JoyControll();
        Movement(); // 移動処理
        if(!action_dead)
        {
            Jump(); // ジャンプ処理
            action_guard = false;
            Guard(); // ガード処理
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


    void Movement_1()
    {
        float x = Input.GetAxis("JoystickHorizontal"); // 方向キー横の入力を取得
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

    void Movement()
    {
        // Joy-Conのスティック入力を取得
        float x = 0f;
        //if (m_joyconL != null)
        //{
        //    x += m_joyconL.GetStick()[0]; // 左Joy-Conの横軸入力を取得
        //}
        if (m_joyconL != null)
        {
            x += m_joyconL.GetStick()[0]; // 右Joy-Conの横軸入力を取得
        }

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
        if (!action_dead)
        {
            animator.SetFloat("Speed", Mathf.Abs(x)); // アニメーションの速度を設定
        }
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y); // 移動速度を設定
    }

    void Shot()
    {
        leftCoolTime -= Time.deltaTime;
        float flag = 0;
        if (m_joyconL.GetButton(Joycon.Button.DPAD_RIGHT))
        {
            shotPoint = highShotPoint; // 高い位置からショット
            flag = 1;
        }
        if (m_joyconL.GetButton(Joycon.Button.DPAD_DOWN))
        {
            shotPoint = lowShotPoint; // 低い位置からショット
            flag = 1;
        }
        if (flag == 1 && leftCoolTime <= 0)
        {
            PlaySound();
            animator.SetTrigger("attack"); // 攻撃アニメーションを再生
            Instantiate(bulletPrefab, shotPoint.position, transform.rotation); // 弾を生成
            leftCoolTime = coolTime;
        }
    }

    void CriticalShot()
    {
        if (m_joyconL.GetAccel()[0] > 2)
        {
            animator.SetTrigger("attack"); // 攻撃アニメーションを再生
            int j;
            for(j = 0; j < 10; j++)
            {
                Instantiate(CriticalbulletPrefab, CriticalShotPoint.position, transform.rotation); // 弾を生成
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

        if (m_joyconL.GetButton(Joycon.Button.SHOULDER_1))
        {
            shotPoint = highShotPoint; // 高い位置でガード
            flag = 1;
        }
        if (m_joyconL.GetButton(Joycon.Button.SHOULDER_2))
        {
            shotPoint = lowShotPoint; // 低い位置でガード
            flag = 1;
        }

        if (flag == 1)
        {
            action_guard = true;
            animator.SetTrigger("guard"); // ガードアニメーションを再生
            Instantiate(GuardObject, shotPoint.position, transform.rotation); // ガードオブジェクトを生成
        }
    }

    public void OnDamage()
    {
        
        Debug.Log("ダメージを受けた"); // デバッグログを追加
        hp -= 1; // ヒットポイントを減少
        TakeDamage(0.1f); // HpGaugeの更新
        ChangeColor();
        Invoke("ResetColor", 0.1f);
        
        //Fling(new Vector2(-1, 1));
        if (hp <= 0)
        {
            stock--; // 残機を減少

            if (stock > 0)
            {
                hp = 10f;
                CriticalPoint = 0;
                HealthSetGauge(1f); // HpGaugeの初期化
                CriticalSetGauge(0f);
                // プレイヤーをリスポーン位置に移動
                Destroy(stockObject[i]);
                i++;
                rb.velocity = Vector3.zero;
                StartCoroutine(Respawn(rb));

            }
            else
            {
                Instantiate(deathEffectPrefab, transform.position, transform.rotation); // 死亡エフェクトを生成
                Destroy(gameObject); // プレイヤーオブジェクトを破壊
                Debug.Log("hogehoge");
                Load();
            }
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

        // シェーダーの確認
        Material material = healthImage.material;
        Debug.Log("使用しているシェーダー: " + material.shader.name);

        // マテリアルのプロパティ確認
        Debug.Log("マテリアルのプロパティ: " + material.GetTexture("_MainTex"));

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

        // シェーダーの確認
        Material material = CriticalImage.material;
        Debug.Log("使用しているシェーダー: " + material.shader.name);

        // マテリアルのプロパティ確認
        Debug.Log("マテリアルのプロパティ: " + material.GetTexture("_MainTex"));

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


    /*private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 24;

        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) が接続されていません");
            return;
        }

        GUILayout.BeginHorizontal(GUILayout.Width(960));

        foreach (var joycon in m_joycons)
        {
            var isLeft = joycon.isLeft;
            var name = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";
            var key = isLeft ? "Z キー" : "X キー";
            var button = isLeft ? m_pressedButtonL : m_pressedButtonR;
            var stick = joycon.GetStick();
            var gyro = joycon.GetGyro();
            var accel = joycon.GetAccel();
            var orientation = joycon.GetVector();

            GUILayout.BeginVertical(GUILayout.Width(480));
            GUILayout.Label(name);
            GUILayout.Label(key + "：振動");
            GUILayout.Label("押されているボタン：" + button);
            GUILayout.Label(string.Format("スティック：({0}, {1})", stick[0], stick[1]));
            GUILayout.Label("ジャイロ：" + gyro);
            GUILayout.Label("加速度：" + accel);
            GUILayout.Label("傾き：" + orientation);
            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();
    }*/

    public void Fling(Vector2 direction)
    {
        if (rb != null)
        {
            // 力を加える（2Dのワールド座標に基づく）
            Debug.Log("吹っ飛ばす方向: " + direction);

            animator.SetTrigger("addforce");
            rb.AddForce(direction.normalized * 10f, ForceMode2D.Impulse);
        }
    }

    private IEnumerator Respawn(Rigidbody2D rb)
    {
        Debug.Log("リスポーン待機中...");
        
        Physics2D.gravity = Vector2.zero;
        // 数秒間待機
        action_dead = true;
        transform.position = respawnPosition;
        yield return new WaitForSeconds(respawnDelay);
        action_dead = false;
        Physics2D.gravity = new Vector2(0, -9.81f);

        Debug.Log("リスポーン完了");
    }

    //シーンを読み込む
    public void Load()
    {
        SceneManager.LoadScene(SceneName);
    }

    //接触開始
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

    // 元の色に戻すメソッド
    public void ResetColor()
    {
        if (renderer != null)
        {
            renderer.material.color = originalColor;
        }
    }

}