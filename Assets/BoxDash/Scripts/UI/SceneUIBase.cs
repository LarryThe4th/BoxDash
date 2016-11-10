using UnityEngine;
using System.Collections;

namespace BoxDash.UI {
    [RequireComponent(typeof(Animator), typeof(CanvasGroup))]
    public abstract class SceneUIBase : MonoBehaviour
    {
        protected Animator m_Animator;
        protected CanvasGroup m_CanvasGroup;

        public abstract UIManager.UIs GetUIType();

        public virtual void Init() {
            m_Animator = GetComponent<Animator>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void EnableUI(bool enable) {
            this.gameObject.SetActive(enable);
        }

        public abstract void HideUI();

        public abstract void ShowUI();
    }
}
