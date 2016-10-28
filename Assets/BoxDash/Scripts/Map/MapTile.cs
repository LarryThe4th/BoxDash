using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace BoxDash.Map {
    [RequireComponent(typeof(Rigidbody))]
    public class MapTile : MonoBehaviour
    {
        public Renderer UpperMesh;
        public Color32 OriginalUpperMeshColor;
        public Vector3 OriginalLocalPosition;

        private Rigidbody m_RigidBody;

        private Renderer[] renders;

        public bool IsCollapsed {
            get; private set;
        }

        private void Start() {
            IsCollapsed = false;

            m_RigidBody = GetComponent<Rigidbody>();
            m_RigidBody.useGravity = false;

            renders = GetComponentsInChildren<Renderer>();
        }

        private void ResetRigidBody()
        {
            m_RigidBody.useGravity = false;
            m_RigidBody.velocity = Vector3.zero;
            m_RigidBody.angularVelocity = Vector3.zero;
        }

        public void ResetTile() {
            // StopCoroutine(HideTile());
            IsCollapsed = false;
            // Reset the rigid body so it wont fall down anymore.
            ResetRigidBody();
            // Reset its position.
            this.transform.localPosition = OriginalLocalPosition;
            this.transform.rotation = Quaternion.Euler(GameManager.TileRotation);
            // Reset its color.
            UpperMesh.material.color = OriginalUpperMeshColor;
            // Show the tile again.
            DisplayTile(true);
        }

        private void DisplayTile(bool display) {
            foreach (var item in renders)
            {
                item.enabled = display;
            }
        }

        public void StartCollapse() {
            IsCollapsed = true;
            m_RigidBody.useGravity = true;
            m_RigidBody.angularVelocity = new Vector3(
                Random.Range(0.0f, 1.0f), 
                Random.Range(0.0f, 1.0f), 
                Random.Range(0.0f, 1.0f)) * (Random.Range(1, 10));

            // StartCoroutine(HideTile());
        }

        /// <summary>
        /// After collapsed, the tile will hide it self.
        /// </summary>
        private IEnumerator HideTile() {
            yield return new WaitForSeconds(1f);
            // Disable the renderer conpomenet.
            foreach (var item in renders)
            {
                DisplayTile(false);
            }
            // Reset the rigid body so it wont fall down anymore.
            ResetRigidBody();
        }
    }
}
