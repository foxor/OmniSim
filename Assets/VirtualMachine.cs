using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class VirtualMachine
    {
        protected const int INSTRUCTION_LIMIT = 400;

        protected Action<MessageIntention> onMessage;

        protected Stack<byte> registerStack;

        protected byte activeRegister
        {
            get
            {
                return registerStack.Peek();
            }
        }

        public VirtualMachine(Action<MessageIntention> onMessage)
        {
            this.onMessage = onMessage;
        }

        public void Process(int voxelOffset, byte instructionPointer)
        {
            for (int instructionCount = 0; instructionCount < INSTRUCTION_LIMIT; instructionCount++)
            {
                byte instruction = SimulationEngine.Data[voxelOffset + instructionPointer];
                byte argument = SimulationEngine.Data[voxelOffset + instructionPointer + 1];

                switch (instruction)
                {
                    default: // Stop
                        return;
                }
            }
        }
    }
}
