using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingController : MonoBehaviour
{
	// トータルスコアのランキング表示
	public void OnButtonRanking()
	{
		naichilab.RankingLoader.Instance.SendScoreAndShowRanking(PlayData.gameOverCount);
	}
}
