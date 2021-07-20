using UnityEngine;

namespace UnityChain
{
	public class Particle : MonoBehaviour
	{
	    public void Prepare(int index)
        {
            Index = index;
            SetTransform();
            Reset();
            particleCollider = GetComponent<ChainCollider>();

            if (null != particleCollider)
            {
                particleCollider.Reset();
            }
        }

        public void Reset()
        {
            SetPrePosition(m_archivePosition);
            SetPosition(m_archivePosition, EParticlePositionChangedEvent.Reset);
            SetRotation(m_archiveRotation);
            SetForce(Vector3.zero);
        }

        public void UpdateRender()
        {
            if (null == m_transformCache)
            {
                return;
            }
            
            m_transformCache.position = Position;
            m_transformCache.rotation = Rotation;
        }

        public void SetPrePosition(Vector3 position)
        {
            PrePosition = position;
        }

        public void SetPosition(Vector3 position, EParticlePositionChangedEvent changedEvent)
        {
            Position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            Rotation = rotation;
        }

        public void SetLinkDistance(float distance)
        {
            LinkDistance = distance;
        }

        public void SetForce(Vector3 force)
        {
            Force = force;
        }

        public void SetIsCollide(bool isCollide)
        {
            IsCollide = isCollide;
        }

        private void SetTransform()
        {
            m_transformCache = transform;
            
            m_archivePosition = m_transformCache.position;
            m_archiveRotation = m_transformCache.rotation;
        }

        public Matrix4x4 LocalToWorldMatrix
        {
            get {
                Matrix4x4 move = Matrix4x4.identity;
                move.SetColumn(3, PositionOffset);
                move[3, 3] = 1;

                Matrix4x4 matrix = m_transformCache.localToWorldMatrix;
                matrix = move * matrix;
                return matrix;
            }
        }

        public Matrix4x4 WorldToLocalMatrix
        {
            get
            {
                Matrix4x4 move = Matrix4x4.identity;
                move.SetColumn(3, PositionOffset);
                move[3, 3] = 1;

                Matrix4x4 matrix = m_transformCache.worldToLocalMatrix;
                matrix = matrix * move;
                return matrix;
            }
        }

        public Vector3 PositionOffset
        {
            get
            {
                Vector3 offset = Position - m_transformCache.position;
                return offset;
            }
        }

        public float velocityDampen = 0.95f;
        public float stiffness = 1f;
        public Vector3 gravity = new Vector3(0, -2, 0);

        public int Index { get; private set; }
        public float LinkDistance { get; private set; }
        public ChainCollider particleCollider { get; private set; }
        public Vector3 PrePosition { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 Force { get; private set; }
        public bool IsCollide { get; private set; }

        private Vector3 m_archivePosition;
        private Quaternion m_archiveRotation;
        private Transform m_transformCache;
    }
}