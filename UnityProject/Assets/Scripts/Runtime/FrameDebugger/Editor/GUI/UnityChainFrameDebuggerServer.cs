using System;
using System.IO;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace UnityChain.FrameDebugger
{
    public partial class UnityChainFrameDebuggerServer: ScriptableObject
    {
        public void Initialize()
        {
            UnityChainLogger.Log( "[UnityChainFrameDebuggerServer] Initialize");
            
            EditorConnection.instance.Initialize();
            
            EditorConnection.instance.Register(UnityChainFrameDebuggerProtocolID.C2S_ClientConected, HandleClientConected);
            EditorConnection.instance.Register(UnityChainFrameDebuggerProtocolID.C2S_Snapshot, HandleSnapshot);
            EditorConnection.instance.Register(UnityChainFrameDebuggerProtocolID.C2S_Synchronous, HandleSynchronous);
            EditorConnection.instance.Register(UnityChainFrameDebuggerProtocolID.C2S_Analyze, HandleAnalyze);
        }
        
        public void DisconnectAll()
        {
            UnityChainLogger.Log( "[UnityChainFrameDebuggerServer] DisconnectAll");
            ContentPlayer = null;
        }
        
        public void PingRemote()
        {
            string message = string.Format("ping form editor, {0}", System.DateTime.Now);
            byte[] bytes = UnityChainFrameDebuggerSerializer.String2Bytes(message);
            SendMessage(UnityChainFrameDebuggerProtocolID.S2C_Ping, bytes);
        }

        public void Synchronous(bool enbale)
        {
            if (enbale)
            {
                UnityChainFrameDebuggerScene.Clear();
                UnityChainFrameDebuggerServerProxy.PreSynchronousData();    
            }

            if (!Conected)
            {
                UnityChainFrameDebuggerClientProxy.HandleREC(enbale);
                return;
            }
            
            byte[] message = BitConverter.GetBytes(enbale);
            SendMessage(UnityChainFrameDebuggerProtocolID.S2C_EnableREC, message);
        }

        private void HandleClientConected(MessageEventArgs args)
        {
            UnityChainLogger.Log( "[UnityChainFrameDebuggerServer] HandleClientConected");
            
            ContentPlayer = new Player();

            using (MemoryStream ms = new MemoryStream(args.data))
            {
                BinaryReader reader = new BinaryReader(ms);
                ContentPlayer.platform = (RuntimePlatform)reader.ReadInt32();
                ContentPlayer.deviceModel = reader.ReadString();
                reader.Close();
            }
        }

        private void HandleSnapshot(MessageEventArgs args)
        {
            UnityChainFrameDebuggerServerProxy.ProcessReceiveData(UnityChainFrameDebuggerProtocolID.C2S_Snapshot, args.data);
        }

        private void HandleSynchronous(MessageEventArgs args)
        {
            UnityChainFrameDebuggerServerProxy.ProcessReceiveData(UnityChainFrameDebuggerProtocolID.C2S_Synchronous, args.data);
        }

        private void HandleAnalyze(MessageEventArgs args)
        {
            UnityChainFrameDebuggerServerProxy.ProcessReceiveData(UnityChainFrameDebuggerProtocolID.C2S_Analyze, null);
        }

        private void SendMessage(Guid protocolID, byte[] message)
        {
            UnityChainLogger.Log( "[UnityChainFrameDebuggerServer] SendMessage:{0}", protocolID);
            EditorConnection.instance.Send(protocolID, message);
        }
        
        private void OnS2C_Ping(MessageEventArgs args)
        {
            string message = UnityChainFrameDebuggerSerializer.Bytes2String(args.data);
            UnityChainLogger.Log("[UnityChainFrameDebuggerClient] ping:{0}", message);
        }

        public bool Conected
        {
            get { return null != ContentPlayer; }
        }

        public string PlayerDeviceModelString
        {
            get
            {
                if (Conected)
                {
                    return ContentPlayer.deviceModel;
                }

                return "Unknwon";
            }
        }

        public UnityChainFrameDebuggerServer.Player ContentPlayer { get; private set; }
    }
}