using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class UnityChainFrameDebuggerInspector
    {
        public void OnEnable()
        {
            
        }

        public void DoGUI(Rect rect)
        {
            GUI.Box(rect, string.Empty);
            
            if (UnityChainFrameDebuggerWindowGUIContext.chainUID <= 0)
            {
                return;
            }

            if (UnityChainFrameDebuggerWindowGUIContext.frame < 0)
            {
                return;
            }

            FrameDebuggerTimelineData timelineData = UnityChainFrameDebuggerWindowGUIContext.GetTimelineData();
            if (null == timelineData)
            {
                return;
            }

            ChainSnapshotData chainSnapshotData = timelineData.snapshotData;
            if (null == chainSnapshotData)
            {
                return;
            }

            DrawDebug(ref rect, chainSnapshotData);
            
            bool focusChain = UnityChainFrameDebuggerWindowGUIContext.chainLocalID == -1;

            if (focusChain)
            {
                ChainGUI(rect, chainSnapshotData);
            }
            else
            {
                ParticleGUI(rect, chainSnapshotData);
            }
        }

        private void DrawDebug(ref Rect rect, ChainSnapshotData chainSnapshotData)
        {
            Rect labelRect = new Rect(rect.x, rect.y, rect.width, 20);
            GUI.Label(labelRect, string.Format("UID:{0}", chainSnapshotData.uid));
            rect.y += 20;
        }

        private void ChainGUI(Rect rect, ChainSnapshotData chainSnapshotData)
        {
            Rect labelRect = new Rect(rect.x, rect.y, rect.width, 20);
            GUI.Label(labelRect, string.Format("name:{0}", chainSnapshotData.name));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("position:{0}", FormatVector(chainSnapshotData.position)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("scale:{0}", FormatVector(chainSnapshotData.scalse)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("rotation:{0}", FormatQuaternion(chainSnapshotData.rotation)));

            if (null == chainSnapshotData.bounds)
            {
                return;
            }
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("bounds size:{0}", FormatVector(chainSnapshotData.bounds.size)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("bounds center:{0}", FormatVector(chainSnapshotData.bounds.center)));
        }

        private void ParticleGUI(Rect rect, ChainSnapshotData chainSnapshotData)
        {
            int index = UnityChainFrameDebuggerWindowGUIContext.chainLocalID;
            ParticleSnapshotData particleSnapshotData = chainSnapshotData.GetParticleSnapshotData(index);
            if (null == particleSnapshotData)
            {
                return;
            }
            
            Rect labelRect = new Rect(rect.x, rect.y, rect.width, 20);
            GUI.Label(labelRect, string.Format("name:{0}", particleSnapshotData.name));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("Index:{0}", particleSnapshotData.Index));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("LinkDistance:{0:N7}", particleSnapshotData.LinkDistance));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("transform position:{0}", FormatVector(particleSnapshotData.transformPosition)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("transform scale:{0}", FormatVector(particleSnapshotData.transformScalse)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("transform rotation:{0}", FormatQuaternion(particleSnapshotData.transformRotation)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("velocityDampen:{0:N7}", particleSnapshotData.velocityDampen));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("stiffness:{0:N7}", particleSnapshotData.stiffness));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("gravity:{0}", FormatVector(particleSnapshotData.gravity)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("PrePosition:{0}", FormatVector(particleSnapshotData.PrePosition)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("Position:{0}", FormatVector(particleSnapshotData.Position)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("Rotation:{0}", FormatQuaternion(particleSnapshotData.Rotation)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("Force:{0}", FormatVector(particleSnapshotData.Force)));
             
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("IsCollide:{0}", particleSnapshotData.IsCollide));
            
            if (null == particleSnapshotData.particleCollider)
            {
                return;
            }
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("bounds size:{0}", FormatVector(particleSnapshotData.particleCollider.size)));
            
            labelRect.y += 20;
            GUI.Label(labelRect, string.Format("bounds center:{0}", FormatVector(particleSnapshotData.particleCollider.center)));
        }

        private string FormatVector(Vector3 vector3)
        {
            return string.Format("x:{0:N7}, y:{1:N7}, z:{2:N7}", vector3.x, vector3.y, vector3.z);
        }
        
        private string FormatQuaternion(Quaternion rotation)
        {
            return string.Format("x:{0:N7}, y:{1:N7}, z:{2:N7}, w:{3:N7}", rotation.x, rotation.y, rotation.z, rotation.w);
        }
    }
}