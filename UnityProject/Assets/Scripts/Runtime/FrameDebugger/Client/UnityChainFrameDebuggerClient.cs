using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace UnityChain.FrameDebugger
{
    public partial class UnityChainFrameDebuggerClient : ScriptableObject
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoadEntryPoint()
        {
            UnityChain.UnityChainLogger.Log("[UnityChainFrameDebuggerClient], InitializeOnLoadEntryPoint");
            
#if !UNITY_EDITOR
            InitializeOnLoad();
#endif
        }
        
        private static void InitializeOnLoad()
        {
            UnityChain.UnityChainLogger.Log("[UnityChainFrameDebuggerClient], InitializeOnLoad");
            instance = ScriptableObject.CreateInstance<UnityChainFrameDebuggerClient>();
        }
        
        private void OnEnable()
        {
            UnityChain.UnityChainLogger.Log("[UnityChainFrameDebuggerClient], OnEnable");
            Initialize();
        }

        void Initialize()
        {
            UnityChain.UnityChainLogger.Log("[UnityChainFrameDebuggerClient], Initialize");
            
            PlayerConnection.instance.RegisterConnection(OnConnected);
            PlayerConnection.instance.RegisterDisconnection(OnDisconnected);
            
            PlayerConnection.instance.Register(UnityChainFrameDebuggerProtocolID.S2C_Ping, HandlePing);
            PlayerConnection.instance.Register(UnityChainFrameDebuggerProtocolID.S2C_EnableREC, HandleEnableREC);
        }

        private void OnConnected(int playerID)
        {
            UnityChainLogger.Log("[UnityChainFrameDebuggerClient] OnConnected:{0}", playerID);

            HandleConected();
        }

        private void OnDisconnected(int playerID)
        {
            UnityChainLogger.Log("[UnityChainFrameDebuggerClient] OnDisconnected:{0}", playerID);

            // destroy the previous instance
            var field = typeof(PlayerConnection).GetField("s_Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            field.SetValue(null, null);

            var go = new GameObject("UnityChainFrameDebuggerClient", new System.Type[] { typeof(UnityChainFrameDebuggerClient.DelayedInitialize) });
            DontDestroyOnLoad(go);
        }

        private void HandlePing(MessageEventArgs args)
        {
            string message = UnityChainFrameDebuggerSerializer.Bytes2String(args.data);
            UnityChainLogger.Log("[UnityChainFrameDebuggerClient] ping:{0}", message);

            HandleConected();
        }
        
        private void HandleEnableREC(MessageEventArgs args)
        {
            bool enable = BitConverter.ToBoolean(args.data, 0);
            UnityChainFrameDebuggerClientProxy.HandleREC(enable);
        }

        private void HandleConected()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8);
                writer.Write((int)Application.platform);
                writer.Write(SystemInfo.deviceModel);
                UnityChainFrameDebuggerClientProxy.SendMessage(UnityChainFrameDebuggerProtocolID.C2S_ClientConected, ms.ToArray());
                writer.Close();
            }
        }

        private static UnityChainFrameDebuggerClient instance;
        
        public class DelayedInitialize : MonoBehaviour
        {
            private void Start()
            {
                Invoke("ExecuteCallback", 0.5f);
            }

            void ExecuteCallback()
            {
                UnityChainFrameDebuggerClient.instance.Initialize();
                Destroy(gameObject);
            }
        }
    }
}