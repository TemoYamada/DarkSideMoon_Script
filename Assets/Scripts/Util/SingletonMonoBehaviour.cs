using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
            }
            return instance;
        }
    }

    public static bool IsInstantiated       /// インスタンス化済みの是非
    {
        get { return instance != null; }
    }


    // ---------------------------------------------------------
    ///
    /// @brief 初期化処理を行う関数
    ///
    // ---------------------------------------------------------
    protected void Awake()
    {
        //Debug.Log( "SingletonMonoBehaviour.Awake() from " + GetType().Name );
        CheckInstance();
    }

    // ---------------------------------------------------------
    /// @brief 唯一のインスタンスか確認
    /// @details そうでない場合 Destroy します
    ///
    /// @return     呼び出し元のインスタンスが、唯一かどうか
    // ---------------------------------------------------------
    protected bool CheckInstance()
    {
        if (this == Instance)
        {
            //Debug.Log( typeof( T ) + " : The only m_Instance" );
            DontDestroyOnLoad(this);
            return true;
        }
        
        //Debug.Log( typeof( T ) + " : Destroy this" );
        Destroy(this.gameObject);
        return false;
    }
}