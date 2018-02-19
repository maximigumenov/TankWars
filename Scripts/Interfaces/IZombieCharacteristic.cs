using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    interface IZombieCharacteristic
    {
        float agresiveLook { get; }
        float normalSpeed { get; }
        float agresiveSpeed { get; }
        ParticleSystem bloodParticle { get; }
        ParticleSystem boomParticle { get; }
        ParticleSystem bigBoomParticle { get; }
        ZombieCharacteristic.ZombieAction unitAction { get; set; }
        BoxCollider boxCollider { get; }
    }
}
