using UnityEngine;

namespace UnityChain
{
	public class ChainCollider : MonoBehaviour
	{
	    private void Start()
        {
            Owner = GetComponent<UnityChain.Particle>();
            m_transformCache = transform;
        }

        public Vector3 ComputePenetration(ChainCollider collider)
        {
            if (null == collider)
            {
                return Vector3.zero;
            }

            CubeVertexs colliderOBB = collider.ComputeOBB();
            Vector3 collider2BoundsPosition = collider.WorldCenter;
            Vector3 collider2BoundsExtents = ComputeExtents(ref collider2BoundsPosition, colliderOBB, GetWorldToLocalMatrix());

            Vector3 penetration = CubeColliderHelper.ComputePenetration(center, Extents, collider2BoundsPosition, collider2BoundsExtents);

            Matrix4x4 local2WorldMatrix = Matrix4x4.TRS(Vector3.zero, m_transformCache.rotation, m_transformCache.lossyScale);
            penetration = local2WorldMatrix.MultiplyPoint3x4(penetration);

            return penetration;
        }

        public CubeVertexs ComputeOBB()
        {
            Matrix4x4 matrix = GetLocalToWorldMatrix();
            Vector3 position = center;

            Vector3 extents = Extents;

            Vector3 p000 = matrix.MultiplyPoint(position + new Vector3(-extents.x, -extents.y, -extents.z));
            Vector3 p001 = matrix.MultiplyPoint(position + new Vector3(-extents.x, -extents.y, extents.z));
            Vector3 p010 = matrix.MultiplyPoint(position + new Vector3(-extents.x, extents.y, -extents.z));
            Vector3 p011 = matrix.MultiplyPoint(position + new Vector3(-extents.x, extents.y, extents.z));
            Vector3 p100 = matrix.MultiplyPoint(position + new Vector3(extents.x, -extents.y, -extents.z));
            Vector3 p101 = matrix.MultiplyPoint(position + new Vector3(extents.x, -extents.y, extents.z));
            Vector3 p110 = matrix.MultiplyPoint(position + new Vector3(extents.x, extents.y, -extents.z));
            Vector3 p111 = matrix.MultiplyPoint(position + new Vector3(extents.x, extents.y, extents.z));

            CubeVertexs vertexs = CubeVertexs.Buffer;
            vertexs[0] = p000;
            vertexs[1] = p001;
            vertexs[2] = p010;
            vertexs[3] = p011;
            vertexs[4] = p100;
            vertexs[5] = p101;
            vertexs[6] = p110;
            vertexs[7] = p111;

            return vertexs;
        }

        public CubeVertexs ComputeOBB2AABBWithTransformCoordinates(ChainCollider collider)
        {
            CubeVertexs vertexs = collider.ComputeOBB();

            Vector3 collider2BoundsPosition = collider.WorldCenter;
            Vector3 collider2BoundsExtents = ComputeExtents(ref collider2BoundsPosition, vertexs, m_transformCache.worldToLocalMatrix);

            float x = collider2BoundsExtents.x;
            float y = collider2BoundsExtents.y;
            float z = collider2BoundsExtents.z;

            Vector3 p000 = collider2BoundsPosition + new Vector3(-x, -y, -z);
            Vector3 p001 = collider2BoundsPosition + new Vector3(-x, -y, z);
            Vector3 p010 = collider2BoundsPosition + new Vector3(-x, y, -z);
            Vector3 p011 = collider2BoundsPosition + new Vector3(-x, y, z);
            Vector3 p100 = collider2BoundsPosition + new Vector3(x, -y, -z);
            Vector3 p101 = collider2BoundsPosition + new Vector3(x, -y, z);
            Vector3 p110 = collider2BoundsPosition + new Vector3(x, y, -z);
            Vector3 p111 = collider2BoundsPosition + new Vector3(x, y, z);

            vertexs[0] = p000;
            vertexs[1] = p001;
            vertexs[2] = p010;
            vertexs[3] = p011;
            vertexs[4] = p100;
            vertexs[5] = p101;
            vertexs[6] = p110;
            vertexs[7] = p111;

            int count = vertexs.Count;
            for (int i = 0; i < count; i++)
            {
                vertexs[i] = GetLocalToWorldMatrix().MultiplyPoint3x4(vertexs[i]);
            }

            return vertexs;
        }

        private Vector3 ComputeExtents(ref Vector3 center, CubeVertexs vertexs, Matrix4x4 matrix)
        {
            Vector3 extents = Vector3.zero;
            center = matrix.MultiplyPoint3x4(center);

            int count = vertexs.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 v = matrix.MultiplyPoint3x4(vertexs[i]);
                Vector3 offset = v - center;
                extents.x = Mathf.Max(Mathf.Abs(offset.x), extents.x);
                extents.y = Mathf.Max(Mathf.Abs(offset.y), extents.y);
                extents.z = Mathf.Max(Mathf.Abs(offset.z), extents.z);
            }

            return extents;
        }

        private Matrix4x4 GetLocalToWorldMatrix()
        {
            if (null == Owner)
            {
                return m_transformCache.localToWorldMatrix;
            }

            return Owner.LocalToWorldMatrix;
        }


        private Matrix4x4 GetWorldToLocalMatrix()
        {
            if (null == Owner)
            {
                return m_transformCache.worldToLocalMatrix;
            }

            return Owner.WorldToLocalMatrix;
        }

        public Vector3 Extents
        {
            get { return size / 2; }
        }

        public Vector3 WorldCenter
        {
            get {
                Vector3 position = GetLocalToWorldMatrix().MultiplyPoint3x4(center);
                return position;
            }
        }

        public UnityChain.Particle Owner { get; private set; }

        public Vector3 center;
        public Vector3 size;

        private Transform m_transformCache;
    }
}