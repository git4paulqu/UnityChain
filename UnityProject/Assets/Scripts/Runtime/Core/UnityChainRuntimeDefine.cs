namespace UnityChain
{
    public static class UnityChainRuntimeDefine
    {
        static UnityChainRuntimeDefine()
        {
            EnablChainGizmos = true;
            ChainGizmosColliderFlag = 15;
        }

        public static bool EnablChainGizmos { get; set; }
        public static int ChainGizmosColliderFlag { get; set; }
    }
}