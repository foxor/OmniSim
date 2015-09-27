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

        public void Awake()
        {
            for (int i = 0; i < LOCK_COUNT; i++)
            {
                lockTargets[i] = new DummyObject();
            }
        }

        public void LockForSeconds()
        {
            int time = System.DateTime.Now.Minute;
            lock (lockTargets[0])
            {
                for (var i = 0; i < 10; i++)
                {
                    System.Threading.Thread.Sleep(100);
                    Debug.Log("Still waiting");
                }
            }
        }

        public void WaitForLock()
        {
            lock (lockTargets[0])
            {
                Debug.Log("Fired");
            }
        }

        public void Start()
        {
            Thread lock1 = new Thread(LockForSeconds);
            lock1.Start();
            Thread lock2 = new Thread(WaitForLock);
            lock2.Start();
        }
    }
}
