using UnityEditor;
using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public partial class UnityChainFrameDebuggerWindow : EditorWindow
    {
        [MenuItem("UnityChain/FrameDebug")]
        static void Init()
        {
            UnityChainFrameDebuggerWindow window = (UnityChainFrameDebuggerWindow)EditorWindow.GetWindow(typeof(UnityChainFrameDebuggerWindow));
            window.titleContent = new GUIContent("Chain.Debug");
            window.Show();
            window.OnInit();
        }

        private void OnInit()
        {
            GameObject monoer = new GameObject();
            m_monoer = monoer.AddComponent<UnityChainFrameDebuggerMonoer>();
            m_monoer.name = "UnityChainFrameDebuggerMonoer";
            m_monoer.updateCallback = OnUpdateCallback;
        }

        private void OnDestroy()
        {
            if (null != m_monoer)
            {
                GameObject.DestroyImmediate(m_monoer.gameObject);
            }
        }

        private void OnEnable()
        {
            m_attachProfilerUI = new UnityChainFrameDebuggerAttachProfilerUI();
            m_attachProfilerUI.OnEnable(this);
            
            m_hierarchy = new UnityChainFrameDebuggerHierarchy();
            m_hierarchy.OnEnable();
            
            m_inspector = new UnityChainFrameDebuggerInspector();
            m_inspector.OnEnable();
            
            m_profiler = new UnityChainFrameDebuggerProfiler();
            m_profiler.OnEnable();
        }

        private void OnDisable()
        {
            Clear();
        }

        private void OnUpdateCallback(float deltaTime)
        {
            m_profiler.Update();
            
            Repaint();
        }

        public static void Clear()
        {
            FrameDebuggerRuntimeDataCollector.Clear();
            FrameDebuggerTimelineDataCollector.Clear();
            UnityChainFrameDebuggerWindowGUIContext.Clear();
            UnityChainFrameDebuggerScene.Clear();
            
            UnityChainFrameDebuggerServerProxy.ProcessReceiveData(UnityChainFrameDebuggerProtocolID.C2S_Analyze, null);
        }

        private UnityChainFrameDebuggerAttachProfilerUI m_attachProfilerUI;
        private UnityChainFrameDebuggerHierarchy m_hierarchy;
        private UnityChainFrameDebuggerInspector m_inspector;
        private UnityChainFrameDebuggerProfiler m_profiler;
        private UnityChainFrameDebuggerMonoer m_monoer;
    }
}