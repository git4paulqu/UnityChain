using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class FrameDebuggerChainTimelineData
    {
        public FrameDebuggerChainTimelineData()
        {
            Clear();  
        }

        public ulong Add(ChainSnapshotData snapshotData, FrameDebuggerTimelineID id, ulong parentUID)
        {
            if (null == snapshotData)
            {
                return 0;
            }

            ulong uid = InternalAdd(snapshotData, id, parentUID);
            return uid;
        }
        
        public ulong GetUpdateUIDFromeFrame(int frame)
        {
            ulong uid = 0;
            m_frame2UpdateUIDCollection.TryGetValue(frame, out uid);
            return uid;
        }

        public FrameDebuggerTimelineData GetData(ulong uid)
        {
            FrameDebuggerTimelineData data = null;
            m_uid2DataCollection.TryGetValue(uid, out data);
            return data;
        }

        public List<FrameDebuggerTimelineData> GetChildrenData(ulong uid)
        {
            FrameDebuggerTimelineData parent = GetData(uid);
            if (null == parent)
            {
                return null;
            }

            int count = parent.children.Count;
            List<FrameDebuggerTimelineData>  children = new List<FrameDebuggerTimelineData>(count);
            for (int i = 0; i < count; i++)
            {
                FrameDebuggerTimelineData data = GetData(parent.children[i]);
                if (null == data)
                {
                    continue;
                }
                
                children.Add(data);
            }

            return children;
        }

        public void Clear()
        {
            m_uid = 0;
            m_frame2UpdateUIDCollection.Clear();
            m_uid2DataCollection.Clear();

            BeginFrame = int.MaxValue;
            EndFrame = int.MinValue;
        }

        private ulong InternalAdd(ChainSnapshotData snapshotData, FrameDebuggerTimelineID id, ulong parentUID)
        {
            m_uid++;

            ulong dataUID = m_uid;
            
            FrameDebuggerTimelineData timelineData = new FrameDebuggerTimelineData();
            timelineData.uid = dataUID;
            timelineData.parent = parentUID;
            timelineData.id = id;
            timelineData.snapshotData = snapshotData;

            m_uid2DataCollection.Add(dataUID, timelineData);
            
            int frame = snapshotData.frame;
            RecaculateFrame(frame);
            
            if (id == FrameDebuggerTimelineID.Update)
            {
                m_frame2UpdateUIDCollection[frame] = dataUID;
            }
            
            FrameDebuggerTimelineData parent = GetData(parentUID);
            if (null == parent)
            {
                return dataUID;
            }

            parent.AddChild(dataUID);
            return dataUID;
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

        private ulong m_uid = 0;
        private Dictionary<int, ulong> m_frame2UpdateUIDCollection = new Dictionary<int, ulong>();
        private Dictionary<ulong, FrameDebuggerTimelineData> m_uid2DataCollection = new Dictionary<ulong, FrameDebuggerTimelineData>();
    }
}