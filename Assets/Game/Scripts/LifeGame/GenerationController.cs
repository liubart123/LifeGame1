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
        public float startSynopsValueRandomRange = 10f;
        /// <summary>
        /// % of synops' change in mutation
        /// </summary>
        public float synopsValueMutationRange = 0.1f;
        public float chanceOfUnitSoftMutation = 0.1f;
        public float chanceOfUnitHardMutation = 0.1f;
        public float chanceOfSynopsSoftMutation = 0.1f;
        public float chanceOfSynopsHardMutation;
        public int countOfUnits = 1;
        /// <summary>
        /// Maximum ratio of success of best and worst ancestor. Used to choose ancestors for next generation
        /// </summary>
        public float maxMinSuccessAncestorRatio = 1;    

        public void Initialize()
        {
            mapController = MapController.Instance;
            populationController = PopulationController.Instance;
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
                i <= baseUnits.Count()/10);

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
        }
        void MutateUnits(Unit[] baseUnits)
        {
            foreach (Unit unit in baseUnits)
            {
                if (Random.Range(0, 1f) < chanceOfUnitSoftMutation)
                    MutateUnit(unit, false);
                if (Random.Range(0, 1f) < chanceOfUnitHardMutation)
                    MutateUnit(unit, true);
            }
        }
        public void MutateUnit(Unit unit, bool isHardMutation = false)
        {
            for (int layer = 0; layer < neuronsNumberInLayers.Length - 1; layer++)
            {
                for (int sourceNeuron = 0; sourceNeuron < neuronsNumberInLayers[layer]; sourceNeuron++)
                {
                    for (int targetNeuron = 0; targetNeuron < neuronsNumberInLayers[layer + 1]; targetNeuron++)
                    {
                        if (isHardMutation == false && Random.Range(0, 1f) < chanceOfSynopsSoftMutation)
                        {
                            unit.synopses[layer][sourceNeuron][targetNeuron] *=
                                1 + Random.Range(-synopsValueMutationRange, synopsValueMutationRange);
                        }
                        if (isHardMutation == true && Random.Range(0, 1f) < chanceOfSynopsHardMutation)
                        {
                            unit.synopses[layer][sourceNeuron][targetNeuron] =
                                Random.Range(-startSynopsValueRandomRange, startSynopsValueRandomRange);
                        }
                    }
                }
            }
        }
        public void GenerateNewSynopsesForUnit(Unit unit, bool isHardMutation = false)
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
                            Random.Range(-startSynopsValueRandomRange, startSynopsValueRandomRange);
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
