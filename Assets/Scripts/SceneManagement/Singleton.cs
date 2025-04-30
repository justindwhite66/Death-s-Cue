using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        if (instance != null && this.gameObject != null)
        {
            Debug.Log($"Destroying duplicate {typeof(T).Name}");
            Destroy(this.gameObject);
        }
        else
        {
            instance = (T)this;
            Debug.Log($"Setting {typeof(T).Name} instance");
            
            if (!gameObject.transform.parent)
            {
                DontDestroyOnLoad(gameObject);
                Debug.Log($"Set {typeof(T).Name} to DontDestroyOnLoad");
            }
        }
    }
    
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            Debug.Log($"{typeof(T).Name} instance destroyed");
        }
    }
}
