using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace UnityChain.FrameDebugger
{
    public class UnityChainFrameDebuggerClientProxy
    {
        public static void HandleREC(bool enable)
        {
            if (enable)
            {
                PreHandleREC();
            }
            else
            {
                SendMessage(UnityChainFrameDebuggerProtocolID.C2S_Analyze, new byte[0]);
            }

            UnityChainRuntimeDefine.EnablFrameDebuggerCollectData = enable;
        }

        public static void Snapshot()
        {
            UnityChain.Chain[] chains = UnityChain.ChainScene.GetChains();
            if (null == chains)
            {
                return;
            }

            foreach (var item in chains)
            {
                SnapshotChain(item);
            }
        }

        public static void SynchronousUpdateData(UpdateSynchronousData data)
        {
            SynchronousRuntimeData(data);
        }

        public static void SynchronousFixedUpdateData(FixedUpdateSynchronousData data)
        {
            if (null == data)
            {
                return;
            }
            
            FixedUpdateSynchronousDataState.PreSynchronous(data);
            SynchronousRuntimeData(data);
        }

        public static void SendMessage(Guid protocolID, byte[] message)
        {
#if UNITY_EDITOR
            UnityChainFrameDebuggerServerProxy.ProcessReceiveData(protocolID, message);
#else
            int count = null == message ? 0 : message.Length;
            UnityChainLogger.Log("[UnityChainFrameDebuggerClientProxy] SendMessage:{0}, count:{1}", protocolID, count);
            PlayerConnection.instance.Send(protocolID, message);
#endif
        }

        private static void PreHandleREC()
        {
            FixedUpdateSynchronousDataState.Reset();   
            
            Snapshot();
        }

        private static void SnapshotChain(UnityChain.Chain chain)
        {
            if (null == chain)
            {
                return;
            }

            ChainSnapshotData chainSnapshotData = chain.CaptureChainSnapshotData();
            if (null == chainSnapshotData)
            {
                return;
            }
            
            byte[] message = chainSnapshotData.Serialize();
            SendMessage(UnityChainFrameDebuggerProtocolID.C2S_Snapshot, message);
        }

        private static void SynchronousRuntimeData(RuntimeData runtimeData)
        {
            if (null == runtimeData)
            {
                return;
            }
            
            byte[] message = runtimeData.Serialize();
            SendMessage(UnityChainFrameDebuggerProtocolID.C2S_Synchronous, message);
        }
    }
}