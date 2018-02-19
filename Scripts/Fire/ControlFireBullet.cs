using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Fire
{
    public static class ControlFireBullet
    {
        public static int maxBullet = 10;
        static List<BulletData> listBullet = new List<BulletData>();

        public static void AddBullet(GameObject gameObject, FireGun.TypeFire typeFire)
        {
            BulletData bulletData = new BulletData(gameObject, typeFire);
            listBullet.Add(bulletData);
        }

        public static GameObject GetBullet(FireGun.TypeFire typeFire)
        {
            for (int i = 0; i < listBullet.Count; i++)
            {
                if (listBullet[i].typeFire == typeFire)
                {
                    GameObject bullet = listBullet[i].gameObject;
                    listBullet.Add(listBullet[i]);
                    listBullet.Remove(listBullet[i]);
                    return bullet;
                }
            }

            return listBullet[0].gameObject;

        }

        public static bool IsBullet(FireGun.TypeFire typeFire)
        {
            int count = 0;
            for (int i = 0; i < listBullet.Count; i++)
            {
                if (listBullet[i].typeFire == typeFire)
                {
                    count++;
                }
            }
            if (count > maxBullet)
            {
                return true;
            }
            return false;
        }
    }

    public class BulletData
    {

        public GameObject gameObject;
        public FireGun.TypeFire typeFire;

        public BulletData()
        {

        }
        public BulletData(GameObject _gameObject, FireGun.TypeFire _typeFire)
        {
            gameObject = _gameObject;
            typeFire = _typeFire;
        }
    }
}
