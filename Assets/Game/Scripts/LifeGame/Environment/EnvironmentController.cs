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
        PointOfEnergy[] pointsOfEnergy;
        public float PoiEnergyCapacity = 100;

        MapController mapController;
        PopulationController populationController;
        public void Initialize()
        {
            mapController = MapController.Instance;
            populationController= PopulationController.Instance;
        }
        public void Reset()
        {
            pointsOfEnergy = new PointOfEnergy[] {
                    //new PointOfEnergy(Vector2Int.zero,PoiEnergyCapacity),
                    new PointOfEnergy(new Vector2Int(mapController.width - 1, mapController.height - 1),PoiEnergyCapacity)
                     };
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
            foreach(var unit in populationController.currentPopulation)
            {
                int sizeOfZoneWithEnergy = 20;
                foreach(var point in pointsOfEnergy)
                {
                    float energy = Mathf.Max(0, sizeOfZoneWithEnergy - GetDistancebetweenPoints(unit.position, point.position));
                    
                    energy = Mathf.Min(point.currentEnergy, energy);
                    point.currentEnergy -= energy;

                    unit.energy += energy;
                }
            }
        }
        float GetDistancebetweenPoints(Vector2Int pos1, Vector2Int pos2)
        {
            return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
        }
        
    }

}
