using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class MessageIntention
    {
        public int origin;
        public int destination;
    }

    class Worker
    {
        public bool shutDown = false;

        public int minIndex;
        public int maxIndex;

        protected Queue<MessageIntention> messageQueue = new Queue<MessageIntention>();

        protected byte[] messageSwapBuffer = new byte[SimulationEngine.MESSAGE_SIZE];

        protected VirtualMachine vm;

        public Worker(int minIndex, int maxIndex)
        {
            vm = new VirtualMachine(PushMessage);
            this.minIndex = minIndex;
            this.maxIndex = maxIndex;
        }

        protected void Execute(int voxelOffset, byte instructionPointer)
        {
            vm.Process(voxelOffset, instructionPointer);
        }

        protected void ExecutePrimary(int voxelIndex)
        {
            lock (SimulationEngine.lockTarget(voxelIndex))
            {
                Execute(voxelIndex * SimulationEngine.BLOB_SIZE, SimulationEngine.PRIMARY_EXECUTION_OFFSET);
            }
        }

        protected void DrainMessageQueue()
        {
            MessageIntention message = messageQueue.Dequeue();
            lock (SimulationEngine.lockTarget(message.origin))
            {
                // this can be faster: https://msdn.microsoft.com/en-us/library/28k1s2k6(VS.80).aspx
                int messageRoot = message.origin * SimulationEngine.BLOB_SIZE + SimulationEngine.OUTGOING_MESSAGE_OFFSET;

                for (byte messageOffset = 0; messageOffset < SimulationEngine.MESSAGE_SIZE; messageOffset++)
                {
                    messageSwapBuffer[messageOffset] = SimulationEngine.Data[messageRoot + messageOffset];
                }
            }
            lock (SimulationEngine.lockTarget(message.destination))
            {
                // this can be faster: https://msdn.microsoft.com/en-us/library/28k1s2k6(VS.80).aspx
                int messageRoot = message.destination * SimulationEngine.BLOB_SIZE + SimulationEngine.OUTGOING_MESSAGE_OFFSET;

                for (byte messageOffset = 0; messageOffset < SimulationEngine.MESSAGE_SIZE; messageOffset++)
                {
                    SimulationEngine.Data[messageRoot + messageOffset] = messageSwapBuffer[messageOffset];
                }
                
                Execute(message.destination * SimulationEngine.BLOB_SIZE, SimulationEngine.PRIMARY_EXECUTION_OFFSET);
            }
        }

        protected void PushMessage(MessageIntention message)
        {
            messageQueue.Enqueue(message);
        }

        public void Run()
        {
            while (!shutDown)
            {
                for (int voxelIndex = minIndex; voxelIndex <= maxIndex && !shutDown; voxelIndex++)
                {
                    ExecutePrimary(voxelIndex);
                }
                while (messageQueue.Any() && !shutDown)
                {
                    DrainMessageQueue();
                }
            }
        }
    }
}
