using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class UnityChainFrameDebuggerMonoer : MonoBehaviour
    {
        private void Update()
        {
            if (null == updateCallback)
            {
                return;
            }
            
            updateCallback.Invoke(Time.frameCount);
        }

        public System.Action<float> updateCallback;
    }
}