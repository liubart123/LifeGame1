using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.LifeGame.Map.CellObjects;
using Assets.Game.Scripts.LifeGame.Units;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    public ObjectPool poolOfCircleObjects;
    public Tilemap tileMap;
    public Tile mapCellTile, obstacleTile;
    public float gridScale;
    List<GameObject> usedCirclesInRender = new List<GameObject>();
    public Gradient gradientForUnitBaseColor;


    void Start()
    {
    }

    public void CreateMap(MapController mapController)
    {
        tileMap.ClearAllTiles();
        for (int x = 0; x < mapController.width; x++)
        {
            for (int y = 0; y < mapController.height; y++)
            {
                var tileMapPosition = new Vector3Int(
                        (-mapController.width / 2 + x),
                        (-mapController.height / 2 + y),
                        0
                );
                var mapPosition = new Vector2Int(x,y);

                if (mapController.GetCell(mapPosition).cellObject is Obstacle)
                    tileMap.SetTile((Vector3Int)tileMapPosition, obstacleTile);
                else
                    tileMap.SetTile((Vector3Int)tileMapPosition, mapCellTile);

            }
        }
    }


    public void RenderMap(MapController mapController)
    {
        int numberOfUsedCircles = 0;
        for (int x = 0; x < mapController.width; x++)
        {
            for (int y = 0; y < mapController.height; y++)
            {
                var cell = mapController.GetCell(x, y);
                var position = new Vector3Int(
                        (-mapController.width / 2 + x),
                        (-mapController.height / 2 + y),
                        0
                );
                //tileMap.SetTile(position, mapCellTile);

                if (cell.unit != null)
                {
                    GameObject circle;
                    if (usedCirclesInRender.Count <= numberOfUsedCircles)
                    {
                        circle = poolOfCircleObjects.GetObjectFromPool();
                        usedCirclesInRender.Add(circle);
                    }
                    else
                    {
                        circle = usedCirclesInRender[numberOfUsedCircles];
                    }
                    numberOfUsedCircles++;

                    circle.GetComponent<SpriteRenderer>().color = CalculateColorForUnit(cell.unit);
                    circle.transform.position = (Vector3)position * gridScale + new Vector3(gridScale/2, gridScale/2,0);
                    circle.SetActive(true);
                }
            }
        }
        for(int i = numberOfUsedCircles; i < usedCirclesInRender.Count; i++)
        {
            usedCirclesInRender[i].SetActive(false);
        }
    }
    public int maxRndValue;
    Color CalculateColorForUnit(Unit unit)
    {
        int countOfSynopses = 0;
        float resultedGradientX = 0;


        var rnd = new System.Random(0);
        for (int i = 0; i < unit.synopses.Length; i++)
        {
            for(int j = 0; j < unit.synopses[i].Length; j++)
            {
                for(int k = 0; k < unit.synopses[i][j].Length; k++)
                {
                    countOfSynopses++;
                    resultedGradientX += unit.synopses[i][j][k] * rnd.Next(0, maxRndValue);
                }
            }
        }
        resultedGradientX = resultedGradientX % 100;
        resultedGradientX += 100;
        resultedGradientX /= 200f;


        return gradientForUnitBaseColor.Evaluate(resultedGradientX);
    }
    float GetImpactGradeOnColorOfOneSynops(float synops, float absAverageValue)
    {
        float[] boundaryValues = new float[]
        {
            -absAverageValue,
            -absAverageValue/4,
            absAverageValue/4,
            absAverageValue,
        };
        int grade = int.MinValue;
        for(int i = 0; i < boundaryValues.Length; i++)
        {
            if (synops <= boundaryValues[i])
            {
                grade = i;
                break;
            }
        }
        if (grade == int.MinValue)
            grade = boundaryValues.Length;

        return (float)grade / boundaryValues.Length;
    }
}
