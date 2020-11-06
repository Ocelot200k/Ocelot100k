using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class player : MonoBehaviour
{
    #region//インスペクターで設定する
    [Header("移動速度")]public float speed;
    [Header("ジャンプ速度")] public float JumpSpeed;
    [Header("ジャンプする高さ")] public float jumpHeight; 
    [Header("重力")] public float gravity;
    [Header("ジャンプする長さ")] public float jumpLimitTime;
    [Header("接地判定")] public GroundCheck ground;
    [Header("天井判定")] public GroundCheck head;
    [Header("ダッシュの速さ表現")] public AnimationCurve dashCurve;
    [Header("ジャンプの速さ表現")] public AnimationCurve jumpCurve;
    #endregion
    #region//プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGround = false;
    private bool isHead = false;
    private bool isJump = false;
    private bool isRun = false;
    private bool isDown = false;
    private float jumpPos = 0.0f;
    private float dashTime = 0.0f;
    private float jumpTime = 0.0f;
    private float beforeKey = 0.0f;
    private string enemyTag = "Enemy";
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //コンポーネートのインスタンスを捕まえ
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       if (!isDown)
       {

        
        //接地判定を得る
        isGround = ground.IsGround();
        isHead = head.IsGround();
        //各種座標軸の速度を求める

        float ySpeed = GetYSpeed();
        float xSpeed = GetXSpees();

        //アニメーションを適用
        SetAnimation();

        //移動速度を設定
        rb.velocity = new Vector2(xSpeed, ySpeed);
       }
        else
        {
            rb.velocity = new Vector2(0, -gravity);
        }
    }
        /// <summary>
        /// Y成分で必要な計算をし、速度を返す
        /// </summary>
        /// <returns>Y軸の速さ</returns>
        private float GetYSpeed()
        {
            float verticalkey = Input.GetAxis("Vertical");
            float ySpeed = -gravity;
            if (isGround)
            {
                if (verticalkey > 0)
                {
                    ySpeed = JumpSpeed;
                    jumpPos = transform.position.y;//ジャンプした位置を記録する
                    isJump = true;
                    jumpTime = 0.0f;
                }
                else
                {
                    isJump = false;
                }
            }
            else if (isJump)
            {
                //上方向キーを押しているか
                bool pushUpKey = verticalkey > 0;
                //現在の高さが飛べる高さより下か
                bool canHeight = jumpPos + jumpHeight > transform.position.y;
                //ジャンプ時間が長くなりすぎていないか
                bool canTime = jumpLimitTime > jumpTime;
                if (pushUpKey && canHeight && canTime && !isHead)
                {
                    ySpeed = JumpSpeed;
                    jumpTime += Time.deltaTime;
                }
                else
                {
                    isJump = false;
                    jumpTime = 0.0f;
                }
            }

            //アニメーションカーブを速度に適用
            if (isJump)
            {
                ySpeed *= dashCurve.Evaluate(jumpTime);
            }

            return ySpeed;
        }
        
        /// <summary>
        /// X成分で必要な計算をし、速度を返す
        /// </summary>
        /// <returns>X軸の速さ</returns>
        private float GetXSpees()
        {
            float horizontalKey = Input.GetAxis("Horizontal");
            float xSpeed = 0.0f;
            if (horizontalKey > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                isRun = true;
                dashTime += Time.deltaTime;
                xSpeed = speed;
            }
            else if (horizontalKey < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                isRun = true;
                dashTime += Time.deltaTime;
                xSpeed = -speed;
            }
            else
            {
                isRun = false;
                xSpeed = 0.0f;
                dashTime = 0.0f;
            }
            //前回の入力からダッシュの判定を判断して速度を変える
            if (horizontalKey > 0 && beforeKey < 0)
            {
                dashTime = 0.0f;
            }
            else if (horizontalKey < 0 && beforeKey > 0)
            {
                dashTime = 0.0f;
            }
            beforeKey = horizontalKey;

            //アニメーションカーブを速度に適用
            xSpeed *= dashCurve.Evaluate(dashTime);

            return xSpeed;
        }

        ///<summary>
        ///アニメーションを設定する
        ///</summary>
        private void SetAnimation()
        {
            anim.SetBool("jump", isJump);
            anim.SetBool("ground", isGround);
            anim.SetBool("run", isRun);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
           if (collision.collider.tag == enemyTag)
           { 
              anim.Play("player_down");
              isDown = true;
           }
        }


}
