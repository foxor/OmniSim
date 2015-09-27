using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets
{
    class SimulationEngine : MonoBehaviour
    {
        protected const int LOCK_COUNT = 100;

        protected class DummyObject { }

        private static DummyObject[] lockTargets = new DummyObject[LOCK_COUNT];

        public static byte[] Data = new byte[72];

        public void Awake()
        {
            for (int i = 0; i < LOCK_COUNT; i++)
            {
                lockTargets[i] = new DummyObject();
            }
        }

        public void LockForSeconds()
        {
            lock (lockTargets[0])
            {
                for (var i = 0; i < 10; i++)
                {
                    System.Threading.Thread.Sleep(100);
                    Data[50] = 44;
                }
                Debug.Log("" + Data[4]);
            }
        }

        public void WaitForLock()
        {
            lock (lockTargets[0])
            {
                Data[4] = 45;
                Debug.Log("" + Data[50]);
            }
        }

        public void Start()
        {
            Thread lock1 = new Thread(LockForSeconds);
            lock1.Start();
            Thread lock2 = new Thread(WaitForLock);
            lock2.Start();
            DFSLayoutMath.verify();
        }
    }
}
