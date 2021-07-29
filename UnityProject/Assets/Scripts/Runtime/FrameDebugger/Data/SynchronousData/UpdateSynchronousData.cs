namespace UnityChain.FrameDebugger
{
    public class UpdateSynchronousData : RuntimeData
    {
        public UpdateSynchronousData()
        {
            id = RuntimeDataID.Update;
        }

        protected override void OnSerialize(ByteBuffer byteBuffer)
        {
            base.OnSerialize(byteBuffer);
            
            byteBuffer.WriteFloat(deltaTime);
        }

        public override void DeSerialize(ByteBuffer byteBuffer)
        {
            base.DeSerialize(byteBuffer);

            deltaTime = byteBuffer.ReadFloat();
        }

        protected override int Size()
        {
            int size = base.Size();
            size += 4;
            return size;
        }

        public float deltaTime;
    }
}