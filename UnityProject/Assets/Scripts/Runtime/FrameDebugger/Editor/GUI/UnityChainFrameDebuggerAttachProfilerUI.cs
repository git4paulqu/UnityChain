using UnityEditor;
using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class UnityChainFrameDebuggerAttachProfilerUI
    {
        public void OnEnable(UnityChainFrameDebuggerWindow owner)
        {
            m_owner = owner;
            
            m_attachProfilerUIType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AttachProfilerUI");
            m_attachProfilerUionGuiLayoutMethodInfo = m_attachProfilerUIType.GetMethod("OnGUILayout");
            m_attachProfilerUI = System.Activator.CreateInstance(m_attachProfilerUIType);

            if (m_server == null)
            {
                m_server = ScriptableObject.CreateInstance<UnityChainFrameDebuggerServer>();
                m_server.Initialize();
            }
        }
        
        public void OnDisable()
        {
            DisconnectAll();
        }

        public void DoGUI(Rect rect)
        {
            DrawMainToolBar();
        }

        private void DrawMainToolBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            m_attachProfilerUionGuiLayoutMethodInfo.Invoke(m_attachProfilerUI, new object[] { m_owner });
            
            GUILayout.Space(5);
            
            bool playerConnected = m_server.Conected;
            GUILayout.Label(playerConnected ? "Player Connected:" : "Player Disconnected");
            if (playerConnected)
            {
                var oldColor = GUI.color;
                GUI.color = Color.yellow;
                GUILayout.Label(m_server.PlayerDeviceModelString);
                GUI.color = oldColor;
                
                // disconnect all
                if (GUILayout.Button("Disconnect", EditorStyles.toolbarButton))
                {
                    DisconnectAll();
                }
                GUILayout.Space(5);
            }

            GUILayout.Space(5);
            
            // check response
            if (GUILayout.Button("Ping", EditorStyles.toolbarButton))
            {
                m_server.PingRemote();
            }
            GUILayout.Space(5);

            bool isSyncData = UnityChainFrameDebuggerServerProxy.IsSynchronousData;

            if (isSyncData)
            {
                Recording();
            }
            else
            {
                Record();
            }

            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                UnityChainFrameDebuggerWindow.Clear();
            }

            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
            {
            }
            
            if (GUILayout.Button("Load", EditorStyles.toolbarButton))
            {
                
            }

            GUILayout.EndHorizontal();
        }

        private void Record()
        {
            if (GUILayout.Button("Record", EditorStyles.toolbarButton))
            {
                m_server.Synchronous(true);
            }
        }

        private void Recording()
        {
            var oldColor = GUI.color;
            GUI.color = Color.green;

            if (GUILayout.Button("Recording", EditorStyles.toolbarButton))
            {
                m_server.Synchronous(false);
            }
            
            GUI.color = oldColor;
        }
        
        private void DisconnectAll()
        {
            m_server.Synchronous(false);
            m_server.DisconnectAll();
        }

        private System.Type m_attachProfilerUIType;
        private System.Reflection.MethodInfo m_attachProfilerUionGuiLayoutMethodInfo;
        private object m_attachProfilerUI;
        
        private UnityChainFrameDebuggerServer m_server;
        private UnityChainFrameDebuggerWindow m_owner;
    }
}