using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Fire;

namespace Unit
{
    interface IMainUnitCharacteristic
    {
        float gravity { get; }
        float acceleration { get; }
        float maxSpeedAcceleration { get; }
        float headSpeedRotate { get; }
        Transform standartTarget { get; }
        Transform enemyTarget { get; set; }
        Transform head { get; }
        Transform targetShoot { get; }
        FireGun.FireData fireData { get; set; }
        List<FireGun.FireData> listFireData { get; }
        float warning { get; }
        float endTerritory { get; }
    }
}
