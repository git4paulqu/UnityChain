using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class UnityChainFrameDebuggerChain
    {
        public UnityChainFrameDebuggerChain(int uid)
        {
            UID = uid;
        }

        public void Instantiate(ChainSnapshotData chainSnapshotData)
        {
            UnityChain.Chain chain = UnityChain.Chain.Instantiate(chainSnapshotData);

            Chian = null;
            if (null != chain)
            {
                gameObject = chain.gameObject;
                Chian = gameObject.GetComponent<UnityChain.Chain>();
                Chian.enabled = false;
            }
        }

        public void Destory()
        {
            UID = 0;
            
            if (null != gameObject)
            {
                GameObject.DestroyImmediate(gameObject);    
            }
        }

        public UnityChain.Chain Chian { get; private set; }

        public int UID { get; private set; }

        public GameObject gameObject { get; private set; }
        
    }
}