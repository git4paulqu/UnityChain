namespace UnityChain
{
    public enum ChainColliderGizmosFlag
    {
        Default                     = 0,
        Bounds                      = 1 << 0,
        ColliderOBB                 = 1 << 1,
        ColliderAABBFromBounds      = 1 << 2,
        ColliderFixedOBB            = 1 << 3,
    }
}