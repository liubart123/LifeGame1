using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame.Map.CellObjects
{
    public class Obstacle : CellObject
    {
        public Obstacle()
        {
        }

        public Obstacle(Vector2Int position) : base(position)
        {
        }

    }
}
