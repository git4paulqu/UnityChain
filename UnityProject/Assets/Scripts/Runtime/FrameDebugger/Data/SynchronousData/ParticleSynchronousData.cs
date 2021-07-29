using System.Collections.Generic;
using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class ParticleSynchronousData
    {
        public static void Serialize(List<ParticleSynchronousData> data, ByteBuffer byteBuffer)
        {
            if (null == data)
            {
                byteBuffer.WriteInt(0);
                return;
            }
            
            byteBuffer.WriteInt(data.Count);

            foreach (var item in data)
            {
                item.Serialize(byteBuffer);
            }
        }
        
        public static void DeSerialize(ref List<ParticleSynchronousData> datas, ByteBuffer byteBuffer)
        {
            int count = byteBuffer.ReadInt();
            datas = new List<ParticleSynchronousData>();

            for (int i = 0; i < count; i++)
            {
                ParticleSynchronousData data = new ParticleSynchronousData();
                data.DeSerialize(byteBuffer);
                datas.Add(data);
            }
        }
        
        public void Serialize(ByteBuffer byteBuffer)
        {
            byteBuffer.WriteInt(index);
            byteBuffer.WriteVector3(force);
        }

        public void DeSerialize(ByteBuffer byteBuffer)
        {
            index = byteBuffer.ReadInt();
            force = byteBuffer.ReadVector3();
        }

        public static int Size(List<ParticleSynchronousData> data)
        {
            if (null == data)
            {
                return 4;
            }

            return 4 + data.Count * (16);
        }

        public int index;
        public Vector3 force;
    }
}