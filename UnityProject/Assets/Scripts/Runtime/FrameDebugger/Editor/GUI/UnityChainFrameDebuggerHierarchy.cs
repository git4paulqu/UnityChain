using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class UnityChainFrameDebuggerHierarchy
    {
        public void OnEnable ()
        {
            // Check if we already had a serialized view state (state 
            // that survived assembly reloading)
            if (m_treeViewState == null)
                m_treeViewState = new TreeViewState ();

            m_treeView = new UnityChainFrameDebuggerHierarchyTreeView(m_treeViewState);
            m_searchField = new SearchField ();
            m_searchField.downOrUpArrowKeyPressed += m_treeView.SetFocusAndEnsureSelectedItem;
        }
        
        public void DoGUI(Rect rect)
        {
            GUI.Box(rect, string.Empty);
            
            Rect contentRect = new Rect(rect.x, rect.y, rect.width, rect.height - 4);

            Prepare();
            
            DoToolbar(rect);
            DoTreeView(rect);
        }

        public void Refresh()
        {
            m_treeView.Reload();
            m_treeView.SetFocusAndEnsureSelectedItem();
            FrameDebuggerTimelineSimulater.Simulate();
        }

        private void Prepare()
        {
            if (!UnityChainFrameDebuggerServerProxy.IsWaitAnalyze)
            {
                return;
            }
            
            UnityChainFrameDebuggerServerProxy.ResetFlag();

            m_treeView.OnDataChanged();
            FrameDebuggerTimelineSimulater.Simulate();
        }

        void DoToolbar(Rect rect)
        {
            Vector2 size = rect.size;
            size.y = 20;
            rect.size = size;
            
            Vector2 position = rect.position;
            position.y += 2;
            rect.position = position;
            
            m_treeView.searchString = m_searchField.OnToolbarGUI (rect, m_treeView.searchString);
        }

        void DoTreeView(Rect rect)
        {
            Vector2 position = rect.position;
            position.y += 22;
            rect.position = position;
            
            m_treeView.OnGUI(rect);
        }
        
        private TreeViewState m_treeViewState;
        private UnityChainFrameDebuggerHierarchyTreeView m_treeView;
        private SearchField m_searchField;
    }
}