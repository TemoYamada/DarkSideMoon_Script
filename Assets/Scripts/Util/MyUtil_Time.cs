using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MyUtil
{ 
	static bool isStartTimer = false;
	static float startTime = 0f;

	// 絶対時間算出時の基準時 （2020年以前はもう計測しない前提）
	static DateTime baseEpoch = new DateTime(2020, 1, 1, 0, 0, 0, 0);

    //-----------------------
	// タイマーのスタート
	public static void StartTimer()
	{
		isStartTimer = true;
		startTime = Time.time;
	}

	// タイマー値を返す
	public static float GetTimerTime()
	{
		if (isStartTimer)
			return Time.time - startTime;
		else
			return 0f;
	}

	// タイマーのリセット
	public static void ResetTimer()
	{
		isStartTimer = false;
	}

	//-----------------------
	// 現在時刻を絶対時間（秒）に換算したものを取得
    public static long GetNowAbsSeconds()
    {
        return GetAbsSeconds(System.DateTime.Now);
    }

	// 絶対時間（秒）を取得
    private static long GetAbsSeconds(DateTime dateTime)
    {
        return (long)(dateTime - baseEpoch).TotalSeconds;
    }

    // AbsSecondsからDateTimeへ変換
    private static DateTime GetDateTime(long absSeconds)
    {
        return baseEpoch.AddSeconds(absSeconds);
    }

	//-----------------------
	// 現在時刻を文字列にして返す
	public static string GetNowTime()
	{
		return System.DateTime.Now.ToString("HH:mm:ss");
	}
}
