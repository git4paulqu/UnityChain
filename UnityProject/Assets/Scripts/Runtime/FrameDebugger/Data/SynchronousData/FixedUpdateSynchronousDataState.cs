namespace UnityChain.FrameDebugger
{
    public static class FixedUpdateSynchronousDataState
    {
        public static void PreSynchronous(FixedUpdateSynchronousData data)
        {
            if (null == data)
            {
                return;
            }
            
            if (data.frame == s_lateUpdateFrame)
            {
                s_lateUpdateSequence = 0;
            }
            else
            {
                s_lateUpdateSequence++;
            }

            data.sequence = s_lateUpdateSequence;
        }

        public static void Reset()
        {
            s_lateUpdateFrame = -1;
            s_lateUpdateSequence = 0;
        }
        
        private static int s_lateUpdateFrame = -1;
        private static int s_lateUpdateSequence = 0;
    }
}