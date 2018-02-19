using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace Unit
{
    public class SimpleMove : IMove
    {
        ICharacteristics unitCharacteristics;

        UnitEvents moveEvents = new UnitEvents();
        bool isMove = true;
        Vector3 vectorZero = Vector3.zero;
        public UnitEvents unitEvents
        {
            get
            {
                return moveEvents;
            }

            set
            {
                moveEvents = value;
            }
        }

        public void Initialization(Characteristics unitCharacteristics)
        {
            this.unitCharacteristics = unitCharacteristics;
        }

        public void Move(Transform mainTransform, Transform target)
        {
            if (isMove)
            {
                float step = unitCharacteristics.speedMove * Time.fixedDeltaTime;
                mainTransform.position = Vector3.MoveTowards(mainTransform.position, target.position, step);
                EndMove(mainTransform, target);
            }
        }

        public void Move(NavMeshAgent agent, Transform target)
        {
            if (isMove)
            {
                agent.destination = target.position;
                EndMove(agent.transform, target);
            }
            else
            {
                agent.destination = agent.transform.position;
            }

        }

        public void Start()
        {
            isMove = true;
        }

        public void Stop()
        {
            isMove = false;
        }

        public void TurnToTarget(Transform mainTransform, Transform target)
        {
            if (target)
            {
                Vector3 dir = target.position - mainTransform.position;
                Quaternion rot = Quaternion.LookRotation(dir);
                mainTransform.rotation = Quaternion.Slerp(mainTransform.rotation, rot, unitCharacteristics.speedRotate * Time.deltaTime);
            }
        }

        void EndMove(Transform mainTransform, Transform target)
        {
            if (isMove)
            {
                float dist = Vector3.Distance(target.position, mainTransform.position);
                if (dist < 0.5f)
                {
                    moveEvents.endMove.Invoke();
                }
            }
        }
    }
}
