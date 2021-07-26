using UnityEngine;

/// <summary>
/// Makes a single component easy to find. Being a Singleton doesn't guarantee that the component is unique!
/// </summary>
/// <typeparam name="T">Component</typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    /// <summary>
    /// Oldest instance of this component.
    /// </summary>
    static T _Instance;

    /// <summary>
    /// Find and return the oldest instance.
    /// </summary>
    public static T Instance {
        get {
            return _Instance ? _Instance : (_Instance = FindObjectOfType<T>());
        }
    }
}