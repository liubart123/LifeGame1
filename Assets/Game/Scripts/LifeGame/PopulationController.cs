using Assets.Game.Scripts.LifeGame.Environment;
using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.LifeGame.Units;
using Assets.Game.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame
{
    public class PopulationController : Singleton<PopulationController>
    {
        public Unit[] currentPopulation;

        public void SetUpRandomPopulation()
        {
            var units = GenerationController.Instance.CreateNewRandomPopulation();
            currentPopulation = units;
        }

        public void SetUpNewGenerationPopulation()
        {
            var units = GenerationController.Instance.CreateNewPopulation(
                currentPopulation);
            currentPopulation = units;
        }
    }
}
