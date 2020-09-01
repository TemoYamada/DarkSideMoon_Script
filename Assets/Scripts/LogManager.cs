using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class LogManager : MonoBehaviour
{
    [SerializeField] Text infoText; // ログ表示用テキスト
    Queue<string> strQueue = new Queue<string>(3);  // ログ表示用

    // ----------
    // void Start()
    // {        
    // }

    // void Update()
    // {        
    // }

    // ----------
    // public void WriteLog(string strBuf)
    // {
    //     WriteLog(strBuf, Color.white);
    // }

    public void WriteLog(string strBuf, bool isRed = false)
    {
        // 現在時刻の追加
        StringBuilder buf = new StringBuilder();
        if (isRed)
        {
            buf.Append("<color=red");
            // buf.Append(color.ToString());
            buf.Append(">");
        }
        buf.Append("[" + MyUtil.GetNowTime() + "]");
        buf.Append(strBuf);
        if (isRed)
        {
            buf.Append("</color>");
        }
    
        strQueue.Enqueue(buf.ToString());

        // ログを3行書く
        if (strQueue.Count > 3)
            strQueue.Dequeue();

        infoText.text = string.Empty;

        int loop = 0;
        foreach(string strTemp in strQueue)
        {
            infoText.text += strTemp.ToString();
            if (loop >= 2)
                break;
            infoText.text += System.Environment.NewLine;
            loop ++;
        }
    }

}
