using System.Collections.Generic;

namespace UnityChain.FrameDebugger
{
    public class FrameDebuggerChainRuntimeFrameData
    {
        public FrameDebuggerChainRuntimeFrameData(int frame)
        {
            Frame = frame;
        }

        public void Add(RuntimeData data)
        {
            m_datas.Add(data);
        }

        public List<RuntimeData> Data
        {
            get { return m_datas; }
        }

        public int Frame { get; private set; }
        private List<RuntimeData> m_datas = new List<RuntimeData>();
    }
}