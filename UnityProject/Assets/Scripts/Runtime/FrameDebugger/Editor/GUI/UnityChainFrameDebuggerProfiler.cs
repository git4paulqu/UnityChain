using System.Collections.Generic;
using UnityEngine;

namespace UnityChain.FrameDebugger
{
    public class UnityChainFrameDebuggerProfiler
    {
        public void OnEnable()
        {
            
        }

        public void DoGUI(Rect rect)
        {
            GUI.Box(rect, string.Empty);

            Vector2 position = rect.position;
            position.x += 5;
            position.y += 5;

            Vector2 size = rect.size;
            size.x -= 10;
            size.y -= 10;
            
            float rectY = position.y;

            FrameDebuggerChainTimelineData chainTimelineData = UnityChainFrameDebuggerWindowGUIContext.GetChainTimelineData();
            if (null == chainTimelineData)
            {
                Reset();
                return;
            }

            {
                //play
                Rect playRect = new Rect(position.x, rectY, 100, 20);
                DrawOperation(playRect);
                rectY += 22;
            }

            {
                // frame slider
                Rect frameSliderRect = new Rect(position.x, rectY, size.x, 20);
                DrawFrameState(frameSliderRect, chainTimelineData);
                
                rectY += 22;
            }

            ulong timelineUID = chainTimelineData.GetUpdateUIDFromeFrame(UnityChainFrameDebuggerWindowGUIContext.frame);
            if (timelineUID <= 0)
            {
                return;
            }
            FrameDebuggerTimelineData timelineData = chainTimelineData.GetData(timelineUID);
            if (null == timelineData)
            {
                return;
            }
            
            {
                // Update
                Rect updateRect = new Rect(position.x, rectY, size.x, 50);
                DrawUpdate(updateRect, timelineData);

                rectY += 52;
            }

            {
                // LateUpdate
                Rect fixedUpdateRect = new Rect(position.x, rectY, size.x, 50);
                DrawAllFixedUpdate(fixedUpdateRect, chainTimelineData, timelineData);
            }

            ApplySnapshotData(timelineData.snapshotData);
        }

        public void Update()
        {
            if (m_playing)
            {
                m_focusFrame++;
                ClampFrame();
            }
        }

        private void DrawOperation(Rect rect)
        {
            var oldColor = GUI.color;

            if (GUI.Button(rect, "Pre Frame"))
            {
                m_focusFrame--;
            }

            rect.x += 110;
            
            string name = m_playing ? "playing" : "play";
            if (m_playing)
            {
                GUI.color = Color.green;    
            }

            if (GUI.Button(rect, name))
            {
                m_playing = !m_playing;
            }
            rect.x += 110;
            
            GUI.color = oldColor;
            
            if (GUI.Button(rect, "Next Frame"))
            {
                m_focusFrame++;
            }

            ClampFrame();
        }

        private void DrawFrameState(Rect rect, FrameDebuggerChainTimelineData chainTimelineData)
        {
            Rect slideRect = rect;
            slideRect.width -= 200;

            if (chainTimelineData.FrameCount > 0 && m_focusFrame < 0)
            {
                m_focusFrame = chainTimelineData.BeginFrame;
            }
            
            m_minFrame = chainTimelineData.BeginFrame;
            m_maxFrame = chainTimelineData.EndFrame;
            
            m_focusFrame = (int)GUI.HorizontalSlider(slideRect, m_focusFrame, m_minFrame, m_maxFrame);
            
            if (m_focusFrame != UnityChainFrameDebuggerWindowGUIContext.frame)
            {
                UnityChainFrameDebuggerWindowGUIContext.frame = m_focusFrame;
                ulong timelineUID = chainTimelineData.GetUpdateUIDFromeFrame(m_focusFrame);
                UnityChainFrameDebuggerWindowGUIContext.timelineUID = timelineUID;
            }
            
            Rect labelRect = rect;
            labelRect.width = 190;
            labelRect.x = slideRect.x + slideRect.width + 10;

            string label = string.Format("{0}/{1}  total:{2}", UnityChainFrameDebuggerWindowGUIContext.frame, m_maxFrame, chainTimelineData.FrameCount);
            GUI.Label(labelRect, label);
        }

        private void DrawUpdate(Rect rect, FrameDebuggerTimelineData timelineData)
        {
            DrawButton(rect, timelineData.id.ToString(), timelineData.uid, 1);
        }

        private void DrawAllFixedUpdate(Rect rect, FrameDebuggerChainTimelineData chainTimelineData, FrameDebuggerTimelineData rootData)
        {
            List<FrameDebuggerTimelineData> fixedUpdateDatas = chainTimelineData.GetChildrenData(rootData.uid);
            if (null == fixedUpdateDatas)
            {
                return;
            }
            
            int count = fixedUpdateDatas.Count;
            float itemWidth = rect.width / count;
            
            for (int i = 0; i < count; i++)
            {
                Rect itemRect = rect;
                itemRect.x = rect.x + itemWidth * i;
                itemRect.width = itemWidth;

                FrameDebuggerTimelineData fixedUpdateData = fixedUpdateDatas[i];
                
                DrawButton(itemRect, fixedUpdateData.id.ToString(), fixedUpdateData.uid, 2);

                itemRect.y += 52;
                DrawFixedUpdate(itemRect, i, chainTimelineData, fixedUpdateData);
            }
        }

        private void DrawFixedUpdate(Rect rect, int index, FrameDebuggerChainTimelineData chainTimelineData, FrameDebuggerTimelineData rootData)
        {
            List<FrameDebuggerTimelineData> children = chainTimelineData.GetChildrenData(rootData.uid);
            if (null == children)
            {
                return;
            }

            int count = children.Count;
            float itemWidth = rect.width / count;
            Rect itemRect = rect;
            itemRect.width = itemWidth;

            for (int i = 0; i < count; i++)
            {
                FrameDebuggerTimelineData data = children[i];

                DrawButton(itemRect, data.id.ToString(), data.uid, 3);
                itemRect.x += itemWidth;
            }
        }

        private void DrawButton(Rect rect, string name, ulong uid, int index)
        {
            string styleName = string.Format("flow node {0}", index);
            if (UnityChainFrameDebuggerWindowGUIContext.timelineUID == uid)
            {
                styleName += " on";
            }

            GUIStyle style = GUI.skin.FindStyle(styleName);
            if (GUI.Button(rect, name, style))
            {
                SetContextTimelineID(uid);
            }
        }

        private void Reset()
        {
            m_playing = false;
            m_focusFrame = -1;
        }

        private void SetContextTimelineID(ulong uid)
        {
            UnityChainFrameDebuggerWindowGUIContext.timelineUID = uid;
        }

        private void ApplySnapshotData(ChainSnapshotData snapshotData)
        {
            UnityChain.Chain chain = UnityChainFrameDebuggerWindowGUIContext.GetFocusChain();;
            if (null == chain)
            {
                return;
            }
            chain.ReadData(snapshotData);
        }

        private void ClampFrame()
        {
            m_focusFrame = Mathf.Clamp(m_focusFrame, m_minFrame, m_maxFrame);
        }

        private bool m_playing = false;
        private int m_focusFrame = -1;
        private int m_minFrame = int.MinValue;
        private int m_maxFrame = int.MaxValue;
    }
}