using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public partial class UnityChainFrameDebuggerHierarchyTreeView : TreeView
    {
        public UnityChainFrameDebuggerHierarchyTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
            Reload();
        }
		
        protected override TreeViewItem BuildRoot ()
        {
            // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
            // are created from data. Here we just create a fixed set of items, in a real world example
            // a data model should be passed into the TreeView and the items created from the model.

            // This section illustrates that IDs should be unique and that the root item is required to 
            // have a depth of -1 and the rest of the items increment from that.
            var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
            var allItems = ReloadData();
			
            // Utility method that initializes the TreeViewItem.children and -parent for all items.
            SetupParentsAndChildrenFromDepths (root, allItems);
			
            // Return root of the tree
            return root;
        }
        
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            int focusID = int.MinValue;
            
            if (selectedIds.Count > 1)
            {
                List<int> focus = new List<int>();
                if (selectedIds.Count > 0)
                {
                    focusID = selectedIds[0];
                    focus.Add(focusID);
                }
            
                SetSelection(focus);
            }
            else if (selectedIds.Count == 1)
            {
                focusID = selectedIds[0];
            }
            else
            {
                return;
            }

            TreeViewItemData data = GetDataFromID(focusID);
            int chainUID = data.ChainUID;
            int localID = data.LocalID;

            UnityChainFrameDebuggerWindowGUIContext.chainUID = chainUID;
            UnityChainFrameDebuggerWindowGUIContext.chainLocalID = localID;

            if (null != data.Root)
            {
                Selection.instanceIDs = new int[] { data.Root.GetInstanceID()};
            }
        }

        public void OnDataChanged()
        {
            Reload();
            SelectionChanged(new List<int>());
        }

        private List<TreeViewItem> ReloadData()
        {
            m_datas.Clear();
            
            UnityChainFrameDebuggerScene.Load();
            List<UnityChainFrameDebuggerChain> chains = UnityChainFrameDebuggerScene.Chains;

            if (null == chains)
            {
                return new List<TreeViewItem>();
            }
            
            foreach (var item in chains)
            {
                LoadChain(item);
            }
            
            List<TreeViewItem> allItems = new List<TreeViewItem>();

            foreach (var item in m_datas)
            {
                allItems.Add(item);
            }
            
            return allItems;
        }

        private void LoadChain(UnityChainFrameDebuggerChain chain)
        {
            if (null == chain || null == chain.gameObject)
            {
                return;
            }
            
            AddItemData(chain.gameObject, 0, chain.UID, -1);

            Transform root = chain.gameObject.transform;
            int count = root.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform child = root.GetChild(i);
                AddItemData(child.gameObject, 1, chain.UID, i);
            }
        }

        private void AddItemData(GameObject gameObject, int depth, int chainUID, int localID)
        {
            if (null == gameObject)
            {
                return;
            }
            
            TreeViewItemData data = new TreeViewItemData(chainUID, localID, gameObject);
            data.depth = depth;
            data.displayName = gameObject.name;
            data.id = m_datas.Count + 1;
            m_datas.Add(data);
        }

        private TreeViewItemData GetDataFromID(int id)
        {
            foreach (var item in m_datas)
            {
                if (item.id == id)
                {
                    return item;
                }
            }

            return null;
        }

        private List<UnityChainFrameDebuggerHierarchyTreeView.TreeViewItemData> m_datas = new List<UnityChainFrameDebuggerHierarchyTreeView.TreeViewItemData>();
    }
}