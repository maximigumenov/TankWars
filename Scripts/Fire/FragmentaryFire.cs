using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unit;
using UnityEngine;

namespace Fire
{
    public class FragmentaryFire : FireGun
    {
        float radius = 1;
        float maxDistance = 10000;
        float findRadius = 10;
        float findDistance = 5;

        Enemy.FindData findObjects = new Enemy.FindData();
        public override void SetDamage()
        {
            base.SetDamage();
            FindObject(mainData.enemyTarget.transform);
        }

        public override void SetDamageEmpty(GameObject bullet)
        {
            base.SetDamageEmpty(bullet);

            ICharacteristics charact = mainData;
            IMainUnitCharacteristic mainCharact = mainData;

            findTerrain.positiveFind = () => DamageTerrain(bullet);
            findTerrain.negativeFind = () => { };
            findTerrain.radius = radius;
            findTerrain.maxDistance = maxDistance;
            findTerrain.direction = Vector3.down;
            findTerrain.typeUnit = TypeUnit.terrain;
            findUnit.FindTerrain(findTerrain, mainCharact.targetShoot);

        }

        protected override void DamageTerrain(GameObject bullet)
        {
            base.DamageTerrain(bullet);
            RaycastHit hit = findTerrain.hits[0];
            GameObject obj = bullet;
            obj.GetComponent<ParticleSystem>().Play();
            obj.transform.position = hit.point;
            FindObject(obj.transform);
        }

        void FindObject(Transform target)
        {
            findObjects.positiveFind = () => SetAdditionalDamage();
            findObjects.negativeFind = () => { };
            findObjects.radius = findRadius;
            findObjects.maxDistance = findDistance;
            findObjects.direction = Vector3.up;
            findObjects.typeUnit = TypeUnit.enemy;
            findUnit.FindObject(findObjects, target);
        }

        void SetAdditionalDamage()
        {

            RaycastHit[] hits = findObjects.hits;

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.transform.GetComponent<Enemy>() != null)
                {
                    IReactionShot reaction = hits[i].collider.transform.GetComponent<IReactionShot>();
                    IMainUnitCharacteristic mainCharact = mainData;

                    reaction.ReactionShot(TypeFire.fragmentary);
                }
            }
        }
    }
}
