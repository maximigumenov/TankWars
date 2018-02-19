using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unit;
using UnityEngine;

namespace Fire
{
    interface IFireGun
    {
        void DataInit(MainUnitCharacteristic data);
        void SetDamage();
        void SetDamageEmpty(GameObject bullet);
        void ActiveFireEffect();
        void DeactiveFireEffect();
    }
}
