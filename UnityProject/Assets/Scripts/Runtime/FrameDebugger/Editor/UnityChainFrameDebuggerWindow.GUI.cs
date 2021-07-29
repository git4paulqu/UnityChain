using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public partial class UnityChainFrameDebuggerWindow
    {
        private float progress = 0;
        private void OnGUI()
        {
           // progress = GUILayout.HorizontalSlider(progress, 0, 100);
           
            m_attachProfilerUI.DoGUI(GetAttachProfilerRect());
            m_hierarchy.DoGUI(GetHierarchyRect());
            m_inspector.DoGUI(GetInspectorRect());
            m_profiler.DoGUI(GetProfilerRect());
        }
        
        private Rect GetAttachProfilerRect()
        {
            Rect rect = this.position;
            return rect;
        }

        private Rect GetHierarchyRect()
        {
            Rect rect = this.position;
            Vector2 size = rect.size;
            size.y -= 20f;
            size.x *= (1f / 3f);
            size.y /= 2;
            rect.size = size;
            
            Vector3 position = Vector3.zero;
            position.y = 20f;
            rect.position = position;
            
            return rect;
        }
        
        private Rect GetInspectorRect()
        {
            Rect rect = this.position;
            Vector2 size = rect.size;
            size.y -= 20f;
            
            size.x *= (1f / 3f);
            size.y /= 2;
            rect.size = size;

            Vector3 position = Vector3.zero;
            position.y += (size.y + 20f);
            rect.position = position;
            return rect;
        }
        
        private Rect GetProfilerRect()
        {
            Rect rect = this.position;
            Vector2 size = rect.size;
            size.y -= 20f;
            
            size.x *= (2f / 3f);
            rect.size = size;

            Vector3 position = Vector3.zero;
            position.x += (size.x / 2f);
            position.y = 20f;
            rect.position = position;
            return rect;
        }
    }
}