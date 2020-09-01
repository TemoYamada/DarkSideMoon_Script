using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Text;

public class ClearController : MyMonoBehaviour
{
    [SerializeField] GameObject clearText2 = default;
    [SerializeField] GameObject clearText3 = default;

    bool isStart = false;

    // ----------
    IEnumerator Start()
    {
        clearText2.SetActive(false);
        clearText3.SetActive(false);

        StringBuilder sr = new StringBuilder();
        sr.AppendLine("地球に住む" + PlayData.rankingName + "は、");
        sr.AppendLine("ただじっと黙って");
        sr.AppendLine("夜空を見上げるのだった。");

        clearText2.GetComponent<Text>().text = sr.ToString();

        yield return new WaitForSeconds(8f);

        clearText2.SetActive(true);

        yield return new WaitForSeconds(8f);

        clearText3.SetActive(true);
    }

    protected override void DoUpdate()
    {
        // if (Input.GetKeyDown(KeyCode.Return))
        // {
        //     Debug.Log("Enter!");
        // }
    }

    // -----
    // ボタンイベント
    public void OnButtonStart()
    {
        if (isStart)
            return;

        isStart = true;

        SoundManager.Play(SoundManager.SEClip.ok);
        FadeManager.Instance.LoadScene("StartScene", 0.2f);
    }

	// ツイート表示
	public void OnTweet()
	{
		// System.TimeSpan timeScore = new System.TimeSpan (0, 0, 0, 0, Mathf.FloorToInt(PlayData.totalTime * 1000f));

		//本文＋ハッシュタグ＊２ツイート（画像なし）
		naichilab.UnityRoomTweet.Tweet (
			"temo_darksidemoon", 
			"【月の裏側】" + PlayData.rankingName.ToString() + "は、夜空を見上げた。" + 
            "月面で散っていった" + PlayData.gameOverCount.ToString() + "人のクローンの存在など知らずに。",
			"unityroom", "unity1week");	
	}


}
