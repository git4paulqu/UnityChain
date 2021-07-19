using UnityEngine;

namespace UnityChain
{
     public static class CubeColliderHelper
    {
        public static Vector3 ComputePenetration(Vector3 boundsCenter, Vector3 boundsExtents, Vector3 cubeCenter, Vector3 cubeExtents)
        {
            Vector3 penetration = Vector3.zero;
            penetration.x = ComputeAxisX(boundsCenter, boundsExtents, cubeCenter, cubeExtents);
            penetration.y = ComputeAxisY(boundsCenter, boundsExtents, cubeCenter, cubeExtents);
            penetration.z = ComputeAxisZ(boundsCenter, boundsExtents, cubeCenter, cubeExtents);
            return penetration;
        }

        private static float ComputeAxisX(Vector3 boundsCenter, Vector3 boundsExtents, Vector3 cubeCenter, Vector3 cubeExtents)
        {
            float x = 0;

            float minBounds = boundsCenter.x - boundsExtents.x;
            float minCube = cubeCenter.x - cubeExtents.y;
            float maxBounds = boundsCenter.x + boundsExtents.x;
            float maxCube = cubeCenter.x + cubeExtents.x;

            if (minCube < minBounds)
            {
                x = minBounds - minCube;
            }
            else if (maxCube > maxBounds)
            {
                x = maxBounds - maxCube;
            }
            return x;
        }

        private static float ComputeAxisY(Vector3 boundsCenter, Vector3 boundsExtents, Vector3 cubeCenter, Vector3 cubeExtents)
        {
            float y = 0;

            float minBounds = boundsCenter.y - boundsExtents.y;
            float minCube = cubeCenter.y - cubeExtents.y;
            float maxBounds = boundsCenter.y + boundsExtents.y;
            float maxCube = cubeCenter.y + cubeExtents.y;

            if (minCube < minBounds)
            {
                y = minBounds - minCube;
            }
            else if (maxCube > maxBounds)
            {
                y = maxBounds - maxCube;
            }
            return y;
        }

        private static float ComputeAxisZ(Vector3 boundsCenter, Vector3 boundsExtents, Vector3 cubeCenter, Vector3 cubeExtents)
        {
            float z = 0;

            float minBounds = boundsCenter.z - boundsExtents.z;
            float minCube = cubeCenter.z - cubeExtents.z;
            float maxBounds = boundsCenter.z + boundsExtents.z;
            float maxCube = cubeCenter.z + cubeExtents.z;

            if (minCube < minBounds)
            {
                z = minBounds - minCube;
            }
            else if (maxCube > maxBounds)
            {
                z = maxBounds - maxCube;
            }
            return z;
        }
    }
}