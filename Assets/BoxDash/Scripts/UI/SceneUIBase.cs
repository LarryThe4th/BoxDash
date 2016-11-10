using UnityEngine;
using System.Collections;

namespace BoxDash.UI {
    public abstract class SceneUIBase : MonoBehaviour
    {

        //// Use this for initialization
        //void Start()
        //{

        //}

        public virtual void EnableUI(bool enable) {
            this.gameObject.SetActive(enable);
        }
    }
}
