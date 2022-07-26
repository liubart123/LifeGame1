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
    Color CalculateColorForUnit(Unit unit)
    {
        float sum1 = 0, sum2 = 0, sum3 = 0;
        int countOfSynopses = 0;
        float totalSumOfAverages = 0;
        float red=0, green=0, blue=0;

        for(int i = 0; i < unit.synopses.Length; i++)
        {
            for(int j = 0; j < unit.synopses[i].Length; j++)
            {
                for(int k = 0; k < unit.synopses[i][j].Length; k++)
                {
                    countOfSynopses++;
                    totalSumOfAverages += Mathf.Abs(unit.synopses[i][j][k]);

                    if (k % 3 == 0)
                        sum1 += unit.synopses[i][j][k];
                    else if (k % 3 == 1)
                        sum2 += unit.synopses[i][j][k];
                    else
                        sum3 += unit.synopses[i][j][k];
                }
            }
        }

        float absAverageValue = totalSumOfAverages / countOfSynopses;
        float maxColorImpactOfEachSynops = 1f / countOfSynopses * 3;

        int synopsCounter = 0;
        Color resultColor = Color.black;
        for (int i = 0; i < unit.synopses.Length; i++)
        {
            for (int j = 0; j < unit.synopses[i].Length; j++)
            {
                for (int k = 0; k < unit.synopses[i][j].Length; k++)
                {
                    var rnd = new System.Random(synopsCounter);
                    var tempColor = gradientForUnitBaseColor.Evaluate((float)rnd.NextDouble());
                    float impact = GetImpactGradeOnColorOfOneSynops(unit.synopses[i][j][k], absAverageValue) * 
                        maxColorImpactOfEachSynops;
                    resultColor += tempColor * impact;
                    //Mathf.Min(
                    //    Mathf.Abs(Mathf.Abs(unit.synopses[i][j][k]) - absAverageValue) / absAverageValue,
                    //    1) * maxColorImpactOfEachSynops;

                    //if (synopsCounter % 3 == 0)
                    //    red += increaseValue;
                    //else if (synopsCounter % 3 == 1)
                    //    green += increaseValue;
                    //else
                    //    blue += increaseValue;

                    synopsCounter++;
                }
            }
        }

        //var vectorColor = new Vector3(
        //    Mathf.Abs(sum1),
        //    Mathf.Abs(sum2),
        //    Mathf.Abs(sum3)
        //    ).normalized * 2;

        //var vectorColor = new Vector3(
        //    red,
        //    green,
        //    blue
        //    );


        //return new Color(vectorColor.x, vectorColor.y, vectorColor.z);
        float max = Mathf.Max(
            Mathf.Max(resultColor.r, resultColor.g),
            resultColor.b);
        resultColor /= max;
        return resultColor;
    }
    float GetImpactGradeOnColorOfOneSynops(float synops, float absAverageValue)
    {
        float[] boundaryValues = new float[]
        {
            -absAverageValue*2,
            -absAverageValue,
            -absAverageValue/2,
            -absAverageValue/4,
            absAverageValue/4,
            absAverageValue/2,
            absAverageValue,
            absAverageValue*2
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
