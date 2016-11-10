using UnityEngine;
using Random = UnityEngine.Random;
using BoxDash.Utility;
using System.Collections;
using System.Collections.Generic;

namespace BoxDash.Tile {
    public enum TileTypes {
        Floor = 0,
        Wall,
        Hole,
        FloorSpikes,
        SkySpikes,
    }

    public class Location2D {
        private int m_LocationX = 0;
        private int m_LocationY = 0;

        public int X {
            get { return m_LocationX; }
            set { m_LocationX = value; }
        }

        public int Y {
            get { return m_LocationY; }
            set { m_LocationY = value; }
        }

        public void SetLocation(int x, int y)
        {
            m_LocationX = x; m_LocationY = y;
        }

        public Location2D() {
            m_LocationX = 0; m_LocationY = 0;
        }
    }

    [RequireComponent(typeof(Rigidbody))]
    public abstract class TileBase : PoolObject
    {
        #region Global static variables
        public static readonly float TileSideLength = 0.254f;
        public static readonly float TileOffset = Mathf.Sqrt(2) * TileSideLength;
        public static readonly Vector3 TileFixedRotation = new Vector3(-90, 45, 0);
        public static readonly Quaternion TileFixedQuaternion = Quaternion.Euler(TileFixedRotation);
        #endregion

        #region Public varibales
        // public Vector3 OriginLocation = Vector3.zero;
        public bool IsCollapsed
        {
            get; private set;
        }

        // Keep tracking the position where this tile repersened to.
        public Location2D CurrentLocation = new Location2D();

        public bool CanPass {
            get { return m_CanPass; }
        }
        #endregion

        #region Private variables
        protected Rigidbody m_RigidBody;
        protected List<Renderer> m_RendererComponents = new List<Renderer>();
        protected Animator m_Animator;
        protected bool m_CanPass = true;
        #endregion

        public virtual void Init(int rowIndex, int columnIndex, Color32 tileColor) {
            CurrentLocation.SetLocation(columnIndex, rowIndex);
            StopFalling();
        }

        public virtual void SetTileColor(Color32 tileColor) {
            foreach (var render in m_RendererComponents)
            {
                render.material.color = tileColor;
            }
        }

        public override void InitPoolObject() {
            IsCollapsed = false;
            m_RigidBody = GetComponent<Rigidbody>();
            m_RigidBody.useGravity = false;

            foreach (var render in GetComponentsInChildren<Renderer>())
            {
                m_RendererComponents.Add(render);
            }

            m_Animator = GetComponent<Animator>();
        }

        public override void EnableObject(bool enable)
        {
            base.EnableObject(enable);
            // When enabling the tiles it will stay at where it should be,
            // and when disabling the tiles it should not keep falling down, so
            // whatever the condition is, it should not be affect by physics
            // unleast it is call by collapse().
            StopFalling();
        }

        public virtual void UseTile(params object[] options)
        {
            // Empty
        }

        public virtual void Collapse() {
            IsCollapsed = true;
            m_RigidBody.useGravity = true;
            m_RigidBody.angularVelocity = new Vector3(
                Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f)) * (Random.Range(1, 10));
            StartCoroutine(StopFallingAfterCollapsed(1f));
        }

        private IEnumerator StopFallingAfterCollapsed(float wait) {
            yield return new WaitForSeconds(wait);
            // Stop falling down.
            StopFalling();
        }

        protected virtual void StopFalling() {
            IsCollapsed = false;
            m_RigidBody.useGravity = false;
            m_RigidBody.velocity = Vector3.zero;
            m_RigidBody.angularVelocity = Vector3.zero;
        }

        public abstract TileTypes GetTileType();

        public virtual void ClearToPass() {
            m_CanPass = true;
        }

        public virtual void UnclearToPass()
        {
            m_CanPass = false;
        }

        //public TileTypes GetTileType {
        //    get { return m_TileType; }
        //}
        //private TileTypes m_TileType = TileTypes.Floor;

        //private Rigidbody m_RigidBody;
        //private Renderer[] renders;

        //public bool IsCollapsed {
        //    get; private set;
        //}

        //public void Init(TileTypes tileType) {
        //    IsCollapsed = false;
        //    m_RigidBody = GetComponent<Rigidbody>();
        //    m_RigidBody.useGravity = false;
        //    renders = GetComponentsInChildren<Renderer>();
        //    m_TileType = tileType;
        //}

        //private void SetType(TileTypes tileType) {
        //    // Only the wall will not change.
        //    if (m_TileType == TileTypes.Wall) return;
        //    m_TileType = tileType;
        //    switch (tileType) {
        //        case TileTypes.Floor:
        //            DisplayTile(true);
        //            break;
        //        case TileTypes.Hole:
        //            DisplayTile(false);
        //            break;
        //    }
        //}

        //private void ResetRigidBody()
        //{
        //    m_RigidBody.useGravity = false;
        //    m_RigidBody.velocity = Vector3.zero;
        //    m_RigidBody.angularVelocity = Vector3.zero;
        //}

        //public void ResetTile(TileTypes newTileType) {
        //    // StopCoroutine(HideTile());
        //    IsCollapsed = false;
        //    // Reset the rigid body so it wont fall down anymore.
        //    ResetRigidBody();
        //    // Reset its position.
        //    this.transform.localPosition = OriginalLocalPosition;
        //    this.transform.rotation = Quaternion.Euler(GameManager.TileRotation);
        //    // Reset its color.
        //    UpperMesh.material.color = OriginalUpperMeshColor;
        //    // Show the tile again.
        //    SetType(newTileType);
        //}

        //public void DisplayTile(bool display) {
        //    if (m_TileType == TileTypes.Wall) return;
        //    foreach (var item in renders)
        //    {
        //        item.enabled = display;
        //    }
        //}

        //public void StartCollapse() {
        //    IsCollapsed = true;
        //    m_RigidBody.useGravity = true;
        //    m_RigidBody.angularVelocity = new Vector3(
        //        Random.Range(0.0f, 1.0f), 
        //        Random.Range(0.0f, 1.0f), 
        //        Random.Range(0.0f, 1.0f)) * (Random.Range(1, 10));

        //    // StartCoroutine(ResetTilePosition());
        //}

        //private IEnumerator ResetTilePosition() {
        //    yield return new WaitForSeconds(1f);
        //    // Reset the rigid body so it wont fall down anymore.
        //    ResetRigidBody();
        //    // Reset its position.
        //    this.transform.localPosition = OriginalLocalPosition;
        //    this.transform.rotation = Quaternion.Euler(GameManager.TileRotation);
        //    // Reset its color.
        //    UpperMesh.material.color = OriginalUpperMeshColor;
        //    DisplayTile(false);
        //}
    }
}
