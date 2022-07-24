using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Game.Scripts.LifeGame.Map.CellObjects;
using Assets.Game.Scripts.LifeGame.Units;

namespace Assets.Game.Scripts.LifeGame.Map
{
    public class MapCell
    {
        public Unit unit;
        public CellObject cellObject;

        public virtual void Reset()
        {
            unit = null;
            cellObject = null;
        }

        public virtual bool IsFree()
        {
            return unit == null && cellObject == null;
        }

        public void SetUnit(Unit unit)
        {
            if (!IsFree())
                return;
            this.unit = unit;
        }
        public void SetCellObject(CellObject cellObject)
        {
            if (!IsFree())
                return;
            this.cellObject = cellObject;
        }
    }
}
