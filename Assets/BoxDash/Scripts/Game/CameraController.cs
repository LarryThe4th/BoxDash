using UnityEngine;
using BoxDash.Map;
using BoxDash.Utility;

namespace BoxDash.SceneCamera {
    [RequireComponent(typeof(Camera))]
    public class CameraController : Singleton<CameraController>
    {
        #region Private varibales
        // ------------- Private varibales -------------
        private Camera m_SceneCamera;
        #endregion

        public void Init() {
            m_SceneCamera  = GetComponent<Camera>();

            ToMapCenter();
        }

        void UpdateCameraPosition()
        {

        }

        public void ToMapCenter() {
            m_SceneCamera.transform.position = new Vector3(
                GameManager.TileOffset * Mathf.FloorToInt((MapManager.MaxNumberOfTilesOnRow - 1) / 2),
                m_SceneCamera.transform.position.y,
                m_SceneCamera.transform.position.z);
        }
    }
}
