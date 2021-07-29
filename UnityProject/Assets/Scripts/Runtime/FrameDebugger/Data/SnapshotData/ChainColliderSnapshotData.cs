using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class ChainColliderSnapshotData
    {
        public static void Serilize(ChainColliderSnapshotData data, ByteBuffer byteBuffer)
        {
            if (null == data)
            {
                byteBuffer.WriteBool(false);
                return;
            }
            
            byteBuffer.WriteBool(true);
            byteBuffer.WriteVector3(data.center);
            byteBuffer.WriteVector3(data.size);
        }

        public static void DeSerialize(ref ChainColliderSnapshotData data, ByteBuffer byteBuffer)
        {
            bool flag = byteBuffer.ReadBool();
            if (!flag)
            {
                data = null;
                return;
            }
            
            data = new ChainColliderSnapshotData();
            data.center = byteBuffer.ReadVector3();
            data.size = byteBuffer.ReadVector3();
        }

        public static int Size(ChainColliderSnapshotData data)
        {
            if (null == data)
            {
                return 1;
            }

            return 25;
        }

        public Vector3 center;
        public Vector3 size;
    }
}