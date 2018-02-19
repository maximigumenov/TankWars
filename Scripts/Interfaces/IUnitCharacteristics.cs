using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Unit
{
    interface IUnitCharacteristics
    {
        float speedMove { get; }
        float speedRotate { get; }
    }
}
