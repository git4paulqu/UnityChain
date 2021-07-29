using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class ParticleSnapshotData : RuntimeData
    {
        public static void Serialize(ParticleSnapshotData[] data, ByteBuffer byteBuffer)
        {
            if (null == data)
            {
                byteBuffer.WriteInt(0);
                return;
            }
            
            byteBuffer.WriteInt(data.Length);
            foreach (var item in data)
            {
                item.Serialize(byteBuffer);
            }
        }

        public static void DeSerialize(ref ParticleSnapshotData[] data, ByteBuffer byteBuffer)
        {
            int count = byteBuffer.ReadInt();
            data = new ParticleSnapshotData[count];
            if (count == 0)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                ParticleSnapshotData item = new ParticleSnapshotData();
                item.DeSerialize(byteBuffer);
                data[i] = item;
            }
        }

        public static int Size(ParticleSnapshotData[] data)
        {
            if (null == data)
            {
                return 4;
            }

            int size = 4;
            foreach (var item in data)
            {
                size += item.Size();
            }

            return size;
        }

        private void Serialize(ByteBuffer byteBuffer)
        {
            byteBuffer.WriteString(name);
            
            //40
            byteBuffer.WriteVector3(transformPosition);
            byteBuffer.WriteVector3(transformScalse);
            byteBuffer.WriteQuaternion(transformRotation);
            
            //60
            byteBuffer.WriteFloat(velocityDampen);
            byteBuffer.WriteFloat(stiffness);
            byteBuffer.WriteVector3(gravity);
            
            //121
            byteBuffer.WriteInt(Index);
            byteBuffer.WriteFloat(LinkDistance);
            byteBuffer.WriteVector3(PrePosition);
            byteBuffer.WriteVector3(Position);
            byteBuffer.WriteQuaternion(Rotation);
            byteBuffer.WriteVector3(Force);
            byteBuffer.WriteBool(IsCollide);
            
            ChainColliderSnapshotData.Serilize(particleCollider, byteBuffer);
        }

        public void DeSerialize(ByteBuffer byteBuffer)
        {
            name = byteBuffer.ReadString();
            
            //40
            transformPosition = byteBuffer.ReadVector3();
            transformScalse = byteBuffer.ReadVector3();
            transformRotation = byteBuffer.ReadQuaternion();
            
            //60
            velocityDampen = byteBuffer.ReadFloat();
            stiffness = byteBuffer.ReadFloat();
            gravity = byteBuffer.ReadVector3();
            
            //121
            Index = byteBuffer.ReadInt();
            LinkDistance = byteBuffer.ReadFloat();
            PrePosition = byteBuffer.ReadVector3();
            Position = byteBuffer.ReadVector3();
            Rotation = byteBuffer.ReadQuaternion();
            Force = byteBuffer.ReadVector3();
            IsCollide = byteBuffer.ReadBool();
            
            ChainColliderSnapshotData.DeSerialize(ref particleCollider, byteBuffer);
        }

        public int Size()
        {
            int size = ByteBuffer.SizeOfString(name);
            size += 4;
            
            size += 121;
            size += ChainColliderSnapshotData.Size(particleCollider);
            return size;
        }

        public string name;
        public Vector3 transformPosition;
        public Vector3 transformScalse;
        public Quaternion transformRotation;
        
        public float velocityDampen;
        public float stiffness;
        public Vector3 gravity;

        public int Index;
        public float LinkDistance;
        public Vector3 PrePosition;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Force;
        public bool IsCollide;
        
        public ChainColliderSnapshotData particleCollider;
    }
}