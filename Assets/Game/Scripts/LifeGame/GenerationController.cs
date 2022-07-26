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
        PopulationController populationController;
        public int[] neuronsNumberInLayers;
        public float synopsMagnitudeRandomAmplitude = 10f;
        public float synopsMagnitudeMutationRandomAmplitude = 0.1f;
        public float chanceOfMutationForUnit = 0.1f;
        public float chanceOfMutation = 0.1f;
        public int countOfUnits = 1;
        public float maxMinSuccessAncestorRatio = 1;

        public void Initialize(
            int[] neuronsNumberInLayers, 
            int countOfUnits = 10, 
            float maxMinSuccessAncestorRatio = 10,
            float chanceOfMutation = 0.1f,
            float chanceOfMutationForUnit = 0.1f)
        {
            mapController = MapController.Instance;
            populationController = PopulationController.Instance;
            this.neuronsNumberInLayers = neuronsNumberInLayers;
            this.countOfUnits = countOfUnits;
            this.maxMinSuccessAncestorRatio = maxMinSuccessAncestorRatio;
            this.chanceOfMutationForUnit = chanceOfMutationForUnit;
            this.chanceOfMutation = chanceOfMutation;
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
                (x, y) => x >= y ? -1 : 1
            ));
            foreach(Unit unit in baseUnits)
            {
                units.Add(AssessSuccessfulnessOfUnit(unit), unit);
            }

            float maxSuccess = units.First().Key;
            var ancestors = units.TakeWhile(
                (u,i) => 
                maxSuccess / u.Key < maxMinSuccessAncestorRatio && 
                i < baseUnits.Count()/10);

            float minUsefullnessOfAncestor = ancestors.Min(u=>u.Key);
            List<Unit> ancestorsListWithProportions = new List<Unit>();
            foreach (var ancestor in ancestors)
            {
                int proportionOfAcnestor = (int)(ancestor.Key / minUsefullnessOfAncestor);
                for (int i=0;i< proportionOfAcnestor; i++)
                {
                    ancestorsListWithProportions.Add(ancestor.Value);
                }
            }

            Unit[] result = new Unit[countOfUnits];
            for (int i = 0; i < countOfUnits; i++)
            {
                result[i] = new Unit();
                CopySynopsesOfAncestorToDescendant(ancestorsListWithProportions[i % ancestorsListWithProportions.Count],result[i]);
            }
            return result;
        }
        float AssessSuccessfulnessOfUnit(Unit unit)
        {
            return unit.energy;
            //int sizeOfLiveZone = 2;
            //if ((unit.position.x < sizeOfLiveZone &&
            //    unit.position.y < sizeOfLiveZone) ||
            //    (unit.position.x >= mapController.width - sizeOfLiveZone &&
            //    unit.position.y >= mapController.height - sizeOfLiveZone))
            //    return 1;
            //if (unit.position.x > mapController.width * 3 / 4 && unit.position.y < mapController.height / 4)
            //    return 1;
            //if (unit.position.x < mapController.width / 4 && unit.position.y > mapController.height * 3 / 4)
            //    return 1;
            //return 0;
        }
        void MutateUnits(Unit[] baseUnits)
        {
            foreach (Unit unit in baseUnits)
            {
                MutateUnit(unit);
            }
        }
        public void MutateUnit(Unit unit)
        {
            for (int layer = 0; layer < neuronsNumberInLayers.Length - 1; layer++)
            {
                for (int sourceNeuron = 0; sourceNeuron < neuronsNumberInLayers[layer]; sourceNeuron++)
                {
                    if (Random.Range(0,1f) < chanceOfMutationForUnit)
                    {
                        for (int targetNeuron = 0; targetNeuron < neuronsNumberInLayers[layer + 1]; targetNeuron++)
                        {
                            if (Random.Range(0, 1f) < chanceOfMutation)
                            {
                                unit.synopses[layer][sourceNeuron][targetNeuron] =
                                    Random.Range(-synopsMagnitudeRandomAmplitude, synopsMagnitudeRandomAmplitude);
                            }
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
