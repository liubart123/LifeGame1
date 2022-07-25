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

        for(int i = 0; i < unit.synopses.Length; i++)
        {
            for(int j = 0; j < unit.synopses[i].Length; j++)
            {
                for(int k = 0; k < unit.synopses[i][j].Length; k++)
                {
                    if (k % 3 == 0)
                        sum1 += unit.synopses[i][j][k];
                    else if (k % 3 == 1)
                        sum2 += unit.synopses[i][j][k];
                    else
                        sum3 += unit.synopses[i][j][k];
                }
            }
        }

        var vectorColor = new Vector3(
            Mathf.Abs(sum1) % 255 / 255f,
            Mathf.Abs(sum2) % 255 / 255f,
            Mathf.Abs(sum3) % 255 / 255f
            ).normalized*2;
        return new Color(vectorColor.x, vectorColor.y, vectorColor.z);
    }
}
