using System;

namespace UnityChain.FrameDebugger
{
    public class UnityChainFrameDebuggerServerProxy
    {
        public static void PreSynchronousData()
        {
            SetFlag(StateFlag.Recording);
            FrameDebuggerRuntimeDataCollector.Clear();
        }

        public static void ProcessReceiveData(Guid id, byte[] data)
        {
            InternalProcessReceiveData(id, data);
        }

        public static void ResetFlag()
        {
            SetFlag(StateFlag.Default);
        }

        private static void InternalProcessReceiveData(Guid id, byte[] data)
        {
            if (id == UnityChainFrameDebuggerProtocolID.C2S_Snapshot ||
                id == UnityChainFrameDebuggerProtocolID.C2S_Synchronous)
            {
                ProcessRuntimeData(id, data);
            }
            
            else if (id == UnityChainFrameDebuggerProtocolID.C2S_Analyze)
            {
                ProcessAnalyze();
            }
        }

        private static void ProcessRuntimeData(Guid id, byte[] data)
        {
            UnityChain.FrameDebugger.RuntimeData runtimeData = UnityChainFrameDebuggerSerializer.DeSerialize(data);
            FrameDebuggerRuntimeDataCollector.Add(runtimeData);
        }

        private static void ProcessAnalyze()
        {
            SetFlag(StateFlag.Analyze);
        }

        private static void SetFlag(StateFlag flag)
        {
            s_stateFlag = (int) flag;
        }

        private static bool IsEqualState(StateFlag flag)
        {
            return ((int) flag & s_stateFlag) > 0;
        }

        public static bool IsSynchronousData
        {
            get { return IsEqualState(StateFlag.Recording); }
        }

        public static bool IsWaitAnalyze
        {
            get { return IsEqualState(StateFlag.Analyze); }
        }

        private static int s_stateFlag;
        
        private enum StateFlag
        {
            Default         = 0,
            Recording       = 1,
            Analyze         = 1 << 1,
        }
    }
}