using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;

public class CorpseManager : SingletonMonoBehaviour<CorpseManager>
{
    public List<CorpseData> corpseList = null;

    // ----------
    void Start()
    {
        corpseList = new List<CorpseData>(PlayData.restoreCount);
    }

    // ----------
    public void AddData(string name, Vector2 pos)
    {
        // しきい値以下のデータは登録しない
        if (pos.x < PlayData.dbThrMinX)
        {
            return;
        }

        CorpseData corpseData = new CorpseData(name, pos);
        SaveData(corpseData);
    }

    void SaveData(CorpseData corpseData)
    {
        // CorpseDataというクラスを作成、サーバーに存在していなければ送信時に自動作成される。
        NCMBObject dbClass = new NCMBObject("CorpseData");

        dbClass["name"] = corpseData.name;
        dbClass["posx"] = corpseData.pos.x;
        dbClass["posy"] = corpseData.pos.y;
        dbClass["deadTime"] = corpseData.deadTime;

        //データ送信
        dbClass.SaveAsync();
    }

    // ---------
    public void LoadData()
    {
        PlayData.loadDB.Value = false;

        // DeadDataからデータを取得する
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("CorpseData");

        query.Limit = PlayData.restoreCount;
        query.OrderByDescending("updateDate");

        //データを検索し取得
        query.FindAsync ((List<NCMBObject> objectList,NCMBException e) =>
        {
            //取得失敗
            if(e != null){
                //エラーコード表示
                Debug.Log(e.ToString());
                return;
            }

            //取得した全データのmessageを表示
            foreach (NCMBObject ncmbObject in objectList)
            {
                CorpseData corpseData = new CorpseData(ncmbObject);

                // しきい値以下のデータは削除する
                if (corpseData.pos.x < PlayData.dbThrMinX)
                {
                    DeleateData(corpseData.objectId);
                    continue;
                }

                corpseList.Add(corpseData);
            }

            PlayData.loadDB.Value = true;
        });
    }

    // void DeleateData(CorpseData corpseData)
    // {
    //     // TestClassというクラスを作成、サーバーに存在していなければ送信時に自動作成される。
    //     NCMBObject dbClass = new NCMBObject("CorpseData");

    //     dbClass.ObjectId = corpseData.objectId;
    //     dbClass.DeleteAsync ();
    // }

    public void DeleateData(string objectId_)
    {
        NCMBObject dbClass = new NCMBObject("CorpseData");

        dbClass.ObjectId = objectId_;
        dbClass.DeleteAsync ();
    }

}
