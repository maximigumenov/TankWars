using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    [System.Serializable]
    public class Characteristics : ICharacteristics
    {
        [Header("Data Object")]
        [SerializeField]
        Transform _transformObject;
        [SerializeField] [Range(0, 1)] float _defense;
        [SerializeField] int HP;
        [SerializeField] int maxHP;
        [Header("Speed")]
        [SerializeField]
        float _speedMove = 1;
        [SerializeField] float _speedRotate = 1;
        [Header("Attack Data")]
        [SerializeField]
        float _maxDistanceAttackLook = 1;
        [SerializeField] float _radiusAttackLook = 1;
        [SerializeField] int _damage = 1;



        public float speedMove
        {
            get
            {
                return _speedMove;
            }

            set
            {
                _speedMove = value;
            }
        }

        public float speedRotate
        {
            get
            {
                return _speedRotate;
            }
        }

        public Transform transformObject
        {
            get
            {
                return _transformObject;
            }
        }

        public float maxDistanceAttackLook
        {
            get
            {
                return _maxDistanceAttackLook;
            }
        }

        public float radiusAttackLook
        {
            get
            {
                return _radiusAttackLook;
            }
        }

        public int hp
        {
            get
            {
                return HP;
            }

            set
            {
                HP = value;
            }
        }

        public int maxHp
        {
            get
            {
                return maxHP;
            }
        }

        public float defense
        {
            get
            {
                return _defense;
            }
        }

        public int damage
        {
            get
            {
                return _damage;
            }
        }
    }
}
