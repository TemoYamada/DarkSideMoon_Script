using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;

public class CorpseData
{
    public string name;
    public Vector2 pos;
    public long deadTime;
    public string objectId;

    // ----------
    // Insertデータ作成用
    public CorpseData(string name_, Vector2 pos_)
    {
        name = name_;
        pos = pos_;
        deadTime = MyUtil.GetNowAbsSeconds();
        objectId = string.Empty;
    }

    // データ復元時用
    public CorpseData(NCMBObject ncmbObject)
    {
        name = ncmbObject["name"].ToString();

        float posx = 0f;
        float posy = 0f;
        float.TryParse(ncmbObject["posx"].ToString(), out posx);
        float.TryParse(ncmbObject["posy"].ToString(), out posy);
        pos = new Vector2(posx, posy);

        long deadT_;
        long.TryParse(ncmbObject["deadTime"].ToString(), out deadT_);
        deadTime = deadT_;

        objectId = ncmbObject.ObjectId;
    }
}
