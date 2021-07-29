using System.Collections.Generic;

namespace UnityChain.FrameDebugger
{
    public class UnityChainFrameDebuggerScene
    {
        public static void Load()
        {
            Clear();
            
            List<int> ids = FrameDebuggerRuntimeDataCollector.GetInstanceUIDs();

            foreach (var id in ids)
            {
                Instantiate(id);
            }
        }
        
        public static void Clear()
        {
            foreach (var item in s_chains)
            {
                item.Destory();
            }
            
            s_chains.Clear();
        }

        public static UnityChainFrameDebuggerChain GetChain(int id)
        {
            foreach (var item in s_chains)
            {
                if (item.UID == id)
                {
                    return item;
                }
            }

            return null;
        }

        private static void Instantiate(int id)
        {
            FrameDebuggerChainRuntimeData chainRuntimeData = FrameDebuggerRuntimeDataCollector.GetChainRuntimeData(id);
            if (null == chainRuntimeData)
            {
                return;
            }
            
            UnityChainFrameDebuggerChain chain = new UnityChainFrameDebuggerChain(id);
            chain.Instantiate(chainRuntimeData.SnapshotData);
            
            s_chains.Add(chain);
        }

        public static List<UnityChainFrameDebuggerChain> Chains
        {
            get { return s_chains; }
        }

        private static List<UnityChainFrameDebuggerChain> s_chains = new List<UnityChainFrameDebuggerChain>();
    }
}