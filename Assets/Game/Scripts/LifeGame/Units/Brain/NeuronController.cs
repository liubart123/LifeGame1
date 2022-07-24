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

        public void ChooseAndPerformUnitAction(Unit unit)
        {
            Profiler.BeginSample("SetInputNeurons");
            SetInputNeurons(unit);
            Profiler.EndSample();

            Profiler.BeginSample("ChooseAndPerformUnitAction");
            for (int layer = 1; layer < neuronsNumberInLayers.Length; layer++)
            {
                for (int neuron = 0; neuron < neuronsNumberInLayers[layer]; neuron++)
                {
                    float newNeuronValue = 0;

                    for (int prevLayerNeuron = 0; prevLayerNeuron < neuronsNumberInLayers[layer - 1]; prevLayerNeuron++)
                    {
                        newNeuronValue += 
                            neurons[layer - 1][prevLayerNeuron] * 
                            unit.synopses[layer - 1][prevLayerNeuron][neuron];

                    }

                    neurons[layer][neuron] = Sigma(newNeuronValue);
                }
            }
            Profiler.EndSample();

            Profiler.BeginSample("GetMaxOutputNeuronNumber");
            int choosedNeuron = GetMaxOutputNeuronNumber();
            Profiler.EndSample();

            Profiler.BeginSample("PerformActionByNeuronIndex");
            neuronActionController.PerformActionByNeuronIndex(choosedNeuron, unit);
            Profiler.EndSample();
        }

        void SetInputNeurons(Unit unit)
        {
            SetInputNeuron(0, unit.position.x / (float)mapController.width);
            SetInputNeuron(1, (mapController.width - unit.position.x) / (float)mapController.width);
            SetInputNeuron(2, unit.position.y / (float)mapController.height);
            SetInputNeuron(3, (mapController.height - unit.position.y) / (float)mapController.height);
            SetInputNeuron(4, 1);
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
