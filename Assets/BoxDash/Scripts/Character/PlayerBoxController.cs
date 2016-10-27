using UnityEngine;
using BoxDash.Utility;

namespace BoxDash.Player {
    /// <summary>
    /// This class controls the player box object.
    /// </summary>
    public class PlayerBoxController : MonoBehaviour
    {
        #region Private variables
        [SerializeField]
        private Transform m_PlayerBoxViualRoot;
        #endregion

        public void Init() {
#if UNITY_EDITOR
            if (!m_PlayerBoxViualRoot) {
                Debug.LogError("m_PlayerBoxViualRoot is NULL.");
                return;
            }
#endif
            // Lift it up to the ground.
            this.transform.position += new Vector3(0, GameManager.TileSideLength / 2, 0);
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.M))
        //    {

        //    }
        //}
    }
}
