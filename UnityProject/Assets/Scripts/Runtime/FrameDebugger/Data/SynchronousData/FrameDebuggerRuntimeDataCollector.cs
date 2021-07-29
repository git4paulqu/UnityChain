using System.Collections.Generic;

namespace UnityChain.FrameDebugger
{
    public class FrameDebuggerRuntimeDataCollector
    {
        public static void Add(RuntimeData runtimeData)
        {
            if (null == runtimeData)
            {
                return;
            }

            int uid = runtimeData.uid;
            FrameDebuggerChainRuntimeData data = GetChainRuntimeData(uid, true);
            data.Add(runtimeData);
        }

        public static FrameDebuggerChainRuntimeData GetChainRuntimeData(int uid)
        {
            FrameDebuggerChainRuntimeData data = GetChainRuntimeData(uid, false);
            return data;
        }

        public static List<int> GetInstanceUIDs()
        {
            List<int> ids = new List<int>();
            foreach (var pair in s_uid2Datas)
            {
                ids.Add(pair.Key);
            }

            return ids;
        }

        public static void Clear()
        {
            s_uid2Datas.Clear();
        }

        private static FrameDebuggerChainRuntimeData GetChainRuntimeData(int uid, bool createIfNotFind)
        {
            FrameDebuggerChainRuntimeData chainRuntimeData = null;
            if (s_uid2Datas.TryGetValue(uid, out chainRuntimeData))
            {
                return chainRuntimeData;
            }

            if (createIfNotFind)
            {
                chainRuntimeData = new FrameDebuggerChainRuntimeData(uid);
                s_uid2Datas.Add(uid, chainRuntimeData);
            }

            return chainRuntimeData;
        }


        private static Dictionary<int, FrameDebuggerChainRuntimeData> s_uid2Datas = new Dictionary<int, FrameDebuggerChainRuntimeData>();
    }
}