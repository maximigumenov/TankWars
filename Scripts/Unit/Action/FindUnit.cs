using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unit
{
    public class FindUnit : IFindUnit
    {
        public void FindObject(Enemy.FindData data, Transform mainTransform)
        {
            List<RaycastHit> listHits = new List<RaycastHit>();
            Vector3 vector = mainTransform.TransformDirection(data.direction);
            RaycastHit[] hits = Physics.SphereCastAll(mainTransform.position, data.radius, vector, data.maxDistance);
            bool isFind = false;
            for (int i = 0; i < hits.Length; i++)
            {
                if (
                    (hits[i].collider.GetComponent<IDataUnit>() != null) &&
                    (hits[i].collider.GetComponent<IDataUnit>().typeUnit == data.typeUnit) &&
                     (hits[i].collider.GetComponent<IDataUnit>().isLife == true)
                    )
                {
                    listHits.Add(hits[i]);
                    isFind = true;
                }
            }

            data.hits = listHits.ToArray();
            if (isFind)
            {
                data.positiveFind();
            }
            else
            {
                data.negativeFind();
            }
        }

        public void FindTerrain(Enemy.FindData data, Transform mainTransform)
        {
            List<RaycastHit> listHits = new List<RaycastHit>();
            Vector3 vector = mainTransform.TransformDirection(data.direction);
            RaycastHit[] hits = Physics.SphereCastAll(mainTransform.position, data.radius, vector, data.maxDistance);
            bool isFind = false;
            for (int i = 0; i < hits.Length; i++)
            {
                if (
                    (hits[i].collider.GetComponent<IUnitTag>() != null) &&
                    (hits[i].collider.GetComponent<IUnitTag>().typeUnit == TypeUnit.terrain)
                    )
                {
                    listHits.Add(hits[i]);
                    isFind = true;
                }
            }

            data.hits = listHits.ToArray();

            if (isFind)
            {
                data.positiveFind();
            }
            else
            {
                data.negativeFind();
            }
        }
    }
}
