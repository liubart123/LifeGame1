using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Game.Scripts.LifeGame.Units.Brain
{
    public class NeuronController : Singleton<NeuronController>
    {
        public float[][] neurons;
        public int[] neuronsNumberInLayers;
        NeuronActionController neuronActionController;
        MapController mapController;

        public void Initialize(int[] neuronsNumberInLayers)
        {
            neuronActionController = NeuronActionController.Instance;
            mapController = MapController.Instance;

            this.neuronsNumberInLayers = neuronsNumberInLayers;
            InitializeNeurons();
        }

        public void InitializeNeurons()
        {
            neurons = new float[neuronsNumberInLayers.Length][];
            for(int layer = 0; layer < neuronsNumberInLayers.Length; layer++)
            {
                neurons[layer] = new float[neuronsNumberInLayers[layer]];
            }
        }
        void ResetNeurons()
        {
            for (int layer = 0; layer < neuronsNumberInLayers.Length; layer++)
            {
                for(int i=0;i< neuronsNumberInLayers[layer]; i++)
                {
                    neurons[layer][i] = 0;
                }
            }
        }

        public void ChooseAndPerformUnitAction(Unit unit)
        {
            Profiler.BeginSample("ResetNeurons");
            ResetNeurons();
            Profiler.EndSample();

            Profiler.BeginSample("SetInputNeurons");
            SetInputNeurons(unit);
            Profiler.EndSample();

            Profiler.BeginSample("ChooseAndPerformUnitAction");
            for (int layer = 0; layer < neuronsNumberInLayers.Length-1; layer++)
            {
                if (layer != 0)
                {
                    for(int i = 0; i < neuronsNumberInLayers[layer]; i++)
                    {
                        neurons[layer][i] = Sigma(neurons[layer][i]);
                    }
                }

                foreach(var synops in unit.synopses[layer])
                {
                    neurons[synops.targetLayer][synops.targetNeuron] +=
                        neurons[layer][synops.sourceNeuron] * synops.value;
                }
            }
            Profiler.EndSample();

            Profiler.BeginSample("GetMaxOutputNeuronNumber");
            int choosedNeuron = GetMaxOutputNeuronNumber();
            Profiler.EndSample();

            Profiler.BeginSample("PerformActionByNeuronIndex");
            lock (this)
            {
                neuronActionController.PerformActionByNeuronIndex(choosedNeuron, unit);
            }
            Profiler.EndSample();
        }

        void SetInputNeurons(Unit unit)
        {
            int i = 0;
            SetInputNeuron(i++, unit.position.x / (float)mapController.width);
            SetInputNeuron(i++, (mapController.width - unit.position.x) / (float)mapController.width);
            SetInputNeuron(i++, unit.position.y / (float)mapController.height);
            SetInputNeuron(i++, (mapController.height - unit.position.y) / (float)mapController.height);
            SetInputNeuron(i++, 1);

            SetInputNeuron(i++,
                mapController.GetNeighbour(
                    unit.position, MapController.EDirection.up)
                ?.IsFree() == true ? 1 : 0);
            SetInputNeuron(i++,
                mapController.GetNeighbour(
                    unit.position, MapController.EDirection.right)
                ?.IsFree() == true ? 1 : 0);
            SetInputNeuron(i++,
                mapController.GetNeighbour(
                    unit.position, MapController.EDirection.down)
                ?.IsFree() == true ? 1 : 0);
            SetInputNeuron(i++,
                mapController.GetNeighbour(
                    unit.position, MapController.EDirection.left)
                ?.IsFree() == true ? 1 : 0);

            SetInputNeuron(i++, mapController.IsEndOfMap(unit.position)?1:0);
        }
        void SetInputNeuron(int neuron, float value)
        {
            if (neuron >= neuronsNumberInLayers[0])
                return;
            neurons[0][neuron] = value;
        }

        int GetMaxOutputNeuronNumber()
        {
            int maxNeuronIndex=0;
            float maxValue = float.MinValue;
            for(int i=0;i< neuronsNumberInLayers[^1]; i++)
            {
                if (neurons[^1][i] > maxValue)
                {
                    maxValue = neurons[^1][i];
                    maxNeuronIndex = i;
                }
            }
            return maxNeuronIndex;
        }

        float Sigma(float x)
        {
            return 1 / (1 + Mathf.Exp(-x));
        }
    }
}
