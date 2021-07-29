namespace UnityChain.FrameDebugger
{
	public class RuntimeData
	{
		public virtual byte[] Serialize()
		{
			int size = Size();
			UnityChain.FrameDebugger.ByteBuffer byteBuffer = new UnityChain.FrameDebugger.ByteBuffer(size);
			OnSerialize(byteBuffer);
			return byteBuffer.Bytes;
		}

		public virtual void DeSerialize(UnityChain.FrameDebugger.ByteBuffer byteBuffer)
		{
			OnDeSerialize(byteBuffer);
		}

		protected virtual void OnSerialize(UnityChain.FrameDebugger.ByteBuffer byteBuffer)
		{
			byteBuffer.WriteInt((int)id);
			byteBuffer.WriteInt(uid);
			byteBuffer.WriteInt(frame);
		}

		protected virtual void OnDeSerialize(UnityChain.FrameDebugger.ByteBuffer byteBuffer)
		{
			id = (RuntimeDataID)byteBuffer.ReadInt();
			uid = byteBuffer.ReadInt();
			frame = byteBuffer.ReadInt();
		}

		protected virtual int Size()
		{
			return 12;
		}

		public RuntimeDataID id;
		public int uid;
		public int frame;
	}
}