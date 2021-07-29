namespace UnityChain
{
    public static class UnityChainRuntimeDefine
    {
        static UnityChainRuntimeDefine()
        {
            EnablChainGizmos = true;
            EnablFrameDebuggerCollectData = false;
            EnablFrameDebuggerReplay = false;
            ChainGizmosColliderFlag = 15;
        }

        public static bool EnablChainGizmos { get; set; }
        public static bool EnablFrameDebuggerCollectData { get; set; }
        public static bool EnablFrameDebuggerReplay { get; set; }
        public static int ChainGizmosColliderFlag { get; set; }
    }
}