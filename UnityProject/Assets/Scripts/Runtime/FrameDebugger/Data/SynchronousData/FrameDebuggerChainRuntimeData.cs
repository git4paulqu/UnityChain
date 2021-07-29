using System;
using System.Collections.Generic;

namespace UnityChain.FrameDebugger
{
    public class FrameDebuggerChainRuntimeData
    {
        public FrameDebuggerChainRuntimeData(int uid)
        {
            UID = uid;
            Reset();
        }
        public void Add(RuntimeData data)
        {
            if (null == data)
            {
                return;
            }

            if (data.id == RuntimeDataID.Snapshot)
            {
                SnapshotData = data as ChainSnapshotData;
                return;
            }
            
            int frame = data.frame;
            RecaculateFrame(frame);

            FrameDebuggerChainRuntimeFrameData frameData = GetFrameData(frame, true);
            frameData.Add(data);
        }

        public FrameDebuggerChainRuntimeFrameData GetFrameData(int frame)
        {
            return GetFrameData(frame, false);
        }

        public void Reset()
        {
            BeginFrame = int.MaxValue;
            EndFrame = int.MinValue;
            m_frame2Data.Clear();
        }
        
        public FrameDebuggerChainRuntimeFrameData GetFrameData(int frame, bool createIfNotFind)
        {
            FrameDebuggerChainRuntimeFrameData data = null;
            if ( m_frame2Data.TryGetValue(frame, out data))
            {
                return data;
            }

            if (createIfNotFind)
            {
                data = new FrameDebuggerChainRuntimeFrameData(frame);
                m_frame2Data.Add(frame, data);
            }
            
            return data;
        }

        private void RecaculateFrame(int frame)
        {
            BeginFrame = Math.Min(frame, BeginFrame);
            EndFrame = Math.Max(frame, EndFrame);
        }

        public int BeginFrame { get; private set; }
        public int EndFrame { get; private set; }

        public int FrameCount
        {
            get
            {
                int frame = EndFrame - BeginFrame;
                frame = Math.Max(0, frame);
                return frame;
            }
        }

        public int UID { get; private set; }
        public ChainSnapshotData SnapshotData { get; private set; }
        
        private Dictionary<int, FrameDebuggerChainRuntimeFrameData> m_frame2Data = new Dictionary<int, FrameDebuggerChainRuntimeFrameData>();
    }
}