//using System.Collections;
//using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayData {

	// public static double HiScore = 0;
	public static bool PlayBGM = true;
    public static bool PlaySE = true;
    public static string rankingName = "";
    
	public static GameMode gameMode = GameMode.Start;
    // ゲームオーバー回数
    public static int gameOverCount = 0;
    // public static int gameOverCount = 2;

    // 落下時の死亡高さ
    public static float deadYPos = -7f;

    // ボス生成済みか
    public static bool bossSpawned = false;

    // ボスを倒したか
    public static bool bossDied = false;

    // DBからデータを取得したかどうか
    public static BoolNoticer loadDB = new BoolNoticer(false);

    // DB保存する座標しきい値
    public static float dbThrMinX = 18f;

    // データ復元する数
    public static int restoreCount = 256;

    // // 必ず新規ルームを作成するか
    // public static bool createNewRoom = false;

    public static void Init()
    {
        // rankingName = string.Empty;
        gameMode = GameMode.Start;
        // createNewRoom = false;
        gameOverCount = 0;
        bossSpawned = false;
        bossDied = false;
    }

}
