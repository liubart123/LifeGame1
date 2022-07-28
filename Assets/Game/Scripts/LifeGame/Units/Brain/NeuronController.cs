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
        float[][] neurons;
        public int[] neuronsNumberInLayers;
        NeuronActionController neuronActionController;
        MapController mapController;

        public void Initialize()
        {
            neuronActionController = NeuronActionController.Instance;
            mapController = MapController.Instance;

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
