using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get => instance; }

    protected abstract T GetThis();

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = GetThis();
        }
        else
        {
            if (!instance.Equals(GetThis()))
            {
                Destroy(GetThis().gameObject);
            }
        }
        // DontDestroyOnLoad(instance.gameObject);
    }
}
