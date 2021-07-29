using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class ChainSynchronousData
    {
        public static void Serialize(ChainSynchronousData data, ByteBuffer byteBuffer)
        {
            if (null == data)
            {
                byteBuffer.WriteBool(false);
                return;
            }
            
            byteBuffer.WriteBool(true);
            byteBuffer.WriteInt((int)data.chainError);
            byteBuffer.WriteVector3(data.chainPosition);
            byteBuffer.WriteVector3(data.chainScale);
            byteBuffer.WriteQuaternion(data.chainRotation);
        }

        public static ChainSynchronousData DeSerialize(ByteBuffer byteBuffer)
        {
            bool flag = byteBuffer.ReadBool();
            if (!flag)
            {
                return null;
            }
            
            ChainSynchronousData data = new ChainSynchronousData();
            data.chainError = (ChainError)byteBuffer.ReadInt();
            data.chainPosition = byteBuffer.ReadVector3();
            data.chainScale = byteBuffer.ReadVector3();
            data.chainRotation = byteBuffer.ReadQuaternion();
            
            return data;
        }

        public static int Size(ChainSynchronousData data)
        {
            if (null == data)
            {
                return 1;
            }

            return 45;
        }

        public ChainError chainError;
        public Vector3 chainPosition;
        public Vector3 chainScale;
        public Quaternion chainRotation;
    }
}