using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    interface IDataUnit
    {
        TypeUnit typeUnit { get; }
        bool isLife { get; set; }
        void ActionUnit_Start(Vector3 position);
    }

    public enum TypeUnit { none, enemy, main, terrain, weapon }
}
