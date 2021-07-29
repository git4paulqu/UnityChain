using System.Collections.Generic;

namespace UnityChain.FrameDebugger
{
    public class FixedUpdateSynchronousData : RuntimeData
    {
        public FixedUpdateSynchronousData()
        {
            id = RuntimeDataID.FixedUpdate;
        }

        protected override void OnSerialize(ByteBuffer byteBuffer)
        {
            base.OnSerialize(byteBuffer);
            
            byteBuffer.WriteFloat(deltaTime);
            byteBuffer.WriteInt(sequence);
            ChainSynchronousData.Serialize(chainData, byteBuffer);
            ParticleSynchronousData.Serialize(particleDatas, byteBuffer);
        }

        protected override void OnDeSerialize(ByteBuffer byteBuffer)
        {
            base.OnDeSerialize(byteBuffer);

            deltaTime = byteBuffer.ReadFloat();
            sequence = byteBuffer.ReadInt();
            chainData = ChainSynchronousData.DeSerialize(byteBuffer);
            ParticleSynchronousData.DeSerialize(ref particleDatas, byteBuffer);
        }

        protected override int Size()
        {
            int size = base.Size();
            size += 8;
            size += ChainSynchronousData.Size(chainData);
            size += ParticleSynchronousData.Size(particleDatas);
            return size;
        }

        public float deltaTime;
        public int sequence;
        public ChainSynchronousData chainData;
        public List<ParticleSynchronousData> particleDatas;
    }
}