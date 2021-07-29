namespace UnityChain.FrameDebugger
{
    public static class UnityChainFrameDebuggerSerializer
    {
        public static byte[] String2Bytes(string message)
        {
            return System.Text.Encoding.ASCII.GetBytes(message);
        }

        public static string Bytes2String(byte[] bytes)
        {
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static UnityChain.FrameDebugger.RuntimeData DeSerialize(byte[] bytes)
        {
            return InternalDeSerialize(bytes);
        }
        
        public static UnityChain.FrameDebugger.RuntimeData InternalDeSerialize(byte[] bytes)
        {
            UnityChain.FrameDebugger.ByteBuffer buffer = new ByteBuffer(bytes);
            UnityChain.FrameDebugger.RuntimeDataID id = (UnityChain.FrameDebugger.RuntimeDataID)buffer.ReadInt();
            buffer.Seek(0);
            
            RuntimeData data = GetRuntimeDataInstanceFromID(id);
            if (null == data)
            {
                return null;
            }
            
            data.DeSerialize(buffer);
            return data;
        }

        private static UnityChain.FrameDebugger.RuntimeData GetRuntimeDataInstanceFromID(UnityChain.FrameDebugger.RuntimeDataID id)
        {
            switch (id)
            {
                case RuntimeDataID.Update:
                    return  new UpdateSynchronousData();
                
                case RuntimeDataID.FixedUpdate:
                    return new FixedUpdateSynchronousData();
                
                case RuntimeDataID.Snapshot:
                    return new ChainSnapshotData();
                
                default:
                    return null;
            }
        }
    }
}