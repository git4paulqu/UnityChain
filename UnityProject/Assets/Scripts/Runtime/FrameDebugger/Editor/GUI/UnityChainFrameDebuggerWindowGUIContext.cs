using UnityEditor.Callbacks;

namespace UnityChain.FrameDebugger
{
    public static class UnityChainFrameDebuggerWindowGUIContext
    {
        public static void Clear()
        {
            chainUID = 0;
            chainLocalID = int.MinValue;
            timelineUID = 0;
            frame = -1;
        }

        public static FrameDebuggerChainTimelineData GetChainTimelineData()
        {
            return FrameDebuggerTimelineDataCollector.GetData(chainUID);
        }

        public static FrameDebuggerTimelineData GetTimelineData()
        {
            FrameDebuggerChainTimelineData chainTimelineData = GetChainTimelineData();
            if (null == chainTimelineData)
            {
                return null;
            }

            return chainTimelineData.GetData(timelineUID);
        }

        public static ChainSnapshotData GetFocusChainSnapshotData()
        {
            FrameDebuggerTimelineData timelineData = GetTimelineData();
            if (null == timelineData)
            {
                return null;
            }

            return timelineData.snapshotData;
        }

        public static UnityChainFrameDebuggerChain GetFocusFrameDebuggerChain()
        {
            UnityChainFrameDebuggerChain frameDebuggerChain = UnityChainFrameDebuggerScene.GetChain(chainUID);
            return frameDebuggerChain;
        }
        
        public static UnityChain.Chain GetFocusChain()
        {
            UnityChainFrameDebuggerChain frameDebuggerChain = GetFocusFrameDebuggerChain();
            if (null == frameDebuggerChain)
            {
                return null;
            }
            return frameDebuggerChain.Chian;
        }

        public static int chainUID;
        public static int chainLocalID;
        public static ulong timelineUID;
        public static int frame;
    }
}