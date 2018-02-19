using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unit;
using UnityEngine;

namespace Fire
{
    public class FireGun : IFireGun
    {
        protected MainUnitCharacteristic mainData;
        protected FindUnit findUnit = new FindUnit();
        protected Enemy.FindData findTerrain = new Enemy.FindData();
        public enum TypeFire { none, machineGun, fragmentary, armorPiercer }
        public void DataInit(MainUnitCharacteristic data)
        {
            mainData = data;
        }

        public static FireGun CreateFire(TypeFire type)
        {
            switch (type)
            {
                case TypeFire.machineGun:
                    return new MachineGunFire();
                    break;
                case TypeFire.fragmentary:
                    return new FragmentaryFire();
                    break;
                case TypeFire.armorPiercer:
                    return new ArmorPiercerFire();
                    break;
                default:
                    return new MachineGunFire();
                    break;
            }
        }

        public virtual void ActiveFireEffect()
        {
            mainData._fireData.objectFire.SetActive(true);
        }

        public virtual void DeactiveFireEffect()
        {
            if (mainData != null)
            {
                mainData._fireData.objectFire.SetActive(false);
            }

        }

        public virtual void SetDamage()
        {

        }

        public virtual void SetDamageEmpty(GameObject bullet)
        {

        }


        protected virtual void DamageTerrain(GameObject bullet)
        {

        }

        [System.Serializable]
        public class FireData : IFireData
        {
            [SerializeField] string nameFire;
            public TypeFire _typeFire;
            public bool _isLoop;
            [SerializeField] int _demage;
            [SerializeField] string _description;
            [SerializeField] Sprite _icon;
            public GameObject _objectFire;
            public GameObject _prefabExplosion;
            List<GameObject> listPrefab = new List<GameObject>();
            [HideInInspector] public FireGun _fireGun;

            public TypeFire typeFire
            {
                get
                {
                    return _typeFire;
                }
            }

            public GameObject objectFire
            {
                get
                {
                    return _objectFire;
                }
            }

            public GameObject prefabExplosion
            {
                get
                {
                    return _prefabExplosion;
                }
            }

            public FireGun fireGun
            {
                get
                {
                    return _fireGun;
                }
                set
                {
                    _fireGun = value;
                }
            }

            public bool isLoop
            {
                get
                {
                    return _isLoop;
                }
            }

            public int demage
            {
                get
                {
                    return _demage;
                }
            }

            public string description
            {
                get
                {
                    return _description;
                }
            }

            public Sprite icon { get { return _icon; } }

            public FireData()
            {

                _fireGun = FireGun.CreateFire(_typeFire);

            }
        }
    }
}
