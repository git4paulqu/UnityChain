using System.Collections.Generic;

namespace UnityChain.FrameDebugger
{
    public class FrameDebuggerTimelineDataCollectorBuffer
    {
        public static void SetInctanceID(int intanceID)
        {
            s_instanceID = intanceID;
        }

        public static void Add(ChainSnapshotData snapshotData, FrameDebuggerTimelineID id, int frame)
        {
            if (id == FrameDebuggerTimelineID.Unknown)
            {
                return;
            }
            
            BuffObject buffObject = new BuffObject();
            buffObject.snapshotData = snapshotData;
            buffObject.id = id;

            if (id == FrameDebuggerTimelineID.Update)
            {
                s_updateData = buffObject;
                return;
            }

            bool create = false;
            if (s_fixedUpdateFrame != frame)
            {
                create = true;
                s_fixedUpdateFrame = frame;
            }

            FixedUpdateFrameBuffer frameBuffer = GetFixedUpdateFrameBuffer(create);
            if (id == FrameDebuggerTimelineID.FixedUpdate)
            {
                frameBuffer.fixedUpdateData = buffObject;
            }
            else
            {
                frameBuffer.datas.Add(buffObject);
            }
        }

        public static void Flush()
        {
            Commit();

            Clear();
        }

        private static void Commit()
        {
            if (null == s_updateData)
            {
                return;
            }

            ulong updateID = FrameDebuggerTimelineDataCollector.Add(s_instanceID, s_updateData.snapshotData, s_updateData.id, 0);

            if (null == s_fixedUpdateDatas)
            {
                return;
            }

            foreach (var fixedUpdateFrameBuffer in s_fixedUpdateDatas)
            {
                BuffObject fixedUpdateData = fixedUpdateFrameBuffer.fixedUpdateData;
                if (null == fixedUpdateData)
                {
                    continue;
                }

                ulong fixedUpdateID = FrameDebuggerTimelineDataCollector.Add(s_instanceID, fixedUpdateData.snapshotData, fixedUpdateData.id, updateID);

                if (null == fixedUpdateFrameBuffer.datas)
                {
                    continue;
                }
                
                foreach (var buffObject in fixedUpdateFrameBuffer.datas)
                {
                    FrameDebuggerTimelineDataCollector.Add(s_instanceID, buffObject.snapshotData, buffObject.id, fixedUpdateID);
                }
            }
        }

        public static void Clear()
        {
            s_instanceID = 0;
            s_fixedUpdateFrame = -1;
            s_updateData = null;
            s_fixedUpdateDatas.Clear();
        }

        private static FixedUpdateFrameBuffer GetFixedUpdateFrameBuffer(bool create)
        {
            if (!create)
            {
                return s_fixedUpdateDatas[s_fixedUpdateDatas.Count - 1];
            }
            
            FixedUpdateFrameBuffer frameBuffer = new FixedUpdateFrameBuffer();
            s_fixedUpdateDatas.Add(frameBuffer);
            return frameBuffer;
        }

        private static int s_instanceID = 0;
        private static int s_fixedUpdateFrame = -1;
        private static BuffObject s_updateData;
        private static List<FixedUpdateFrameBuffer> s_fixedUpdateDatas = new List<FixedUpdateFrameBuffer>();
        
        private class FixedUpdateFrameBuffer
        {
            public BuffObject fixedUpdateData;
            public List<BuffObject> datas = new List<BuffObject>();
        }
        
        private class BuffObject
        {
            public ChainSnapshotData snapshotData;
            public FrameDebuggerTimelineID id;
        }
    }
}