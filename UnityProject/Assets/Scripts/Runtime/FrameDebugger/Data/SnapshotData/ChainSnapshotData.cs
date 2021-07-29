using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class ChainSnapshotData : RuntimeData
    {
        public ChainSnapshotData()
        {
            id = RuntimeDataID.Snapshot;
        }

        protected override void OnSerialize(ByteBuffer byteBuffer)
        {
            base.OnSerialize(byteBuffer);
            
            byteBuffer.WriteString(name);
            byteBuffer.WriteVector3(position);
            byteBuffer.WriteVector3(scalse);
            byteBuffer.WriteQuaternion(rotation);
            
            byteBuffer.WriteInt((int)error);
            ChainColliderSnapshotData.Serilize(bounds, byteBuffer);
            ParticleSnapshotData.Serialize(particles, byteBuffer);
        }

        public override void DeSerialize(ByteBuffer byteBuffer)
        {
            base.DeSerialize(byteBuffer);

            name = byteBuffer.ReadString();
            position = byteBuffer.ReadVector3();
            scalse = byteBuffer.ReadVector3();
            rotation = byteBuffer.ReadQuaternion();

            error = (ChainError)byteBuffer.ReadInt();
            ChainColliderSnapshotData.DeSerialize(ref bounds, byteBuffer);
            ParticleSnapshotData.DeSerialize(ref particles, byteBuffer);
        }

        protected override int Size()
        {
            int size = base.Size();
        
            size += ByteBuffer.SizeOfString(name);
            size += 4;
            size += 44;

            size += ChainColliderSnapshotData.Size(bounds);
            size += ParticleSnapshotData.Size(particles);
            
            return size;
        }

        public ParticleSnapshotData GetParticleSnapshotData(int index)
        {
            if (null == particles)
            {
                return null;
            }

            if (index < 0 || index >= particles.Length)
            {
                return null;
            }

            return particles[index];
        }

        public string name;
        public Vector3 position;
        public Vector3 scalse;
        public Quaternion rotation;
        
        public ChainError error;
        public ChainColliderSnapshotData bounds;
        public ParticleSnapshotData[] particles;
    }
}