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

        public Synops(byte sourceLayer, byte sourceNeuron, byte targetLayer, byte targetNeuron)
        {
            this.sourceLayer = sourceLayer;
            this.sourceNeuron = sourceNeuron;
            this.targetLayer = targetLayer;
            this.targetNeuron = targetNeuron;
        }

        public Synops Copy()
        {
            Synops result = new Synops(this.sourceLayer, this.sourceNeuron, this.targetLayer, this.targetNeuron);
            result.value = value;
            return result;
        }

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

        public override string ToString()
        {
            return $"[{sourceLayer}][{sourceNeuron}]->[{targetLayer}][{targetNeuron}]:{value}";
        }
    }
}
