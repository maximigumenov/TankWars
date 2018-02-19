using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    public interface IMove
    {
        UnitEvents unitEvents { get; set; }
        void Start();
        void Stop();
        void Initialization(Characteristics unitCharacteristics);
        void Move(Transform mainTransform, Transform target);
        void TurnToTarget(Transform mainTransform, Transform target);
    }
}
