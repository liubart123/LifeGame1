using Assets.Game.Scripts.LifeGame.Environment;
using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.LifeGame.Units;
using Assets.Game.Scripts.LifeGame.Units.Brain;
using Assets.Game.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Game.Scripts.LifeGame
{
    public class LifeTickController : Singleton<LifeTickController>
    {
        PopulationController lifeIterationController;
        NeuronController neuronController;
        EnvironmentController environmentController;
        public int currentTickCount=0;

        public void Initialize()
        {
            lifeIterationController = PopulationController.Instance;
            neuronController = NeuronController.Instance;
            environmentController = EnvironmentController.Instance;
        }
        public void Reset()
        {
            currentTickCount = 0;
        }
        public void RunTick()
        {
            PerformUnitsAction();

            Profiler.BeginSample("UpdateEnvironmentForTick");
            environmentController.UpdateEnvironmentForTick();
            Profiler.EndSample();

            currentTickCount++;
        }

        void PerformUnitsAction()
        {

            Parallel.ForEach(
                SplitUnitsPopulationInChunks(
                    lifeIterationController.currentPopulation
                        .Where(u => u.isLive), 
                    16), 
                units =>
                {
                    foreach(var unit in units)
                    {
                        neuronController.ChooseAndPerformUnitAction(unit);
                    }
                });
            //foreach (var unit in lifeIterationController.currentPopulation)
            //{
            //    if (unit.isLive != true)
            //        continue;
            //    neuronController.ChooseAndPerformUnitAction(unit);
            //}
        }

        List<List<Unit>> SplitUnitsPopulationInChunks(IEnumerable<Unit> units, int countOfChunks)
        {
            List<List<Unit>> result = new List<List<Unit>>();
            for(int x = 0; x < countOfChunks; x++)
            {
                result.Add(new List<Unit>());
            }

            int i = 0;
            foreach(var unit in units)
            {
                result[i % countOfChunks].Add(unit);
                i++;
            }
            return result;
        }
    }
}
