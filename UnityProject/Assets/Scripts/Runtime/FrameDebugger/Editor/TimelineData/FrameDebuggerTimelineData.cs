using System.Collections.Generic;

namespace UnityChain.FrameDebugger
{
    public class FrameDebuggerTimelineData
    {
        public void AddChild(ulong uid)
        {
            if (children.Contains(uid))
            {
                return;
            }
            children.Add(uid);
        }

        public ulong uid;
        public ulong parent;
        public List<ulong> children = new List<ulong>();
        
        public FrameDebuggerTimelineID id;
        public ChainSnapshotData snapshotData;
    }
}