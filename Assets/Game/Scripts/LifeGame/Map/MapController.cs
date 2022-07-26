﻿using Assets.Game.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame.Map
{
    public class MapController : Singleton<MapController>
    {
        MapCell[,] cells;
        public int width, height;


        public enum EDirection
        {
            up,upRIght,right,downRight,down,downLeft,left,leftUp
        }

        public void ResetMap()
        {
            foreach(var cell in cells)
            {
                cell.Reset();
            }
        }
        public void CreateMap()
        {
            cells = new MapCell[width,height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x,y] = new MapCell();
                }
            }
        }

        public MapCell GetNeighbour(Vector2Int pos, EDirection direction)
        {
            Vector2Int neighbourPos = pos;
            switch (direction)
            {
                case EDirection.up:
                    neighbourPos.y++;
                    break;
                case EDirection.upRIght:
                    neighbourPos.x++;
                    neighbourPos.y++;
                    break;
                case EDirection.right:
                    neighbourPos.x++;
                    break;
                case EDirection.downRight:
                    neighbourPos.x++;
                    neighbourPos.y--;
                    break;
                case EDirection.down:
                    neighbourPos.y--;
                    break;
                case EDirection.downLeft:
                    neighbourPos.x--;
                    neighbourPos.y--;
                    break;
                case EDirection.left:
                    neighbourPos.x--;
                    break;
                case EDirection.leftUp:
                    neighbourPos.x--;
                    neighbourPos.y++;
                    break;
            }

            if (!IsPositionInsideTheMap(neighbourPos))
                return null;
            return GetCell(neighbourPos);
        }
        public bool IsEndOfMap(Vector2Int pos)
        {
            return pos.x == 0 || pos.y == 0 || pos.x == width - 1 || pos.y == height - 1;
        }
        public bool IsPositionInsideTheMap(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
        }

        public MapCell GetCell(Vector2Int pos)
        {
            return cells[pos.x, pos.y];
        }
        public MapCell GetCell(int x, int y)
        {
            return cells[x, y];
        }
    }
}
