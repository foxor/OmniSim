using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets
{
    enum SimulationStatus
    {
        Breeding = 0,
        Simulating,
        Busy,
        Evaluating,
    }

    class SimulationEngine : MonoBehaviour
    {
        public const int BLOB_SIZE = 256;
        public const int PRIMARY_EXECUTION_OFFSET = 0;
        public const int OUTGOING_MESSAGE_OFFSET = 64;
        public const int LAYER_TIME_INDEX_OFFSET = 64;
        public const int COLOR_OFFSET = 65;
        public const int MESSAGE_EXECUTION_OFFSET = 128;
        public const int INCOMING_MESSAGE_OFFSET = 192;
        public const int MESSAGE_SIZE = 64;
        public const int EXECUTION_BLOCK_SIZE = 64;
        public const int INSTRUCTION_SIZE = 2;

        protected const int LOCK_COUNT = 100;

        protected const float SIMULATION_TIME = 10f;

        protected static int WORKER_COUNT = Environment.ProcessorCount;

        public class DummyObject { }

        private static DummyObject[] lockTargets = new DummyObject[LOCK_COUNT];

        public static byte[] Data = new byte[DFSLayoutMath.count() * BLOB_SIZE];

        protected Worker[] workers = new Worker[WORKER_COUNT];
        protected Thread[] workerThreads = new Thread[WORKER_COUNT];

        protected SimulationStatus status;

        public static DummyObject lockTarget (int voxelIndex)
        {
            voxelIndex = ((voxelIndex >> 16) ^ voxelIndex) * 0x45d9f3b;
            voxelIndex = ((voxelIndex >> 16) ^ voxelIndex) * 0x45d9f3b;
            voxelIndex = ((voxelIndex >> 16) ^ voxelIndex);
            return lockTargets[voxelIndex % LOCK_COUNT];
        }

        public void Awake()
        {
            for (int i = 0; i < LOCK_COUNT; i++)
            {
                lockTargets[i] = new DummyObject();
            }
            int lastVoxelIndex = -1;
            for (int i = 0; i < WORKER_COUNT; i++)
            {
                workers[i] = new Worker(lastVoxelIndex + 1, lastVoxelIndex = (DFSLayoutMath.count() * (i + 1)) / WORKER_COUNT - 1);
                workerThreads[i] = new Thread(workers[i].Run);
            }
        }

        public void Start()
        {
            //DFSLayoutMath.verify();
        }

        protected IEnumerator simulationCoroutine()
        {
            status = SimulationStatus.Busy;

            for (int i = 0; i < WORKER_COUNT; i++)
            {
                workers[i].shutDown = false;
                workerThreads[i].Start();
            }
            yield return new WaitForSeconds(SIMULATION_TIME);

            for (int i = 0; i < WORKER_COUNT; i++)
            {
                workers[i].shutDown = true;
            }
            for (int i = 0; i < WORKER_COUNT; i++)
            {
                workerThreads[i].Join();
            }

            status = SimulationStatus.Evaluating;
            yield break;
        }

        protected IEnumerator evaluationCoroutine()
        {
            status = SimulationStatus.Busy;

            status = SimulationStatus.Breeding;
            yield break;
        }

        protected IEnumerator breedingCoroutine()
        {
            status = SimulationStatus.Busy;

            status = SimulationStatus.Simulating;
            yield break;
        }

        public void Update()
        {
            switch (status)
            {
                case SimulationStatus.Simulating:
                    StartCoroutine(simulationCoroutine());
                    break;
                case SimulationStatus.Evaluating:
                    StartCoroutine(evaluationCoroutine());
                    break;
                case SimulationStatus.Breeding:
                    StartCoroutine(breedingCoroutine());
                    break;
            }
        }
    }
}
