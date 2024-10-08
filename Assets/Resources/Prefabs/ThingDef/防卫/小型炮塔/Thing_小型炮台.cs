using ChenChen_AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ChenChen_Thing
{
    public class Thing_小型炮台 : Building
    {
        [Header("炮筒")]
        public GameObject Top;

        [Header("子弹")]
        public GameObject Bullet;
        public Transform ShootPosition;
        public float Offset = -90f;

        [Header("射击间隔时间和扫描范围")]
        public float shootInterval = 1f;
        public float scanRadius = 5f; // 扫描半径

        protected ObjectPool<GameObject> _bulletPool;

        protected override void Start()
        {
            base.Start();

            _bulletPool = new ObjectPool<GameObject>(Create, Get, Release);
            StartCoroutine(PeriodicScan());
        }

        #region Pool
        private GameObject Create()
        {
            return Instantiate(Bullet);
        }

        private void Get(GameObject bullet)
        {
            bullet.SetActive(true);
            // 设定位置和方向
            bullet.transform.position = ShootPosition.position;
            bullet.transform.rotation = Top.transform.rotation;
        }

        private void Release(GameObject bullet)
        {
            bullet.SetActive(false);
        }

        private void Destroy(GameObject bullet)
        {
            Destroy(bullet);
        }
        #endregion

        #region Bullet
        /// <summary>
        /// 发射子弹
        /// </summary>
        protected virtual void ShootBullet()
        {
            GameObject bullet = _bulletPool.Get();
            //bullet.GetComponent<Bullet>().Shot(_bulletPool);
        }

        /// <summary>
        /// 改变炮塔朝向
        /// </summary>
        /// <param name="targetPosition"></param>
        public void RotateTopTowards(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - Top.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + Offset;
            Top.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        /// <summary>
        /// 定期扫描周围敌人
        /// </summary>
        public void Scan()
        {
            //Todo
        }


        private IEnumerator PeriodicScan()
        {
            while (true)
            {
                Scan();
                yield return new WaitForSeconds(shootInterval); // 每0.5秒扫描一次
            }
        }
        #endregion
    }
}