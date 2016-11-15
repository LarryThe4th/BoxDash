using UnityEngine;
using System.Collections;

namespace BoxDash.Utility {
    public abstract class Singleton<T> : MonoBehaviour
        where T : Component
    {
        private static T m_Instance;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    var objs = FindObjectsOfType(typeof(T)) as T[];
                    if (objs.Length > 0)
                        m_Instance = objs[0];
                    if (objs.Length > 1)
                    {
#if UNITY_EDITOR
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
#endif
                    }
                    if (m_Instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        m_Instance = obj.AddComponent<T>();
                    }
                }
                return m_Instance;
            }
        }
    }
}
