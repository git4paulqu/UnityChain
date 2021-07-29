using System;
using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class ByteBuffer
    {
        public ByteBuffer(byte[] bytes)
        {
            m_bytes = bytes;
            Count = bytes.Length;
            Seek(0);
        }

        public ByteBuffer(int capacity)
        {
            m_bytes = new byte[capacity];
            Count = capacity;
            Seek(0);
        }

        public void WriteBool(bool flag)
        {
            byte value = flag ? (byte) 1 : (byte) 0;
            m_bytes[Position] = value;
            Position += 1;
        }

        public void WriteInt(int value)
        {
            m_bytes[Position] = (byte)(value & 0xFF);
            m_bytes[Position + 1] = (byte)((value >> 8) & 0xFF);
            m_bytes[Position + 2] = (byte)((value >> 16) & 0xFF);
            m_bytes[Position + 3] = (byte)((value >> 24) & 0xFF);
            Position += 4;
        }

        public void WriteFloat(float value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            m_bytes[Position] = temp[0];
            m_bytes[Position + 1] = temp[1];
            m_bytes[Position + 2] = temp[2];
            m_bytes[Position + 3] = temp[3];
            Position += 4;
        }

        public void WriteVector3(Vector3 vector)
        {
            WriteFloat(vector.x);
            WriteFloat(vector.y);
            WriteFloat(vector.z);
        }
        
        public void WriteQuaternion(Quaternion quaternion)
        {
            WriteFloat(quaternion.x);
            WriteFloat(quaternion.y);
            WriteFloat(quaternion.z);
            WriteFloat(quaternion.w);
        }

        public void WriteString(string value)
        {
            int count = SizeOfString(value);
            WriteInt(count);
            byte[] bytes =  System.Text.Encoding.UTF8.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, m_bytes, Position, count);
            Position += (count);
        }

        public bool ReadBool()
        {
            byte value = m_bytes[Position];
            Position += 1;
            return value == (byte)1;
        }

        public int ReadInt()
        {
            int value = (int)((m_bytes[Position] & 0xFF) |
                             ((m_bytes[Position + 1] & 0xFF) << 8) |
                             ((m_bytes[Position + 2] & 0xFF) << 16) |
                             ((m_bytes[Position + 3] & 0xFF) << 24));
            Position += 4;
            return value;
        }

        public float ReadFloat()
        {
            m_temp[0] = m_bytes[Position];
            m_temp[1] = m_bytes[Position + 1];
            m_temp[2] = m_bytes[Position + 2];
            m_temp[3] = m_bytes[Position + 3];
            Position += 4;
            
            float value = BitConverter.ToSingle(m_temp, 0);
            return value;
        }

        public Vector3 ReadVector3()
        {
            Vector3 vector = Vector3.zero;
            vector.x = ReadFloat();
            vector.y = ReadFloat();
            vector.z = ReadFloat();
            return vector;
        }
        
        public Quaternion ReadQuaternion()
        {
            Quaternion quaternion = Quaternion.identity;
            quaternion.x = ReadFloat();
            quaternion.y = ReadFloat();
            quaternion.z = ReadFloat();
            quaternion.w = ReadFloat();
            return quaternion;
        }

        public string ReadString()
        {
            int count = ReadInt();
            string value = System.Text.Encoding.UTF8.GetString(m_bytes, Position, count);
            Position += count;
            return value;
        }

        public void Seek(int position)
        {
            Position = position;
        }
        
        public static int SizeOfString(string value)
        {
            byte[] bytes =  System.Text.Encoding.UTF8.GetBytes(value);
            int size = null == bytes ? 0 : bytes.Length;
            return size;
        }

        public int Position { get; private set; }

        public int Count { get; private set; }

        public byte[] Bytes
        {
            get { return m_bytes; }
        }

        private byte[] m_bytes;
        private byte[] m_temp = new byte[8];
    }
}