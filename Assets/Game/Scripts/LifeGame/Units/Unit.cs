using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame.Units
{
    public class Unit
    {
        public bool isLive = true;
        public Vector2Int position;
        /// <summary>
        /// [layer of source neuron] [source neuron numbber] [target neuron number]
        /// </summary>
        public float[][][] synopses;

    }
}
