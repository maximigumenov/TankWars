using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unit;
using UnityEngine;

namespace Fire
{
    public class MachineGunFire : FireGun
    {
        float radius = 1;
        float maxDistance = 10000;

        public override void SetDamage()
        {
            base.SetDamage();

            IReactionShot reaction = mainData.enemyTarget.GetComponent<IReactionShot>();
            IMainUnitCharacteristic mainCharact = mainData;

            reaction.ReactionShot(mainCharact.fireData._typeFire);
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
        }
    }
}
