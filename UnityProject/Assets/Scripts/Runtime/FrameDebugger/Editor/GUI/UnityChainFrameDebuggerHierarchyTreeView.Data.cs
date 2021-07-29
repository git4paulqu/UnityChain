using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public partial class UnityChainFrameDebuggerHierarchyTreeView
    {
        private class TreeViewItemData : TreeViewItem
        {
            public TreeViewItemData(int chainUID, int localID, GameObject gameObject)
            {
                ChainUID = chainUID;
                LocalID = localID;
                Root = gameObject;
            }

            public int ChainUID { get; private set; }
            public int LocalID { get; private set; }
            public GameObject Root { get; private set; }
        }
    }
}