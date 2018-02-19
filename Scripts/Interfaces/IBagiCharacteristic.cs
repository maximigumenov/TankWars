using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    interface IBagiCharacteristic
    {
        BagiCharacteristic.BagiAction unitAction { get; set; }
        BoxCollider boxCollider { get; }
        ParticleSystem boomParticle { get; }
        GameObject fireObject { get; }
    }
}
