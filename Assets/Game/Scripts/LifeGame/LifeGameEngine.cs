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
    internal class LifeGameEngine : Singleton<LifeGameEngine>
    {
        MapController mapController;

        public void Initialize()
        {
            mapController = MapController.Instance;
        }

        public void MoveUnit(Unit unit, Vector2Int newPos)
        {
            mapController.GetCell(unit.position).unit = null;
            mapController.GetCell(newPos).SetUnit(unit);
            unit.position = newPos;
        }
        public void TryMoveUnit(Unit unit, Vector2Int newPos)
        {
            if (mapController.IsPositionInsideTheMap(newPos) && 
                mapController.GetCell(newPos).IsFree())
                MoveUnit(unit, newPos);
        }
    }
}
