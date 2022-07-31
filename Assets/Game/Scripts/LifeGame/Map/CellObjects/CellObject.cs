using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame.Map.CellObjects
{
    public class CellObject
    {
        public Vector2Int position;

        public CellObject()
        {
        }

        public CellObject(Vector2Int position)
        {
            this.position = position;
        }
    }
}
