using UnityEngine;

#if UNITY_EDITOR

namespace UnityChain
{
    public partial class Chain
    {
        [ContextMenu("Debug Propertry")]
        private void ResetPropertry()
        {
            ResetVelocityDampen();
            ResetStiffness();
            ResetGravity();
        }

        private void ResetVelocityDampen()
        {
            foreach (var p in particles)
            {
                if (null == p)
                {
                    continue;
                }

                p.velocityDampen = particleVelocityDampen;
            }
        }

        private void ResetStiffness()
        {
            foreach (var p in particles)
            {
                if (null == p)
                {
                    continue;
                }

                p.stiffness = particleStiffness;
            }
        }
        
        private void ResetGravity()
        {
            foreach (var p in particles)
            {
                if (null == p)
                {
                    continue;
                }

                p.gravity = particleGravity;
            }
        }

        [Header("[DEBUG]")]
        public float particleVelocityDampen = 0.95f;
        [Header("[DEBUG]")]
        public float particleStiffness = 1f;
        [Header("[DEBUG]")]
        public Vector3 particleGravity = new Vector3(0, -0.1f, 0);
    }
}

#endif //UNITY_EDITOR