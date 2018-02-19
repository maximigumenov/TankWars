using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    interface IFindUnit
    {
        void FindObject(Enemy.FindData data, Transform mainTransform);
        void FindTerrain(Enemy.FindData data, Transform mainTransform);
    }
}
