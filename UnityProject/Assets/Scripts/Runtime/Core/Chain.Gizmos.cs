using UnityEngine;

namespace UnityChain
{
	public partial class Chain
	{
        private void OnDrawGizmos()
        {
            bool enableGizmos = UnityChainRuntimeDefine.EnablChainGizmos;
            if (!enableGizmos)
            {
                return;
            }

            DrawLine();

            DrawColliders();
        }

        private void DrawLine()
		{
			int count = ParticleCount;
			if (count < 1)
			{
				return;
			}

			for (int i = 0; i < count; i++)
			{
				Particle p = particles[i];

				Gizmos.DrawSphere(p.Position, 0.01f);

				int next = i + 1;
				if (next >= count)
				{
					continue;
				}

				Particle p2 = particles[next];
				Gizmos.DrawLine(p2.Position, p.Position);
			}
		}

		private void DrawColliders()
		{
            DrawBounds();

            DrawColliderOBB();

            DrawColliderAABBFromBounds();

            DrawColliderFixedOBB();
		}
		
		 private void DrawBounds()
        {
            if (!EqualFlag(ChainColliderGizmosFlag.Bounds))
            {
                return;
            }

            if (null == bounds)
            {
                return;
            }

            CubeVertexs vertexs = bounds.ComputeOBB();
            DrawVertexs(vertexs, Color.red);
        }

        private void DrawColliderOBB()
        {
            if (!EqualFlag(ChainColliderGizmosFlag.ColliderOBB))
            {
                return;
            }

            if (null == particleColliders)
            {
                return;
            }

            foreach (var item in particleColliders)
            {
                if (null == item)
                {
                    continue;
                }

                CubeVertexs vertexs = item.ComputeOBB();
                DrawVertexs(vertexs, Color.green);
            }
        }

        private void DrawColliderAABBFromBounds()
        {
            if (!EqualFlag(ChainColliderGizmosFlag.ColliderAABBFromBounds))
            {
                return;
            }

            if (null == particleColliders || null == bounds)
            {
                return;
            }

            foreach (var item in particleColliders)
            {
                if (null == item)
                {
                    continue;
                }

                CubeVertexs vertexs = bounds.ComputeOBB2AABBWithTransformCoordinates(item);
                DrawVertexs(vertexs, Color.blue);
            }
        }

        private void DrawColliderFixedOBB()
        {
            if (!EqualFlag(ChainColliderGizmosFlag.ColliderFixedOBB))
            {
                return;
            }

            if (null == particleColliders || null == bounds)
            {
                return;
            }

            foreach (var item in particleColliders)
            {
                if (null == item)
                {
                    continue;
                }

                Vector3 penetration = bounds.ComputePenetration(item);
                CubeVertexs vertexs = item.ComputeOBB();

                for (int i = 0; i < vertexs.Count; i++)
                {
                    vertexs[i] += penetration;
                }

                DrawVertexs(vertexs, Color.yellow);
            }
        }

        private void DrawVertexs(CubeVertexs vertexs, Color color)
        {
            Color backupColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawLine(vertexs[0], vertexs[1]);
            Gizmos.DrawLine(vertexs[2], vertexs[3]);
            Gizmos.DrawLine(vertexs[0], vertexs[2]);
            Gizmos.DrawLine(vertexs[1], vertexs[3]);

            Gizmos.DrawLine(vertexs[4], vertexs[5]);
            Gizmos.DrawLine(vertexs[6], vertexs[7]);
            Gizmos.DrawLine(vertexs[4], vertexs[6]);
            Gizmos.DrawLine(vertexs[5], vertexs[7]);

            Gizmos.DrawLine(vertexs[0], vertexs[4]);
            Gizmos.DrawLine(vertexs[1], vertexs[5]);
            Gizmos.DrawLine(vertexs[2], vertexs[6]);
            Gizmos.DrawLine(vertexs[3], vertexs[7]);

            Gizmos.color = backupColor;
        }

        private bool EqualFlag(ChainColliderGizmosFlag gizmosFlag)
        {
            int flag = UnityChainRuntimeDefine.ChainGizmosColliderFlag;
            return (flag & (int)gizmosFlag << 0) > 0;
        }
	}
}