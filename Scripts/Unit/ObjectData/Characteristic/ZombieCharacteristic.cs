using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    [System.Serializable]
    public class ZombieCharacteristic : Characteristics, IZombieCharacteristic
    {
        public enum ZombieAction { none, move, run, attack, death }
        [Header("Zombie Look")]
        [SerializeField]
        float _agresiveLook = 50;
        [Header("Zombie Speed")]
        [SerializeField]
        float _normalSpeed = 1;
        [SerializeField] float _agresiveSpeed = 10;
        [Space(10)]
        [Header("Zombie Action")]
        [SerializeField]
        ZombieAction _unitAction;
        [Header("Fire Data")]
        [SerializeField]
        ParticleSystem _bloodParticle;
        [SerializeField] ParticleSystem _boomParticle;
        [SerializeField] ParticleSystem _bigBoomParticle;
        [Header("Zombie Objects")]
        [SerializeField]
        BoxCollider _boxCollider;

        public float agresiveLook
        {
            get
            {
                return _agresiveLook;
            }
        }

        public float normalSpeed
        {
            get
            {
                return _normalSpeed;
            }
        }

        public float agresiveSpeed
        {
            get
            {
                return _agresiveSpeed;
            }
        }

        public ZombieAction unitAction
        {
            get
            {
                return _unitAction;
            }

            set
            {
                _unitAction = value;
            }
        }

        public ParticleSystem bloodParticle
        {
            get
            {
                return _bloodParticle;
            }
        }

        public ParticleSystem boomParticle
        {
            get
            {
                return _boomParticle;
            }
        }

        public ParticleSystem bigBoomParticle
        {
            get
            {
                return _bigBoomParticle;
            }
        }

        public BoxCollider boxCollider
        {
            get
            {
                return _boxCollider;
            }
        }
    }
}
