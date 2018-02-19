using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Fire;

namespace Unit
{
    [System.Serializable]
    public class MainUnitCharacteristic : Characteristics, IMainUnitCharacteristic
    {
        [Header("Gravity Data")]
        [SerializeField]
        float _gravity = 6;
        [Header("Speed Tank Head")]
        [SerializeField]
        float _acceleration;
        [SerializeField] float _maxSpeedAcceleration;
        [SerializeField] float _headSpeedRotate;
        [Header("Transform Target")]
        [SerializeField]
        Transform _standartTarget;
        [SerializeField] Transform _targetShoot;
        public Transform _enemyTarget;
        [SerializeField] Transform _head;

        public int nowNumberWeapon = 0;
        public FireGun.FireData _fireData = new FireGun.FireData();
        [Header("Fire Data")]
        public List<FireGun.FireData> _listFireData;
        [Header("Borders of the territory")]
        [SerializeField]
        float _warning = 370;
        [SerializeField] float _endTerritory = 440;


        public float gravity
        {
            get
            {
                return _gravity;
            }
        }


        public float acceleration
        {
            get
            {
                return _acceleration;
            }
        }

        public float maxSpeedAcceleration { get { return _maxSpeedAcceleration; } }

        public float headSpeedRotate { get { return _headSpeedRotate; } }

        public Transform standartTarget { get { return _standartTarget; } }

        public Transform enemyTarget
        {
            get
            {
                return _enemyTarget;
            }

            set
            {
                _enemyTarget = value;
            }

        }

        public Transform head { get { return _head; } }

        public FireGun.FireData fireData
        {
            get
            {
                return _fireData;
            }

            set
            {
                _fireData = value;
            }
        }

        public List<FireGun.FireData> listFireData
        {
            get
            {
                return _listFireData;
            }

        }

        public float warning
        {
            get
            {
                return _warning;
            }
        }

        public float endTerritory
        {
            get
            {
                return _endTerritory;
            }
        }

        public Transform targetShoot
        {
            get
            {
                return _targetShoot;
            }
        }
    }
}
