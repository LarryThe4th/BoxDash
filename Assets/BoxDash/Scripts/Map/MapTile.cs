using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace BoxDash.Map {
    public enum TileTypes {
        Floor = 0,
        Wall = 1,
        Hole,
        Spik,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class MapTile : MonoBehaviour
    {
        public Renderer UpperMesh;
        public Color32 OriginalUpperMeshColor;
        public Vector3 OriginalLocalPosition;

        public TileTypes GetTileType {
            get { return m_TileType; }
        }
        private TileTypes m_TileType = TileTypes.Floor;

        private Rigidbody m_RigidBody;
        private Renderer[] renders;

        public bool IsCollapsed {
            get; private set;
        }

        public void Init(TileTypes tileType) {
            IsCollapsed = false;
            m_RigidBody = GetComponent<Rigidbody>();
            m_RigidBody.useGravity = false;
            renders = GetComponentsInChildren<Renderer>();
            m_TileType = tileType;
        }

        private void SetType(TileTypes tileType) {
            // Only the wall will not change.
            if (m_TileType == TileTypes.Wall) return;
            m_TileType = tileType;
            switch (tileType) {
                case TileTypes.Floor:
                    DisplayTile(true);
                    break;
                case TileTypes.Hole:
                    DisplayTile(false);
                    break;
            }
        }

        private void ResetRigidBody()
        {
            m_RigidBody.useGravity = false;
            m_RigidBody.velocity = Vector3.zero;
            m_RigidBody.angularVelocity = Vector3.zero;
        }

        public void ResetTile(TileTypes newTileType) {
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
            SetType(newTileType);
        }

        public void DisplayTile(bool display) {
            if (m_TileType == TileTypes.Wall) return;
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

            // StartCoroutine(ResetTilePosition());
        }

        private IEnumerator ResetTilePosition() {
            yield return new WaitForSeconds(1f);
            // Reset the rigid body so it wont fall down anymore.
            ResetRigidBody();
            // Reset its position.
            this.transform.localPosition = OriginalLocalPosition;
            this.transform.rotation = Quaternion.Euler(GameManager.TileRotation);
            // Reset its color.
            UpperMesh.material.color = OriginalUpperMeshColor;
            DisplayTile(false);
        }

    }
}
