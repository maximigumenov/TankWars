using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    interface ICharacteristics
    {
        //data
        float defense { get; }
        int hp { get; set; }
        int maxHp { get; }
        //speed
        float speedMove { get; set; }
        float speedRotate { get; }
        //attack
        float maxDistanceAttackLook { get; }
        float radiusAttackLook { get; }
        int damage { get; }
        //object
        Transform transformObject { get; }
    }
}
