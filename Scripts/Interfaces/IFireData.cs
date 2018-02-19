using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Fire
{
    interface IFireData
    {
        bool isLoop { get; }
        int demage { get; }
        string description { get; }
        Sprite icon { get; }
        FireGun.TypeFire typeFire { get; }
        GameObject objectFire { get; }
        GameObject prefabExplosion { get; }
        FireGun fireGun { get; set; }
    }
}
