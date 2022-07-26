using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame.Environment
{
    internal class PointOfEnergy
    {
        public Vector2Int position;
        public float currentEnergy;

        public PointOfEnergy()
        {
        }

        public PointOfEnergy(Vector2Int position, float currentEnergy)
        {
            this.position = position;
            this.currentEnergy = currentEnergy;
        }
    }
}
