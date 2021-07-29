using System;

namespace UnityChain.FrameDebugger
{
    public static class UnityChainFrameDebuggerProtocolID
    {
        public static readonly Guid S2C_Ping = new Guid("3ea42a254ef63f15905a49a4a5cda3d7");
        public static readonly Guid S2C_EnableREC = new Guid("8f99e3b88b45a037d13d958397824d61");
        
        public static readonly Guid C2S_ClientConected = new Guid("02bf40d8eaca7408ba28afe746ed9a4e");
        public static readonly Guid C2S_Snapshot = new Guid("103ae560d30b6ec4c7f9b0f46c659284");
        public static readonly Guid C2S_Synchronous = new Guid("6a3c4dea35fb829fe31d807502ff23d3");
        public static readonly Guid C2S_Analyze = new Guid("3df7f3323f4c310ac28fa5df4bb221ce");
    }
}