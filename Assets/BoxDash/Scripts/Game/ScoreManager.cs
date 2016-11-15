using System;
using System.Collections.Generic;
using BoxDash.SaveAndLoad;
using BoxDash.Utility;


namespace BoxDash.Score
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        public enum ScoreTypes {
            MaxDistance = 0,   // Keep track how far did this player goes.
            Credit,             // These credit points can unlock new box apperence.
        }

        #region Private variable
        private string m_LastLoginPlayerName;
        private Dictionary<string, int> m_ScoreRecorder = new Dictionary<string, int>();
        #endregion

        public void Init(string playerName) {
            UserLoaclDataManager.DeleteAllKey();
            m_LastLoginPlayerName = playerName;
            m_ScoreRecorder = new Dictionary<string, int>();
            InitScoreRecorder();
        }

        private void InitScoreRecorder() {
            foreach (int iter in Enum.GetValues(typeof(ScoreTypes)))
            {
                string key = Enum.GetName(typeof(ScoreTypes), iter);
                m_ScoreRecorder.Add(
                    Enum.GetName(typeof(ScoreTypes), iter) + m_LastLoginPlayerName,
                    UserLoaclDataManager.LoadData(key, 0));
            }
        }

        public void SetNewScore(ScoreTypes type, int addScore) {
            m_ScoreRecorder[type.ToString() + m_LastLoginPlayerName] += addScore;
        }

        public int GetData(ScoreTypes type) {
            return m_ScoreRecorder[type.ToString() + m_LastLoginPlayerName];
        }

        public void SaveScore() {
            if (m_ScoreRecorder.Count == 0) return;
            foreach (var key in m_ScoreRecorder.Keys)
            {
                UserLoaclDataManager.SetIntToPlayerPerf(key, m_ScoreRecorder[key]);
            }
        }
    }
}
