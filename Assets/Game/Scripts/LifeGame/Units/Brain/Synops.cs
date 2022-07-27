using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.LifeGame.Units.Brain
{
    public class Synops
    {
        public byte sourceLayer, sourceNeuron, targetLayer, targetNeuron;
        public float value;

        /// <summary>
        /// Whether given synops connects same neurons
        /// </summary>
        /// <param name="synops"></param>
        /// <returns></returns>
        public bool IsSameConnection(Synops synops)
        {
            return sourceLayer == synops.sourceLayer &&
                sourceNeuron == synops.sourceNeuron &&
                targetLayer == synops.targetLayer &&
                targetNeuron == synops.targetNeuron;
        }
    }
}
