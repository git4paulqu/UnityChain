using UnityEngine;

namespace UnityChain
{
    //    3-----7
    //   /     /|
    //  2-----6 |
    //  | 1   | 5
    //  |     |/
    //  0-----4
    public class CubeVertexs
    {
        public static CubeVertexs Buffer
        {
            get
            {
                if (null == m_buffer)
                {
                    m_buffer = new CubeVertexs();
                }

                return m_buffer;
            }
        }

        private static CubeVertexs m_buffer;
        
        public Vector3 this[int index] {
            get
            {
                return m_vertexs[index];
            }

            set
            {
                m_vertexs[index] = value;
            }
        }

        public int Count
        {
            get { return m_vertexs.Length; }
        }

        private Vector3[] m_vertexs = new Vector3[8];
    }
}