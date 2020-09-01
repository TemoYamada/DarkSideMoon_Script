using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
	[SerializeField] Sprite deadSprite = null;
	public Color deadColor;

    public float pow = 250f;

	GameManager gameManager;		// ゲームマネージャー
	SimpleAnimation simpleAnimation;

	[HideInInspector] public bool isDead = false;

    Transform nameTextTrn = null;

	float preXPos = 0f;

    // 左右の向きに応じたスケール
    Vector3 rightDir = Vector3.one;
    Vector3 leftDir = new Vector3(-1, 1, 1);

	// 死体データ削除用キー
	string objectId = string.Empty;

	// ----------
	void Awake ()
	{
		GameObject gameManagerObj = GameObject.FindWithTag("GameController");
		if (gameManagerObj != null)
			gameManager = gameManagerObj.GetComponent<GameManager>();

        simpleAnimation = GetComponent<SimpleAnimation>();
        nameTextTrn = MyUtil.GetChild(this.transform, "NameText");

		preXPos = transform.position.x;
	}

	void Start()
	{
		if (isDead)
			return;

		// キャラ表示名をセット
		if (photonView != null)
		{
			nameTextTrn.GetComponent<TextMeshPro>().text = photonView.Owner.NickName;

			if (!photonView.IsMine)
			{
				// 自身の操作キャラ以外は、Operateを無効に
				GetComponent<PlayerOperate>().enabled = false;
			}
		}
		else
		{
			nameTextTrn.GetComponent<TextMeshPro>().text = PlayData.rankingName;
		}
	}

	private void Update()
	{
		// 左右の向き変更 （前回位置との差分で判定）
		if (transform.position.x > preXPos + 0.01f)
		{
            transform.localScale = rightDir;
            nameTextTrn.localScale = rightDir;
		}
        else if (transform.position.x + 0.01f < preXPos)
        {
            transform.localScale = leftDir;
            nameTextTrn.localScale = leftDir;
        }

		preXPos = transform.position.x;

		// 落下した場合はゲームオーバー
        if (transform.position.y < PlayData.deadYPos)
        {
			if (isDead)
	        	FallInDead();	// 死亡キャラの落下
			else
    	    	GameOver(true);	// 落下の場合
        }

		// Rを押したら、死体を残さずリスポーン
		if (!isDead && Input.GetKeyDown(KeyCode.R))
		{
    	    GameOver(true);	// 落下の場合
		}
	}
	
	// ------
	private void OnTriggerEnter2D(Collider2D other)
	{
		// プレイ中でなければ、衝突判定は行わない
		if (PlayData.gameMode != GameMode.Play)
			return;

		if (isDead)
			return;

		if (other.gameObject.tag == "Trap")
		{
			GameOver();
		}

		if (other.gameObject.tag == "Goal")
		{
			gameManager.GameClear();
		}

		if (other.gameObject.tag == "Enemy")
		{
			GameOver();
		}


		if (other.gameObject.tag == "Flag")
		{
			SoundManager.Play(SoundManager.SEClip.check);
			gameManager.SetRespawnPos(other.transform.position);
		}

		// ボスとの接触時
		if (other.gameObject.tag == "BossCollider")
		{
			GameOver();
		}

		if (other.gameObject.tag == "Orb")
		{
			// SE音
			SoundManager.Play(SoundManager.SEClip.bossHit);

			// ボスにダメージ
			GameObject bossObj = GameObject.FindGameObjectWithTag("Boss");
			if (bossObj != null)
				bossObj.GetComponent<BossManager>().MyBossDamage();

			// 自キャラを削除
			GameOver(true);
		}

		if (other.gameObject.tag == "BossCollider" || other.gameObject.tag == "Orb")
		{
			// ボスと当たった際は、確率でキャラを増やす
			int rnd = Random.Range(0, 2);
			if (rnd == 0)
			{
				gameManager.MakeClone();
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (isDead)
			return;

		if (other.gameObject.tag == "Enemy")
		{
			GameOver();
		}
		else if (other.gameObject.tag == "Trap")
		{
			GameOver();
		}
	}

	void FallInDead()
	{
		// 死亡済みでさらに落下した場合は、死体データから取り除く
		Debug.Log("DeleteDead: " + objectId);

		if (!PhotonNetwork.OfflineMode
		&& PlayData.gameMode == GameMode.Play)
		{
			// (同一ゲーム内で、死亡 → 落下のときは、Idを取得してないので削除できないが、
			// まあとりあえず目を瞑る)
			gameManager.DeleteDead(objectId);
		}

		Destroy(gameObject);
	}

	void GameOver(bool isDelete = false)
	{
		if (PlayData.gameMode != GameMode.Play)
			return;

		Debug.Log("GameOver");

		// 速度を止める
		MyUtil.StopMove(this.gameObject);

		if (isDead)
			return;

		isDead = true;

		if (isDelete)
		{
			// 死亡アニメーション (1回)
			simpleAnimation.Play("Die");
		}
		else
		{
			// ゲームオーバー 演出
			StartCoroutine(GameOverAnimation());

			// SE音
			SoundManager.Play(SoundManager.SEClip.gameOver);
		}

		if (photonView != null &&
			photonView.IsMine)
		{			
			// ゲームオーバー回数を加算
			PlayData.gameOverCount++;

			// // 死亡演出のGlitch
			// CameraPlay.Glitch(1f);

			if (isDelete)
			{
				// (死亡した上で落下した場合は除く)
				// シーンは再ロードせずに、新たなキャラクターを生む
				gameManager.MakeAlive();
			}
			else
			{
				// 死体データの格納
				gameManager.CorpseAddData(photonView.Owner.NickName, transform.position);

				// 今のキャラは、死亡状態にして放置する
				MyUtil.DelayCall(1f, ()=> 
				{
					// SetDead(string.Empty);
					// RPCで実行する
					photonView.RPC(nameof(RPCSetDead), RpcTarget.All);   

					// シーンは再ロードせずに、新たなキャラクターを生む
					gameManager.MakeAlive();
				});
			}
		}

		// 落下キャラは削除する
		if (isDelete)
		{
			// PhotonNetwork.Destroy(this.gameObject);
			// MyUtil.DelayCall(1f, ()=>PhotonNetwork.Destroy(this.gameObject));

			// Ownerからしか削除できないので、とりあえず普通で
			Destroy(gameObject);
		}
	}

	IEnumerator GameOverAnimation()
	{
		while(true)
		{
			// 死亡アニメーションの繰り返し
			simpleAnimation.Play("Die");
			yield return new WaitForSeconds(0.3f);
		}
	}
    // ----------
	// このキャラを死体に変更 （DB上のデータから作成する場合にも利用）
	[PunRPC]
	public void RPCSetDead()
	{
		MySetDead(string.Empty, string.Empty);
	}

	public void MySetDead(string name, string objectId_)
	{
		isDead = true;
		simpleAnimation.enabled = false;

		// ☆ やるかは悩ましい。
		// // 回転を有効
		// GetComponent<Rigidbody2D>().freezeRotation = false;

		// 死亡スプライトをセット
		MyUtil.SetSprite(this.gameObject, deadSprite);

		TextMeshPro nameTextPro = nameTextTrn.GetComponent<TextMeshPro>();

		// 名前をセット （あえて渡されている場合 = DB上のデータから作成する場合）
		if (string.IsNullOrEmpty(name) == false)
			nameTextPro.text = name;

		if (string.IsNullOrEmpty(objectId_) == false)
			objectId = objectId_;

		// 色を変更 （スプライトとテキスト）
		GetComponent<SpriteRenderer>().color = deadColor;
		// nameTextPro.color = deadColor;
	
		// Tagを変更
		this.tag = "Dead";

		// Layerを変更
		MyUtil.ChangeLayer(this.gameObject, "Dead");
	}

	public void AddUpForce()
	{
		if (photonView.ViewID > 0)
		{
			// RPCで実行する
			photonView.RPC(nameof(RPCAddUpForce), RpcTarget.All);   
		}
		else
			RPCAddUpForce();
	}

	[PunRPC]
	public void RPCAddUpForce()
	{
		GetComponent<Rigidbody2D>().AddForce(Vector2.up * pow);
	}
}
