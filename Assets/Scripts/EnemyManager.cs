using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	public LayerMask blockLayer;		// ブロックレイヤー

	private Rigidbody2D rbody;

	[SerializeField] float moveSpeed = 1;
	[SerializeField] float judgeHight = 0.2f;

	public enum MOVE_DIR
	{
		LEFT,
		RIGHT,
	}

	[SerializeField] MOVE_DIR moveDirection = MOVE_DIR.LEFT;	// 移動方向

	void Start () {
		rbody = GetComponent<Rigidbody2D>();		
	}
	
	// void Update () {
	// }

	void FixedUpdate () {
		bool isBlock;	// 進行方向にブロックがあるか否か

		switch(moveDirection)
		{
			case MOVE_DIR.LEFT:
				// 進行方向に速度をセット
				rbody.velocity = new Vector2(moveSpeed * -1, rbody.velocity.y);
				transform.localScale = new Vector2(1, 1);	// 画像を左向きにする

				// 前方への衝突判定
				isBlock = Physics2D.Linecast(
					// 前方の衝突判定（敵の背中から鼻先への線分がblockLayerに触れるか？）
					new Vector2(transform.position.x, transform.position.y + judgeHight),
					new Vector2(transform.position.x - judgeHight, transform.position.y + judgeHight),
					blockLayer);

				if (isBlock)
					moveDirection = MOVE_DIR.RIGHT;
				break;
			case MOVE_DIR.RIGHT:
				// 進行方向に速度をセット
				rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);
				transform.localScale = new Vector2(-1, 1);	// 画像を右向きにする

				// 前方への衝突判定
				isBlock = Physics2D.Linecast(
					new Vector2(transform.position.x, transform.position.y + judgeHight),
					new Vector2(transform.position.x + judgeHight, transform.position.y + judgeHight),
					blockLayer);

				if (isBlock)
					moveDirection = MOVE_DIR.LEFT;
				break;
		}
	}

	// public void DestroyEnemy()
	// {
	// 	Destroy(this.gameObject);
	// }

}
