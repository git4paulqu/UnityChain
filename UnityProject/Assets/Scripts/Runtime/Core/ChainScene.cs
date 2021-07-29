using System.Collections.Generic;
using UnityEngine;

namespace UnityChain
{
    public class ChainScene
    {
        public static UnityChain.Chain[]GetChains()
        {
            UnityChain.Chain[] chains = GameObject.FindObjectsOfType<UnityChain.Chain>();
            
            return chains;
        }
    }
}