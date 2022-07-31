using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame.Environment
{
    [Serializable]
    public class PointOfEnergy
    {
        public Vector2Int position;
        public float currentEnergy;
        public float basicEnergy;

        public PointOfEnergy()
        {
        }

        public PointOfEnergy(Vector2Int position, float currentEnergy)
        {
            this.position = position;
            this.currentEnergy = currentEnergy;
        }
        public PointOfEnergy(Vector2Int position)
        {
            this.position = position;
            this.currentEnergy = basicEnergy;
        }
        public void Reset()
        {
            this.currentEnergy = basicEnergy;

        }
    }
}
