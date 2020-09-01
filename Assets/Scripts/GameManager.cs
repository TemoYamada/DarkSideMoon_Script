using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using EaseTools;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MyMonoBehaviour
{
	public GameObject buttons;
	[SerializeField] GameObject playerPrefab = default;
	[SerializeField] CinemachineVirtualCamera vCamera = default;

	CorpseManager corpseManager;
    LogManager logManager;		// ログ用

	Vector2 respawnPos = new Vector2(-9, 1);

	[SerializeField] GameObject beamGate;

	[SerializeField] GameObject goalGate = default;

	[SerializeField] GameObject explosionPrefab;

	NetworkManager networkManager;
	
#if UNITY_EDITOR
	public bool isDebug = false;
	public Vector2[] debugRespawnPos;
	public int debugPosIndex = 0;
#endif

	// -----
	// Use this for initialization
	void Start ()
	{
#if UNITY_EDITOR
		if (isDebug)
			respawnPos = debugRespawnPos[debugPosIndex];
#endif
		GameObject NetworkManagerObj = GameObject.FindGameObjectWithTag("NetworkManager");
		networkManager = NetworkManagerObj.GetComponent<NetworkManager>();

        GameObject logManagerObj = GameObject.FindWithTag("LogManager");
		if (logManagerObj != null)
	        logManager = logManagerObj.GetComponent<LogManager>();

		PlayData.gameMode = GameMode.Play;

		// BGM
		BgmManager.Play("Threat");

		// ゴールの扉制御
		SetOnceCall(()=> PlayData.bossDied, ()=> {
			goalGate.SetActive(false);
			// BGMの変更
			BgmManager.Play("JumpAndShootMan");
		});

		PlayData.loadDB = new BoolNoticer(false);
		PlayData.loadDB.SetOnChange(true, _=> MakeDeadDBData());

		corpseManager = GameObject.FindWithTag("CorpseManager").GetComponent<CorpseManager>();
		corpseManager.LoadData();
	}
	
	protected override void DoUpdate () {
	}

	// -----
	void MakeDeadDBData()
	{
		// NCMBからの死体復元
		if (corpseManager != null && corpseManager.corpseList != null)
		{
			foreach(CorpseData corpseData in corpseManager.corpseList)
			{
				MakeDead(corpseData);
			}
		}
	}

	// ----------
	// 生成関連

	// 死体データのSingletonと、DBへの格納
	public void CorpseAddData(string name, Vector2 position)
	{
		corpseManager.AddData(name, position);
	}

	// DBデータからの死亡キャラの生成
	void MakeDead(CorpseData corpseData)
	{
		GameObject deadPlayer = Instantiate(playerPrefab, 
			corpseData.pos, playerPrefab.transform.rotation);

		PlayerManager playerManager = deadPlayer.GetComponent<PlayerManager>();
		playerManager.MySetDead(corpseData.name, corpseData.objectId);
	}

	// 生存キャラの生成（初回スタート時、再復活時とも）
	public void MakeAlive()
	{
		if (logManager != null)
		{
	        logManager.WriteLog("あらたな" + PhotonNetwork.NickName + "が月面に降り立った。");
		}

		// 生成
		GameObject newPlayer = PhotonNetwork.Instantiate(
			playerPrefab.name, respawnPos, Quaternion.identity);

		// Cinemachineのターゲットにセット
		vCamera.Follow = newPlayer.transform;
		vCamera.LookAt = newPlayer.transform;
	}

	public void MakeClone()
	{
		if (logManager != null)
		{
	        logManager.WriteLog(PhotonNetwork.NickName + "が、力を合わせるために駆けつけた！");
		}

		// 生成
		GameObject newPlayer = PhotonNetwork.Instantiate(
			playerPrefab.name, respawnPos, Quaternion.identity);

		// Cinemachineのターゲットにセット
		vCamera.Follow = newPlayer.transform;
		vCamera.LookAt = newPlayer.transform;
	}

	// DBデータからの死亡キャラの削除
	public void DeleteDead(string objectId)
	{
		if (string.IsNullOrEmpty(objectId) == false)
			corpseManager.DeleateData(objectId);
	}

	// ----------
	// 復活ポイントのセット
	public void SetRespawnPos(Vector2 pos)
	{
		// より右の復活ポイントを触った場合に、復活ポイントを更新
		if (pos.x > respawnPos.x)
		{
			respawnPos = pos;
		}
	}

	// ----------
	// ゲームオーバー、クリア関連

	public void GameClear()
	{
		if (PlayData.gameMode != GameMode.Play)
			return;

		PlayData.gameMode = GameMode.Clear;

		// Masterを他人に移譲する
		networkManager.OnButtonChangeMaster();

		// SE音
		SoundManager.Play(SoundManager.SEClip.pibo);

		// Debug.Log("Clear!");

		buttons.SetActive(false);

		StartCoroutine(SubGameClear());
	}

	IEnumerator SubGameClear()
	{
		yield return new WaitForSeconds(1f);

		// SE音
		SoundManager.Play(SoundManager.SEClip.clear);

		// カメラ振動エフェクト
    	CameraPlay.EarthQuakeShake(2f);

		yield return new WaitForSeconds(2f);

		SoundManager.Play(SoundManager.SEClip.bomb);

		// 爆発エフェクト
		Instantiate(explosionPrefab,
			goalGate.transform.position + Vector3.right * 3,
			explosionPrefab.transform.rotation);

		yield return new WaitForSeconds(2.5f);

		// 崩壊させる
		GameObject blockGrid = GameObject.Find("Grid/BlockTilemap2");
		blockGrid.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

		yield return new WaitForSeconds(1f);

		// 切断
		networkManager.PhotonDisConnect();

		yield return new WaitForSeconds(1f);

		// Glitchエフェクト
		CameraPlay.Glitch2(2f);

		yield return new WaitForSeconds(2f);

		// カメラノイズエフェクト
		CameraPlay.Noise(2f);

		yield return new WaitForSeconds(2f);

		// クリアシーンを表示
		// FadeManager.Instance.LoadScene("ClearScene", 1f);
		SceneManager.LoadScene("ClearScene");
	}
}
