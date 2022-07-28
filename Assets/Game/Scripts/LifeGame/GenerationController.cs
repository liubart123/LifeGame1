using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.LifeGame.Units;
using Assets.Game.Scripts.LifeGame.Units.Brain;
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
        public float chanceOfRemovingSynops = 0.1f;
        public int countOfUnits = 1;
        public float maxMinSuccessAncestorRatio = 1;

        public int startNumberOfNeuorns;
        public int numberOfNeuronsForMutation;


        public void Initialize(
            int[] neuronsNumberInLayers, 
            int countOfUnits = 10, 
            float maxMinSuccessAncestorRatio = 10,
            float chanceOfMutation = 0.1f,
            float chanceOfMutationForUnit = 0.1f,
            float chanceOfRemovingSynops = 0.1f,
            int startNumberOfNeuorns = 10,
            int numberOfNeuronsForMutation = 5)
        {
            mapController = MapController.Instance;
            populationController = PopulationController.Instance;
            this.neuronsNumberInLayers = neuronsNumberInLayers;
            this.countOfUnits = countOfUnits;
            this.maxMinSuccessAncestorRatio = maxMinSuccessAncestorRatio;
            this.chanceOfMutationForUnit = chanceOfMutationForUnit;
            this.chanceOfMutation = chanceOfMutation;
            this.chanceOfRemovingSynops = chanceOfRemovingSynops;
            this.startNumberOfNeuorns = startNumberOfNeuorns;
            this.numberOfNeuronsForMutation = numberOfNeuronsForMutation;
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
            if (Random.Range(0, 1f) < chanceOfMutationForUnit)
            {
                int localNumberOfNeuronsForMutation = 1 + (int)((numberOfNeuronsForMutation-1) * Random.Range(0, 1f));
                for (int i = 0; i < localNumberOfNeuronsForMutation; i++)
                {
                    MutateSynops(unit);
                }
            }
        }
        public void GenerateNewSynopsesForUnit(Unit unit)
        {
            unit.synopses = new List<Synops>[neuronsNumberInLayers.Length - 1];
            for(int layer =0;layer< neuronsNumberInLayers.Length-1; layer++)
            {
                unit.synopses[layer] = new List<Synops>();
            }
            for (int i = 0; i < startNumberOfNeuorns; i++)
            {
                MutateSynops(unit);
            }
        }
        public void MutateSynops(Unit unit)
        {
            if (chanceOfRemovingSynops > Random.Range(0, 1f))
            {
                int removeLayer = Random.Range(0, neuronsNumberInLayers.Length - 1);
                int removeIndex = Random.Range(0, unit.synopses[removeLayer].Count);
                if (removeIndex >= unit.synopses[removeLayer].Count)
                    return;
                unit.synopses[removeLayer].RemoveAt(removeIndex);
                return;
            }


            int sourceLayer = Random.Range(0, neuronsNumberInLayers.Length - 1);
            int sourceIndex = Random.Range(0, neuronsNumberInLayers[sourceLayer]);

            int targetLayer = Random.Range(sourceLayer + 1, neuronsNumberInLayers.Length);
            int targetIndex = Random.Range(0, neuronsNumberInLayers[targetLayer]);

            var newSynops = new Synops(
                (byte)sourceLayer,
                (byte)sourceIndex,
                (byte)targetLayer,
                (byte)targetIndex);

            newSynops.value = FullyMutateSynopsValue();

            var existingSynops = unit.synopses[sourceLayer].Find(s => s.IsSameConnection(newSynops));
            if (existingSynops != null)
            {
                existingSynops.value = newSynops.value;
            } else
            {
                unit.synopses[sourceLayer].Add(newSynops);
            }
        }

        float FullyMutateSynopsValue()
        {
            return Random.Range(-synopsMagnitudeRandomAmplitude, synopsMagnitudeRandomAmplitude);
        }

        public void CopySynopsesOfAncestorToDescendant(Unit ancestor, Unit descendant)
        {
            descendant.synopses = new List<Synops>[neuronsNumberInLayers.Length - 1];
            for (int layer = 0; layer < neuronsNumberInLayers.Length - 1; layer++)
            {
                descendant.synopses[layer] = new List<Synops>();
                foreach(var synops in ancestor.synopses[layer])
                {
                    descendant.synopses[layer].Add(synops.Copy());
                }
            }
        }
    }
}
