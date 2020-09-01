using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOperate : MonoBehaviour
{
    [SerializeField] DetectTouchController detectTouchController = null;
    // Transform nameTextTrn = null;
	LayerMask blockLayer = default;	// ブロックレイヤー
	Rigidbody2D rbody;
    public float jumpPower = 400;	// ジャンプの力
	bool goJump = false;	// ジャンプしたか否か

    Vector2 move = Vector2.zero;    // 入力移動量

    public float acceleration = 100;    // 移動加速力

	// ブロックに接地しているか否か
    BoolNoticer canJump = new BoolNoticer(false);

    // アニメーション制御
    SimpleAnimation simpleAnimation = default;

    PlayerManager playerManager;

    // ----------
    void Start()
    {
        blockLayer = LayerMask.GetMask(new string[] { "Block" });

        rbody = GetComponent<Rigidbody2D>();
        simpleAnimation = GetComponent<SimpleAnimation>();
        // nameTextTrn = MyUtil.GetChild(this.transform, "NameText");

        playerManager = GetComponent<PlayerManager>();

        // 値の変更を監視: false → trueに戻った時に、通常のアニメーションに戻る
        canJump.SetOnChange(true, _ => simpleAnimation.Play("Default"));
    }

    // Update is called once per frame
    void Update()
    {
        canJump.Value = detectTouchController.isTouching;

        if (playerManager.isDead)
            return;

        // 仮 PlayerOperate　（入力値の受け取り）
        if (this.CompareTag("Player"))
        {
            // 左右移動
            float moveDx = Input.GetAxis("Horizontal");
            Move(moveDx);

            // ジャンプ
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
                Jump();
        }
    }

    void FixedUpdate ()
    {
        if (rbody == null)
            return;

        if (playerManager.isDead)
            return;

		// ジャンプ処理
		if (goJump)
		{
            simpleAnimation.Play("Jump");
            // SE音
    		SoundManager.Play(SoundManager.SEClip.jump);

            // 上にジャンプさせる
			rbody.AddForce (Vector2.up * jumpPower);
			goJump = false;
		}

        // 左右移動
        rbody.velocity = new Vector2(
            move.x * acceleration * Time.fixedDeltaTime, 
            rbody.velocity.y);
        
        // // 左右の向き変更
        // if (move.x > 0.1f)
        // {
        //     transform.localScale = rightDir;
        //     nameTextTrn.localScale = rightDir;
        // }
        // else if (move.x < -0.1f)
        // {
        //     transform.localScale = leftDir;
        //     nameTextTrn.localScale = leftDir;
        // }
	}

    // -----
    public void Jump()
    {
		if (canJump.Value)
			goJump = true;
    }

    public void Move(float dx)
    {
        move.x = dx;
    }

}
