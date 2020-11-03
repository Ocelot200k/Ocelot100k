using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    //インスペクターで設定する
    public float speed;//速度
    public float JumpSpeed;//ジャンプ速度
    public float jumpHeight; //ジャンプする高さ
    public float gravity;//重力
    public float jumpLimitTime;//ジャンプ制限
    public GroundCheck ground;//設置判定
    public GroundCheck head;//頭をぶつけた判定
    //プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGround = false;
    private bool isHead = false;
    private bool isJump = false;
    private float jumpPos = 0.0f;
    private float jumpTime = 0.0f;
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
        //接地判定を得る
        isGround = ground.IsGround(); //new
        isHead = head.IsGround();
        //キー入力されたら行動する
        float horizontalKey = Input.GetAxis("Horizontal");
        float verticalkey = Input.GetAxis("Vertical");
        float xSpeed = 0.0f;
        float ySpeed = -gravity;

        if (isGround)
        {
            if (verticalkey > 0)
            {
                ySpeed = JumpSpeed;
                jumpPos = transform.position.y;//ジャンプした位置を記録する
                isJump = true;
                jumpTime = 0.0f;
                Debug.Log("反応あり");
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
                Debug.Log("反応あり2");
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
                Debug.Log("反応あり3");
            }
        }
            if (horizontalKey > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                anim.SetBool("run", true);
                xSpeed = speed;
            Debug.Log("反応あり4");
        }
            else if (horizontalKey < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                anim.SetBool("run", true);
                xSpeed = -speed;
            Debug.Log("反応あり5");
        }
            else
            {
                anim.SetBool("run", false);
                xSpeed = 0.0f;
            Debug.Log("反応あり6");
        }
            anim.SetBool("jump", isJump);
            anim.SetBool("ground", isGround);
            rb.velocity = new Vector2(xSpeed, ySpeed);
        
    }
}