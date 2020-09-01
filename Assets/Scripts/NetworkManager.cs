using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

// MonoBehaviourPunCallbacksを継承して、Photonのコールバックを受け取る
public class NetworkManager : MonoBehaviourPunCallbacks
{
#if UNITY_EDITOR
    [SerializeField] bool offlineMode = false; // オフラインモード
#endif
   
    LogManager logManager;

    bool isLeave = false;    // ログアウトの意図かどうか

    bool isChangeMaster = false;

    // -----
    void Start()
    {
        logManager = GameObject.FindWithTag("LogManager").GetComponent<LogManager>();

        if (string.IsNullOrEmpty(PlayData.rankingName))
        {
            // ニックネームセット （仮）
            PhotonNetwork.NickName = MyUtil.GetNowAbsSeconds().ToString();
        }
        else
        {
            // ニックネームセット
		    PhotonNetwork.NickName = PlayData.rankingName;
        }

        PhotonNetwork.OfflineMode = false;

#if UNITY_EDITOR
        // デバッグ用オフラインモード
        if (offlineMode)
        {
            PhotonNetwork.OfflineMode = true;
            logManager.WriteLog("-- オフラインモード --", true);
        }
        else
#endif

        {
            // PUNに接続
            PhotonConnect();
        }
    }

    // void Update()
    // {        
    // }

    // -----
    // PUNに接続
    void PhotonConnect()
    {
        isLeave = false;

        // PhotonServerSettingsに設定した内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // PUN切断
    public void PhotonDisConnect()
    {
        PhotonNetwork.Disconnect();
    }

    // -----
    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // LeaveRoom後にもまた、ここに来ることに注意
        // → つまりログアウトの意図であれば、次のJoinRandomRoomはしないようにする
        if (isLeave)
            return;

        logManager.WriteLog("ドッキング完了！ これより月面着陸に入る。");

        // if (PlayData.createNewRoom)
        //     OnJoinRandomFailed(0, string.Empty); // 必ず新規ルーム作成
        // else
            PhotonNetwork.JoinRandomRoom();     // ランダムにルームに入室する
    }

    // ランダムなルームの入室に失敗した場合
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        logManager.WriteLog("他に探検者はいないようだ...");

        // ルームがないと入室に失敗するため、その時は自分で作る。引数はルーム名
        PhotonNetwork.CreateRoom(MyUtil.GetNowAbsSeconds().ToString());
    }

    // マッチングが成功し、ルームに入室後に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = true;

        // 自分以外のプレイヤーを取得
        Player[] players = PhotonNetwork.PlayerListOthers;
        if (players != null)
        {
            foreach(Player player in players)
            {
                logManager.WriteLog("すでに" + player.NickName + "がいるようだ。", true);
            }
        }

        // -- マッチング後、Playerを生成する -- 
        GameObject gmObj = GameObject.FindWithTag("GameController");
        gmObj.GetComponent<GameManager>().MakeAlive();
    }

    // -- ここら辺の失敗は、接続数オーバーなどが原因となるか? ---
    // ルームの作成に失敗した場合
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        OffLineByJoinFaild(message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        OffLineByJoinFaild(message);
    }

    void OffLineByJoinFaild(string message)
    {
        logManager.WriteLog(message, true);
        PhotonDisConnect();

        PhotonNetwork.OfflineMode = true;
        logManager.WriteLog("通信が途絶えた。救援は望めないようだ...");
        logManager.WriteLog("-- オフラインモード --", true);
    }

    // -----
    // 他プレイヤーが参加した時に呼ばれるコールバック
    public override void OnPlayerEnteredRoom(Player player)
    {
        logManager.WriteLog("あらたな" + player.NickName + "が月面に降り立った。", true);
    }

    // 他プレイヤーが退出した時に呼ばれるコールバック
    public override void OnPlayerLeftRoom(Player player)
    {
        logManager.WriteLog(player.NickName + "が月面から離脱した。", true);
    }

    // 新しいMasterへ変更時の処理
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        logManager.WriteLog(newMasterClient.NickName + "が月面の覇者になった。");
    }

    // ----------
    // Masterを他人に移譲する
    public void OnButtonChangeMaster()
    {
        // 自身がMaster時のみ、移譲可能
        if (PhotonNetwork.IsMasterClient)
        {
            // 自分以外のプレイヤーを取得
            Player[] players = PhotonNetwork.PlayerListOthers;

            if (players == null || players.Length == 0)
            {
                isChangeMaster = true;
                return;
            }

            foreach(Player player in players)
            {
                isChangeMaster = PhotonNetwork.SetMasterClient(player);
                if (isChangeMaster)
                    break;
            }
        }

        // (最終的にとりあえるtrueにする)
        isChangeMaster = true;
    }

    // ----------
    public void OnButtonChangeOption()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 部屋への参加を禁止する。（例: ボス戦に突入したので）
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    // PUNから去り、タイトルへ戻る
    public void OnButtonLeave()
    {
        if (isLeave)
            return;

        isLeave = true;

        // マスターを変更する
        OnButtonChangeMaster();

        StartCoroutine(SubLeave());       
    }

    IEnumerator SubLeave()
    {
        logManager.WriteLog(PhotonNetwork.NickName + "は離脱準備。3...");
        yield return new WaitForSeconds(1f);
        logManager.WriteLog("2...");
        yield return new WaitForSeconds(1f);
        logManager.WriteLog("1...");

        // マスター変更まで待つ
        yield return new WaitUntil(()=> isChangeMaster);

        // 切断
        PhotonDisConnect();

        // 必要に応じてシーン移動（タイトルに戻るなど）
		FadeManager.Instance.LoadScene("StartScene", 0.2f);
    }
}
