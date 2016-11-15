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
        /// Try load data from loacl storage based on the given key.
        /// </summary>
        /// <param name="key">The key for the target save data.</param>
        /// <param name="data">The local variable use for receive the loaded data.</param>
        /// <param name="defaultValue">The default value for the local variable in case can't find the match key in the database. </param>
        /// <returns>If key exist in the database, return TRUE.</returns>
        public static bool LoadData(string key, ref int data, int defaultValue = 0) {
            if (PlayerPerfabHasKey(key)) {
                data = GetIntFormPlayerPerf(key); return true;
            }
            SetIntToPlayerPerf(key, defaultValue);
            data = defaultValue;
            return false;
        }

        /// <summary>
        /// Try load data from loacl storage based on the given key.
        /// </summary>
        /// <param name="key">The key for the target save data.</param>
        /// <param name="data">The local variable use for receive the loaded data.</param>
        /// <param name="defaultValue">The default value for the local variable in case can't find the match key in the database. </param>
        /// <returns>If key exist in the database, return TRUE.</returns>
        public static bool LoadData(string key, ref string data, string defaultValue = "")
        {
            if (PlayerPerfabHasKey(key))
            {
                data = GetstringFormPlayerPerf(key); return true;
            }
            SetStringToPayerPerf(key, defaultValue);
            data = defaultValue;
            return false;
        }

        /// <summary>
        /// Try load data from loacl storage based on the given key.
        /// </summary>
        /// <param name="key">The key for the target save data.</param>
        /// <param name="data">The local variable use for receive the loaded data.</param>
        /// <param name="defaultValue">The default value for the local variable in case can't find the match key in the database. </param>
        /// <returns>If key exist in the database, return TRUE.</returns>
        public static bool LoadData(string key, ref float data, float defaultValue = 0.0f)
        {
            if (PlayerPerfabHasKey(key))
            {
                data = GetIntFormPlayerPerf(key); return true;
            }
            SetFloatToPlayerPerf(key, defaultValue);
            data = defaultValue;
            return false;
        }

        /// <summary>
        /// Try load data from loacl storage based on the given key.
        /// </summary>
        /// <param name="key">The key for the target save data.</param>
        /// <param name="defaultValue">The default value for the local variable in case can't find the match key in the database. </param>
        /// <returns></returns>
        public static int LoadData(string key, int defaultValue = 0) {
            if (PlayerPerfabHasKey(key))
            {
                return GetIntFormPlayerPerf(key);
            }
            SetIntToPlayerPerf(key, defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Try load data from loacl storage based on the given key.
        /// </summary>
        /// <param name="key">The key for the target save data.</param>
        /// <param name="defaultValue">The default value for the local variable in case can't find the match key in the database. </param>
        /// <returns></returns>
        public static string LoadData(string key, string defaultValue = "")
        {
            if (PlayerPerfabHasKey(key))
            {
                return GetstringFormPlayerPerf(key);
            }
            SetStringToPayerPerf(key, defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// Try load data from loacl storage based on the given key.
        /// </summary>
        /// <param name="key">The key for the target save data.</param>
        /// <param name="defaultValue">The default value for the local variable in case can't find the match key in the database. </param>
        /// <returns></returns>
        public static float LoadData(string key, float defaultValue = 0.0f)
        {
            if (PlayerPerfabHasKey(key))
            {
                return GetFloatFormPlayerPerf(key);
            }
            SetFloatToPlayerPerf(key, defaultValue);
            return defaultValue;
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
        /// The staic class constructor of the UserLoaclDataManager.
        /// </summary>
        static UserLoaclDataManager() {
            // DeleteAllKey();
        }
    }
}