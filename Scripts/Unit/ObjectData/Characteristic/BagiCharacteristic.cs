using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    [System.Serializable]
    class BagiCharacteristic : Characteristics, IBagiCharacteristic
    {
        public enum BagiAction { none, move, run, attack, death }
        [SerializeField] BagiAction _bagiAction = BagiAction.move;
        [SerializeField] BoxCollider _boxCollider;
        [SerializeField] ParticleSystem _boomParticle;
        [SerializeField] GameObject _fireObject;


        public BagiAction unitAction
        {
            get
            {
                return _bagiAction;
            }

            set
            {
                _bagiAction = value;
            }
        }

        public BoxCollider boxCollider
        {
            get
            {
                return _boxCollider;
            }
        }

        public ParticleSystem boomParticle
        {
            get { return _boomParticle; }
        }

        public GameObject fireObject
        {
            get
            {
                return _fireObject;
            }
        }
    }
}
