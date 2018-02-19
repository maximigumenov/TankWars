using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    [System.Serializable]
    public class UnitCharacteristics : IUnitCharacteristics
    {
        [SerializeField] float _speedMove;
        [SerializeField] float _speedRotate;

        public float speedMove
        {
            get
            {
                return _speedMove;
            }
        }

        public float speedRotate
        {
            get
            {
                return _speedRotate;
            }
        }
    }
}
