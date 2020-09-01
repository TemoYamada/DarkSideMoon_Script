using System;
// using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyMonoBehaviour : MonoBehaviour
{
    // 処理のデータクラス
    class CallData
    {
        public Func<bool> filter = null;
        public Action action = null;
    }

    // 一定時間ごとに繰り返す処理のメンバー
    List<CallData> repeatCallList = new List<CallData>();
    List<float> repeatSpanList = new List<float>();  // 処理する間隔

    // (処理間隔を変更する機能は、MySpawnerに移行し、ここではやらない方向に変更)
    // List<float?> changeDurationList = new List<float?>();
    // List<float?> limitList = new List<float?>();
    List<float> repeatDeltaList = new List<float>();  // 間隔カウント用
    short repeatCount = 0;

    // 一度のみ行う処理のメンバー
    List<CallData> onceCallList = new List<CallData>();
    List<bool> onceDoneList = new List<bool>(); // 処理したかどうか
    short onceCount = 0;

    // データアクセス用
    CallData callCache;

    // -----
    // 派生先では、Update()の代わりに、protected override void DoUpdate()を使用する。
    void Update()
    {
        // 一定時間(duration)ごとにfilter受け付けをして処理を繰り返す
        if (repeatCount > 0)
        {
            for (short loop = 0; loop < repeatCount; loop++)
            {
                repeatDeltaList[loop] += Time.deltaTime;

                if (repeatDeltaList[loop] > repeatSpanList[loop])
                {
                    callCache = repeatCallList[loop];

                    if (callCache.filter == null
                    || callCache.filter())
                    {
                        callCache.action();
                        repeatDeltaList[loop] = 0f;

                        // // 処理間隔を変更する設定の場合
                        // float? changeDuration = changeDurationList[loop];
                        // if (changeDuration != null)
                        // {
                        //     repeatSpanList[loop] += changeDuration.Value;

                        //     if (changeDuration.Value > 0)
                        //     {
                        //         if (repeatSpanList[loop] > limitList[loop])
                        //         {
                        //             repeatSpanList[loop] = limitList[loop].Value;
                        //         }
                        //     }
                        //     else if (changeDuration.Value < 0)
                        //     {
                        //         if (repeatSpanList[loop] < limitList[loop])
                        //         {
                        //             repeatSpanList[loop] = limitList[loop].Value;
                        //         }
                        //     }

                        // }
                    }
                }
            }
        }

        // 一度のみ受け付ける処理
        if (onceCount > 0)
        {
            for (short loop = 0; loop < onceCount; loop++)
            {
                callCache = onceCallList[loop];

                if (callCache.filter == null
                || callCache.filter())
                {
                    if (onceDoneList[loop] == false)
                    {
                        callCache.action();
                        onceDoneList[loop] = true;
                    }
                }
            }
        }

        // 派生クラス独自の処理を呼び出し
        DoUpdate();
    }

    // 参考: https://baba-s.hatenablog.com/entry/2014/02/03/101733
    protected abstract void DoUpdate();

    // -----
    // 以下、Start()メソッドからの呼び出し前提

    // 一定時間(duration)ごとに処理を繰り返す
	// 引数 actionの記述例:  () => OnDoRepeat()。 （引数なし、戻り値なし）
    protected void SetRepeatCall(float duration, Action action)
    {
        SetRepeatCall(null, duration, action);
    }

    // 一定時間(duration)ごとにfilterによる呼び出し受け付けを繰り返す
	// 引数 filterの記述例: () => OnResultBool()。 （引数なし、戻り値 bool）
	// または、() => (PlayData.gameMode == GameMode.Play)など。
	// 引数 actionの記述例:  () => OnDoRepeat()。 （引数なし、戻り値なし）
    protected void SetRepeatCall(Func<bool> filter, float duration, Action action)
    {
        CallData callData = new CallData();
        callData.filter = filter;
        callData.action = action;

        repeatCallList.Add(callData);
        repeatSpanList.Add(duration);
        repeatDeltaList.Add(duration);  // 初回はすぐに実行可能とするため、duration経過扱いとする

        repeatCount++;

        // SetRepeatCall(filter, duration, null, null, action);
    }

    // // 一定時間(duration)ごとにfilterによる呼び出し受け付けを繰り返す
	// // 引数 filterの記述例: () => OnResultBool()。 （引数なし、戻り値 bool）
	// // または、() => (PlayData.gameMode == GameMode.Play)など。
	// // 引数 actionの記述例:  () => OnDoRepeat()。 （引数なし、戻り値なし）
    // protected void SetRepeatCall(Func<bool> filter,
    //     float duration, float? changeDuration, float? limit,
    //     Action action)
    // {
    //     CallData callData = new CallData();
    //     callData.filter = filter;
    //     callData.action = action;

    //     repeatCallList.Add(callData);
    //     repeatSpanList.Add(duration);
    //     changeDurationList.Add(changeDuration);
    //     limitList.Add(limit);
    //     repeatDeltaList.Add(duration);  // 初回はすぐに実行可能とするため、duration経過扱いとする

    //     repeatCount++;
    // }

    // 一度のみ処理を受け付ける （例: タイトル画面でクリックされたら実行など）
	// 引数 filter: () => OnResultBool()。 （引数なし、戻り値 bool）
	// または、() => (Input.GetKeyDown(KeyCode.R))など。 ← Rボタンを押されたらリセットなど
	// 引数 actionの記述例:  () => OnDoOnce()。 （引数なし、戻り値なし）
    protected void SetOnceCall(Func<bool> filter, Action action)
    {
        CallData callData = new CallData();
        callData.filter = filter;
        callData.action = action;

        onceCallList.Add(callData);
        onceDoneList.Add(false);

        onceCount++;
    }
}
