using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.LifeGame.Map.CellObjects;
using Assets.Game.Scripts.LifeGame.Units;
using Assets.Game.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame.Environment
{
    public class EnvironmentController : Singleton<EnvironmentController>
    {
        public const int NUMBER_OF_OBSTACLES = 0;

        MapController mapController;
        PopulationController populationController;
        public void Initialize()
        {
            mapController = MapController.Instance;
            populationController= PopulationController.Instance;
        }

        public void CreateAndPlaceObstaclesOnMap()
        {
            for (int i=0;i< NUMBER_OF_OBSTACLES; i++)
            {
                Obstacle obstacle = new Obstacle();
                var pos = GetFreeRadnomPosition();
                obstacle.position = pos;
                mapController.GetCell(pos).SetCellObject(obstacle);
            }
        }
        public void PlaceUnitsOnMap()
        {
            Unit[] units = populationController.currentPopulation;
            foreach (var unit in units)
            {
                var pos = GetFreeRadnomPosition();
                unit.position = pos;
                mapController.GetCell(pos).SetUnit(unit);
            }
        }
        Vector2Int GetFreeRadnomPosition()
        {
            Vector2Int pos;
            do
            {
                pos = GetRadnomPosition();
            } while (mapController.GetCell(pos)?.IsFree() == false);
            return pos;
        }
        Vector2Int GetRadnomPosition()
        {
            return new Vector2Int(
                UnityEngine.Random.Range(0, mapController.width),
                UnityEngine.Random.Range(0, mapController.height));
        }
        public void UpdateEnvironmentForTick()
        {
            //Debug.Log("EnvironmentController: UpdateEnvironmentForTick");
        }
        
    }
}
