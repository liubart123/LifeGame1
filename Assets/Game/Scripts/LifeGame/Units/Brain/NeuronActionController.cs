using Assets.Game.Scripts.LifeGame.Map;
using Assets.Game.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.LifeGame.Units.Brain
{
    internal class NeuronActionController : Singleton<NeuronActionController>
    {
        MapController mapController;
        LifeGameEngine lifeGameEngine;

        public void Initialize()
        {
            lifeGameEngine = LifeGameEngine.Instance;
            mapController = MapController.Instance;
        }

        public void PerformActionByNeuronIndex(int neuronIndex, Unit unit)
        {
            switch (neuronIndex)
            {
                case 0:
                    lifeGameEngine.TryMoveUnit(unit, unit.position + Vector2Int.up);
                    break;
                case 1:
                    lifeGameEngine.TryMoveUnit(unit, unit.position + Vector2Int.right);
                    break;
                case 2:
                    lifeGameEngine.TryMoveUnit(unit, unit.position + Vector2Int.down);
                    break;
                case 3:
                    lifeGameEngine.TryMoveUnit(unit, unit.position + Vector2Int.left);
                    break;
            }
        }
    }
}
