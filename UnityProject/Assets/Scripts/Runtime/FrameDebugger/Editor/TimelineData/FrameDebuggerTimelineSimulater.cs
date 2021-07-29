using System.Collections.Generic;

namespace UnityChain.FrameDebugger
{
    public class FrameDebuggerTimelineSimulater
    {
        public static void Simulate()
        {
            FrameDebuggerTimelineDataCollector.Clear();
            FrameDebuggerTimelineDataCollectorBuffer.Clear();
            
            List<UnityChainFrameDebuggerChain> chains = UnityChainFrameDebuggerScene.Chains;
            if (null == chains)
            {
                return;
            }

            foreach (var item in chains)
            {
                SimulateChain(item);
            }
        }

        private static void SimulateChain(UnityChainFrameDebuggerChain debuggerChain)
        {
            if (null == debuggerChain)
            {
                return;
            }

            UnityChain.Chain chain = debuggerChain.Chian;
            if (null == chain)
            {
                return;
            }

            int chainUID = debuggerChain.UID;
            FrameDebuggerChainRuntimeData chainRuntimeData = FrameDebuggerRuntimeDataCollector.GetChainRuntimeData(chainUID);
            if (null == chainRuntimeData || chainRuntimeData.FrameCount < 1)
            {
                return;
            }

            int start = chainRuntimeData.BeginFrame;
            int end = chainRuntimeData.EndFrame;
            for (int i = start; i <= end; i++)
            {
                FrameDebuggerChainRuntimeFrameData frameData = chainRuntimeData.GetFrameData(i);
                if (null == frameData)
                {
                    continue;
                }
                
                FrameDebuggerTimelineDataCollectorBuffer.SetInctanceID(chainUID);

                SimulateFrame(chain, frameData);
                
                FrameDebuggerTimelineDataCollectorBuffer.Flush();
            }
        }

        private static void SimulateFrame(UnityChain.Chain chain, FrameDebuggerChainRuntimeFrameData frameData)
        {
            if (null == frameData)
            {
                return;
            }

            List<RuntimeData> runtime = frameData.Data;

            foreach (var item in runtime)
            {
                if (item.id == RuntimeDataID.Update)
                {
                    chain.ReadData(item as UpdateSynchronousData);
                    SnapshotTimelineData(chain, FrameDebuggerTimelineID.Update, frameData.Frame);
                }
                else if (item.id == RuntimeDataID.FixedUpdate)
                {
                    chain.ReadData(item as FixedUpdateSynchronousData,
                        () => { SnapshotTimelineData(chain, FrameDebuggerTimelineID.Simulate, frameData.Frame); },
                        () => { SnapshotTimelineData(chain, FrameDebuggerTimelineID.ApplyConstraint, frameData.Frame);},
                        () => { SnapshotTimelineData(chain, FrameDebuggerTimelineID.AdjustCollisions, frameData.Frame);},
                        () => { SnapshotTimelineData(chain, FrameDebuggerTimelineID.SetAngles, frameData.Frame);}
                    );
                    
                    SnapshotTimelineData(chain, FrameDebuggerTimelineID.FixedUpdate, frameData.Frame);
                }
            }
        }

        private static void SnapshotTimelineData(UnityChain.Chain chain, FrameDebuggerTimelineID id, int frame)
        {
            ChainSnapshotData chainSnapshotData = chain.CaptureChainSnapshotData();
            chainSnapshotData.frame = frame;
            FrameDebuggerTimelineDataCollectorBuffer.Add(chainSnapshotData, id, frame);
        }
    }
}