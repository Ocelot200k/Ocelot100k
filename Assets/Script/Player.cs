using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    #region//インスペクターで設定する
    [Header("速度")] public float speed;
    [Header("重力")] public float gravity; 
    [Header("ジャンプする速度")] public float jumpSpeed;
    [Header("高さ制限")] public float jumpHeight;
    [Header("ジャンプ制限時間")] public float jumpLimitTime;
    [Header("踏みつけ判定の高さの割合")] public float stepOnRate;
    [Header("接地判定")] public GroundCheck ground; 
    [Header("頭ぶつけた判定")] public GroundCheck head;
    [Header("ダッシュの速さ表現")] public AnimationCurve dashCurve;
    [Header("ジャンプの速さ表現")] public AnimationCurve jumpCurve;
    #endregion
    #region//プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private CapsuleCollider2D capcol = null;
    private bool isGround = false;
    private bool isJump = false;
    private bool isHead = false;
    private bool isRun = false;
    private bool isDown = false;
    private bool isOtherJump = false;
    private float jumpPos = 0.0f;
    private float otherJumpHeight = 0.0f;
    private float dashTime, jumpTime; 
    private float beforeKey;
    private string enemyTag = "Enemy";
    #endregion

    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capcol = GetComponent<CapsuleCollider2D>();
    }

    void FixedUpdate()
    {
        if (!isDown)
        {


            //接地判定を得る
            isGround = ground.IsGround();
            isHead = head.IsGround();

            //キー入力されたら行動する

            float ySpeed = GetYSpeed();
            float xSpeed = GetXSpeed();

            SetAnimation();


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
        float verticalKey = Input.GetAxis("Vertical");
        float ySpeed = -gravity;

        if (isOtherJump)
        {
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + otherJumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;

            if (canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isOtherJump = false;
                jumpTime = 0.0f;
            }
        }
        else if (isGround)
        {
            if (verticalKey > 0)
            {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y; //ジャンプした位置を記録する
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
            bool pushUpKey = verticalKey > 0;
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;

            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
            }
        }
            if (isJump || isOtherJump)
            {
                ySpeed *= jumpCurve.Evaluate(jumpTime);
            }

            return ySpeed;
        

    }
    /// <summary>
    /// X成分で必要な計算をし、速度を返す。
    /// </summary>
    /// <returns>X軸の速さ</returns>
    private float GetXSpeed()
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

            //前回の入力からダッシュの反転を判断して速度を変える New
            if (horizontalKey > 0 && beforeKey < 0)
            {
                dashTime = 0.0f;
            }
            else if (horizontalKey < 0 && beforeKey > 0)
            {
                dashTime = 0.0f;
            }

            beforeKey = horizontalKey;

            xSpeed *= dashCurve.Evaluate(dashTime);
        return xSpeed;


    }

    private void SetAnimation()
    {
        anim.SetBool("jump", isJump || isOtherJump);
        anim.SetBool("ground", isGround);
        anim.SetBool("run", isRun);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == enemyTag)
        {
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));
            float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;
            foreach (ContactPoint2D p in collision.contacts)
            {

                if (p.point.y < judgePos)
                {
                    ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                    if (o != null)
                    {
                        otherJumpHeight = o.boundHeight;
                        o.playerStepOn = true;
                        jumpPos = transform.position.y;
                        isOtherJump = true;
                        isJump = false;
                        jumpTime = 0.0f;
                    }
                    else
                    {
                        Debug.Log("ついてない");
                    }
                }
                else
                {
                    anim.Play("player_down");
                    isDown = true;
                    break;
                }
            }

        }
    }
}