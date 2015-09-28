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

        public class DummyObject { }

        private static DummyObject[] lockTargets = new DummyObject[LOCK_COUNT];

        public static byte[] Data = new byte[DFSLayoutMath.count() * BLOB_SIZE];

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
        }

        public void Start()
        {
            //DFSLayoutMath.verify();
        }
    }
}
