using UnityEngine;
using System.Collections;

namespace BoxDash.UI {
    [RequireComponent(typeof(Animator), typeof(CanvasGroup))]
    public abstract class SceneUIBase : MonoBehaviour
    {
        #region Event
        private void OnEnable()
        {
            EventCenter.UpdateUIEvent += UpdateUI;
        }

        private void OnDisable()
        {
            EventCenter.UpdateUIEvent -= UpdateUI;
        }
        #endregion

        protected Animator m_Animator;
        protected CanvasGroup m_CanvasGroup;
        protected bool m_IsDisplaying = false;

        public abstract UIManager.SceneUIs GetUIType();

        public virtual void Init() {
            m_Animator = GetComponent<Animator>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_IsDisplaying = false;
        }

        public virtual void EnableUI(bool enable) {
            this.gameObject.SetActive(enable);
        }

        public virtual void UpdateUI() { }

        public virtual void HideUI() {
            m_IsDisplaying = false;
        }

        public virtual void ShowUI() {
            m_IsDisplaying = true;
        }
    }
}
