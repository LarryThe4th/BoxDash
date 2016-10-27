using UnityEngine;
using System.Collections;

namespace BoxDash.SaveAndLoad {
    public static class UserLoaclDataManager
    {
        /// <summary>
        /// Check if there is a key that is aleady in the player prefabs.
        /// </summary>
        /// <param name="key">The key value</param>
        /// <returns>Reutns TRUE if the key did exist.</returns>
        public static bool PlayerPerfabHasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.GetInt() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static int GetIntFormPlayerPerf(string key)
        {
            return PlayerPrefs.GetInt(key);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.GetFloat() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static float GetFloatFormPlayerPerf(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.GetInt() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static string GetstringFormPlayerPerf(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        /// <summary>
        /// A wapper methd of the PlayerPrefs.SetFloat() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static void SetIntToPlayerPerf(string key, int valueInt)
        {
            PlayerPrefs.SetInt(key, valueInt);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.SetFloat() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static void SetFloatToPlayerPerf(string key, float valueFloat)
        {
            PlayerPrefs.SetFloat(key, valueFloat);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.SetString() method.
        /// </summary>
        /// <param name="key">The key you are looking for.</param>
        /// <returns>If key exsit, return ture.</returns>
        public static void SetStringToPayerPerf(string key, string valueString)
        {
            PlayerPrefs.SetString(key, valueString);
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.DeleteAll() method.
        /// </summary>
        public static void DeleteAllKey()
        {
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// A wapper method of the PlayerPrefs.DeleteKey() method.
        /// </summary>
        /// <param name="key">The target value's key.</param>
        public static void DeleteTheKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        /// <summary>
        /// The staic class constructor of the UserLoaclDataManager,
        /// Clear all data when first time called.
        /// </summary>
        static UserLoaclDataManager() {
            DeleteAllKey();
        }
    }
}