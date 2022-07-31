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
        public PointOfEnergy[] pointsOfEnergy;
        public int unitPlacementOffset = 20;
        public int sizeOfZoneWithEnergy = 20;
        public Vector2Int[] pointsOfObstaclesLine;

        MapController mapController;
        PopulationController populationController;
        public void Initialize()
        {
            mapController = MapController.Instance;
            populationController= PopulationController.Instance;
        }
        public void Reset()
        {
            foreach (var poi in pointsOfEnergy){
                poi.Reset();
            }

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


            List<Vector2Int> positionsOfLine = new List<Vector2Int>();
            for (int i = 0; i < pointsOfObstaclesLine.Length - 1; i+=2)
            {
                Vector2Int startPos = pointsOfObstaclesLine[i];
                Vector2Int endPos = pointsOfObstaclesLine[i+1];

                float a = Mathf.Infinity;
                if ((endPos.x - startPos.x) != 0)
                {
                    a = (endPos.y - startPos.y) / (float)(endPos.x - startPos.x);
                }
                float b = startPos.y - startPos.x * a;

                int xDirection = 1;
                if (startPos.x > endPos.x)
                    xDirection = -1;

                int yDirection = 1;
                if (startPos.y > endPos.y)
                    yDirection = -1;

                Vector2Int currentPos = startPos;
                positionsOfLine.Add(currentPos);
                while (currentPos != endPos)
                {
                    float yFromX = a * currentPos.x + b;

                    if (Mathf.Abs(currentPos.y - yFromX) <= 0.5f)
                    {
                        currentPos.x += xDirection;
                    }
                    else
                    {
                        currentPos.y += yDirection;
                    }
                    positionsOfLine.Add(currentPos);
                }
            }

            foreach (var obstacle in positionsOfLine)
            {
                mapController.GetCell(obstacle)?.SetCellObject(new Obstacle(obstacle));
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
                UnityEngine.Random.Range(unitPlacementOffset, mapController.width - unitPlacementOffset),
                UnityEngine.Random.Range(unitPlacementOffset, mapController.height - unitPlacementOffset));
        }
        public void UpdateEnvironmentForTick()
        {
            foreach(var unit in populationController.currentPopulation)
            {
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
