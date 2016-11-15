using UnityEngine;
using BoxDash.Map;
using BoxDash.Tile;
using BoxDash.Utility;

namespace BoxDash.SceneCamera {
    [RequireComponent(typeof(Camera))]
    public class CameraController : Singleton<CameraController>
    {
        #region Events
        private void OnEnable()
        {
            EventCenter.PlayerMovedInWorldSpaceEvent += UpdateCameraDesireLocation;
        }

        private void OnDisable()
        {
            EventCenter.PlayerMovedInWorldSpaceEvent -= UpdateCameraDesireLocation;
        }
        #endregion

        #region Public varibales
        // ---------- Public varibales -----------
        public static bool StartFollowingPlayer = false;




        #endregion

        #region Private varibales
        // ------------- Private varibales -------------
        // The reference of the camera compoment on this game object.
        private Camera m_SceneCamera;
        [Range(1, 5)]
        [SerializeField]
        // The camera follow speed.
        private int m_FollowSpeed = 1;
        // The camera only needs to follow the player on Z axis.
        private float m_DesireCameraDestinationZ = 0;
        #endregion

        public void Init() {
            m_SceneCamera  = GetComponent<Camera>();
            ToMapCenter();
        }

        void UpdateCameraDesireLocation(Vector3 playerPosition)
        {
            // Update the camera's
            m_DesireCameraDestinationZ = Mathf.Floor(playerPosition.z);
        }

        /// <summary>
        /// Reset the camera's position so it can fouces on the center of the map.
        /// </summary>
        public void ToMapCenter() {
            m_SceneCamera.transform.position = new Vector3(
                // TileBase.TileOffset * Mathf.FloorToInt((MapManager.Instance.GetMaximunTilesOnColnum - 1) / 2),
                TileBase.TileOffset * ((MapManager.Instance.GetMaximunTilesOnColnum - 1) / 2) - TileBase.TileOffset / 2,
                m_SceneCamera.transform.position.y,
                m_SceneCamera.transform.position.z);
        }

        private void Update() {
            // If current camera position is far away form the player position on Z axis.
            if (!Mathf.Approximately(this.transform.position.z, m_DesireCameraDestinationZ)) {
               this.transform.localPosition = 
                    new Vector3(
                        this.transform.localPosition.x, 
                        this.transform.localPosition.y, 
                        // Lerp bwtween current camera position Z and player position Z.
                        Mathf.Lerp(this.transform.localPosition.z, 
                                    m_DesireCameraDestinationZ, 
                                    Time.deltaTime * m_FollowSpeed));
            }
        }
    }
}
