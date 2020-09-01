# DarkSideMoon_Script
Unity1週間ゲームジャム　お題「ふえる」に投稿した「月の裏側」のスクリプトです。 
プレイはこちら↓  
[UnityRoomの作品ページ 月の裏側](https://unityroom.com/games/temo_darksidemoon)

## 利用したアセットやライブラリ（スクリプトに関係するもの）

### ・PUN 2 Free
Photonオンライン機能の実装に利用  
[アセットストアページ](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922)

### ・DOTween (HOTween v2)
簡単な動きと、遅延実行に利用
[アセットストアページ](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)

### ・NCMB
ランキングと、死体データの格納・復帰に利用
[アセットストアページ](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)

### ・unityroom-tweet
ツイート機能に利用
[アセットストアページ](https://github.com/naichilab/unityroom-tweet)

### ・unity-simple-ranking
ランキング機能に利用
[アセットストアページ](https://blog.naichilab.com/entry/webgl-simple-ranking)

### ・naichilab/Unity-FadeManager
暗転するシーンの切り替えに利用
[アセットストアページ](https://github.com/naichilab/Unity-FadeManager)

### ・naichilab/Unity-BgmManager
BGM再生に利用
[アセットストアページ](https://github.com/naichilab/Unity-BgmManager)

### ・am1tanaka/OpenSimpleFramework201801
この中のSoundControllerをSoundManagerとして、変名・改造し利用
[アセットストアページ](https://github.com/am1tanaka/OpenSimpleFramework201801)

### ・SimpleAnimation
Animatorとか使わないで、アニメーションする
[アセットストアページ](https://github.com/Unity-Technologies/SimpleAnimation)

## ソースの簡単な解説
* ●●Controller : 動くモノ 
* ●●Spawner : 敵などを生み出すモノ
* GameManager : ゲーム全体を司る神っぽいクラス
* NetworkManager : ゲーム全体に関するPhoton周りの神っぽいクラス（接続やルーム入室など）
* CorpseManager : 死体データのNCMBとの読み取り、書き出しを司るクラス
* PUNとかRPCとかついているのはPhoton関連です。
* ソース内のMyUtil.●●とあるのは自作のユーティリティメソッドを呼び出しています。
* データを格納するEntityとして以下
    * PlayData : ゲーム全体を通して保持するデータを格納（スコアとか）
    * CorpseData : 死体データ1つ分を格納し、この単位でNCMBとやり取り

## Utilフォルダ下のソース
* MyUtil●● : よく使うけど、よく書き方を忘れるメソッドや処理を記載。
* Projectile : Inspectorで指定した方向に飛んでいく
* ColliderController2D : 衝突時に行う処理を呼び出す
* BoolNoticer : boolのフラグが切り替わった際に、なにか処理をする
* MyMonoBehaviour : 以下の機能を呼び出せる基底クラス
    * 条件に合致したら、１回のみ、なにか処理をする
    * 指定した秒数ごとに、なにか処理をする
    * 指定した秒数ごとに、条件に合致するかチェックし、なにか処理をする