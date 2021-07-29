using System.Collections.Generic;

namespace UnityChain.FrameDebugger
{
    public class FrameDebuggerTimelineDataCollector
    {
        public static ulong Add(int instanceID, ChainSnapshotData snapshotData, FrameDebuggerTimelineID id, ulong parentUID)
        {
            FrameDebuggerChainTimelineData chainTimelineData = GetData(instanceID, true);
            return chainTimelineData.Add(snapshotData, id, parentUID);
        }

        public static FrameDebuggerChainTimelineData GetData(int id)
        {
            return GetData(id, false);
        }
        
        public static FrameDebuggerChainTimelineData GetData(int id, bool instanceIfNotFind = false)
        {
            FrameDebuggerChainTimelineData data = null;
            if (s_instanceID2DataCollection.TryGetValue(id, out data))
            {
                return data;
            }

            if (instanceIfNotFind)
            {
                data = new FrameDebuggerChainTimelineData();
                s_instanceID2DataCollection.Add(id, data);
            }
            
            return data;
        }

        public static void Clear()
        {
            s_instanceID2DataCollection.Clear();
        }

        private static Dictionary<int, FrameDebuggerChainTimelineData> s_instanceID2DataCollection = new Dictionary<int, FrameDebuggerChainTimelineData>();
    }
}