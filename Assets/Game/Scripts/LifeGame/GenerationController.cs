using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.LifeGame.Units;
using Assets.Game.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame
{
    public class GenerationController : Singleton<GenerationController>
    {
        MapController mapController;
        public int[] neuronsNumberInLayers;
        public float synopsMagnitudeRandomAmplitude = 10f;
        public float synopsMagnitudeMutationRandomAmplitude = 0.1f;
        public float chanceOfMutation = 0.05f;
        public int countOfUnits = 1;
        public int countOfAncestors = 1;

        public void Initialize(int[] neuronsNumberInLayers, int countOfUnits = 10, int countOfAncestors = 1)
        {
            this.neuronsNumberInLayers = neuronsNumberInLayers;
            mapController = MapController.Instance;
            this.countOfUnits = countOfUnits;
            this.countOfAncestors = countOfAncestors;
        }

        public Unit[] CreateNewRandomPopulation()
        {
            Unit[] units = new Unit[countOfUnits];
            for(int i=0;i<countOfUnits; i++)
            {
                units[i] = new Unit();
                GenerateNewSynopsesForUnit(units[i]);
            }
            return units;
        }

        public Unit[] CreateNewPopulation(Unit[] baseUnits)
        {
            Unit[] result = CreateDescendants(baseUnits);
            MutateUnits(result);
            return result;
        }
        Unit[] CreateDescendants(Unit[] baseUnits)
        {

            SortedList<float, Unit> units = new SortedList<float, Unit>(Comparer<float>.Create(
                (x, y) => x >= y ? 1 : -1
            ));
            foreach(Unit unit in baseUnits)
            {
                units.Add(AssessSuccessfulnessOfUnit(unit), unit);
            }
            var ancestors = units.Values.TakeLast(countOfAncestors).ToList();

            Unit[] result = new Unit[countOfUnits];

            for (int i = 0; i < countOfUnits; i++)
            {
                result[i] = new Unit();
                CopySynopsesOfAncestorToDescendant(ancestors[i % countOfAncestors],result[i]);
            }
            return result;
        }
        float AssessSuccessfulnessOfUnit(Unit unit)
        {
            int sizeOfLiveZone = 10;
            if ((unit.position.x < sizeOfLiveZone &&
                unit.position.y < sizeOfLiveZone) ||
                (unit.position.x >= mapController.width - sizeOfLiveZone &&
                unit.position.y >= mapController.height - sizeOfLiveZone))
                return 1;
            //if (unit.position.x > mapController.width * 3 / 4 && unit.position.y < mapController.height / 4)
            //    return 1;
            //if (unit.position.x < mapController.width / 4 && unit.position.y > mapController.height * 3 / 4)
            //    return 1;
            return 0;
        }
        void MutateUnits(Unit[] baseUnits)
        {
            foreach (Unit unit in baseUnits)
            {
                MutateUnit(unit);
            }
        }
        void MutateUnit(Unit unit)
        {
            for (int layer = 0; layer < neuronsNumberInLayers.Length - 1; layer++)
            {
                for (int sourceNeuron = 0; sourceNeuron < neuronsNumberInLayers[layer]; sourceNeuron++)
                {
                    for (int targetNeuron = 0; targetNeuron < neuronsNumberInLayers[layer + 1]; targetNeuron++)
                    {
                        if (Random.Range(0,1f) < chanceOfMutation)
                        {
                            unit.synopses[layer][sourceNeuron][targetNeuron] +=
                                Random.Range(-synopsMagnitudeRandomAmplitude, synopsMagnitudeRandomAmplitude);
                        }
                    }
                }
            }
        }
        public void GenerateNewSynopsesForUnit(Unit unit)
        {
            unit.synopses = new float[neuronsNumberInLayers.Length-1][][];
            for(int layer =0;layer< neuronsNumberInLayers.Length-1; layer++)
            {
                unit.synopses[layer] = new float[neuronsNumberInLayers[layer]][];
                for(int sourceNeuron=0; sourceNeuron < neuronsNumberInLayers[layer]; sourceNeuron++)
                {
                    unit.synopses[layer][sourceNeuron] = new float[neuronsNumberInLayers[layer+1]];
                    for(int targetNeuron=0; targetNeuron < neuronsNumberInLayers[layer+1]; targetNeuron++)
                    {
                        unit.synopses[layer][sourceNeuron][targetNeuron] = 
                            Random.Range(-synopsMagnitudeRandomAmplitude, synopsMagnitudeRandomAmplitude);
                    }
                }
            }
        }
        public void CopySynopsesOfAncestorToDescendant(Unit ancestor, Unit descendant)
        {
            descendant.synopses = new float[neuronsNumberInLayers.Length - 1][][];
            for (int layer = 0; layer < neuronsNumberInLayers.Length - 1; layer++)
            {
                descendant.synopses[layer] = new float[neuronsNumberInLayers[layer]][];
                for (int sourceNeuron = 0; sourceNeuron < neuronsNumberInLayers[layer]; sourceNeuron++)
                {
                    descendant.synopses[layer][sourceNeuron] = new float[neuronsNumberInLayers[layer + 1]];
                    for (int targetNeuron = 0; targetNeuron < neuronsNumberInLayers[layer + 1]; targetNeuron++)
                    {
                        descendant.synopses[layer][sourceNeuron][targetNeuron] =
                            ancestor.synopses[layer][sourceNeuron][targetNeuron];
                    }
                }
            }
        }
    }
}
