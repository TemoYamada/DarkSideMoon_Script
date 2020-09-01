using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoolNoticer
{
    // 処理のデータクラス
    class CallData
    {
        public bool? trigger = null;
        public Func<bool> filter = null;
        public Action<bool> action = null;
    }

    // 処理のメンバー
    List<CallData> callList = new List<CallData>();
    short listCount = 0;

    // データアクセス用
    CallData callCache;

    [SerializeField] bool inFlag = false;

    public bool Value
    {
        get
        {
            return inFlag;
        }
        set
        {
            if (inFlag != value)
            {
                inFlag = value;
                ExecuteAction();
            }
        }
    }

    // -----
    // コンストラクタ
    public BoolNoticer(bool initialValue)
    {
        inFlag = initialValue;
    }

    // -----
    // boolのフラグ変更時に、行う処理を登録
	// 引数 trigger: true、false どちらがセットされた時に処理を行うか。どちらもであればnullセット
	// 引数 filterの記述例: () => OnResultBool()。 （引数なし、戻り値 bool）
	// または、() => (PlayData.gameMode == GameMode.Play)など。
	// 引数 actionの記述例:  x => OnDoAction(x)。 （引数bool、戻り値なし）
	public void SetOnChange(
		bool? trigger, Func<bool> filter, Action<bool> action)
	{
        CallData callData = new CallData();
        callData.trigger = trigger;
        callData.filter = filter;
        callData.action = action;

        callList.Add(callData);
        listCount++;
	}

    // boolのフラグ変更時に、行う処理を登録
	// 引数 trigger: true、false どちらがセットされた時に処理を行うか。どちらもであればnullセット
	// 引数 actionの記述例:  x => OnDoAction(x)。 （引数bool、戻り値なし）
	public void SetOnChange(
		bool? trigger, Action<bool> action)
	{
        SetOnChange(trigger, null, action);
	}

    // boolのフラグ変更時に、行う処理を登録
	// 引数 actionの記述例:  x => OnDoAction(x)。 （引数bool、戻り値なし）
	public void SetOnChange(Action<bool> action)
	{
        SetOnChange(null, null, action);
	}

    // -----
    void ExecuteAction()
    {
        if (listCount > 0)
        {
            for (short loop = 0; loop < listCount; loop++)
            {
                callCache = callList[loop];

                if (callCache.trigger == null
                || callCache.trigger.Value == inFlag)
                {
                     if (callCache.filter == null
                    || callCache.filter())
                    {
                        callCache.action(inFlag);
                    }
                }
            }
        }
    }
}
