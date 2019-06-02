using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseWork_Project.Rendering
{
    public class BatchQueue<T>
    {
        private T[] queue;
        private int frontPointer = -1, rearPointer = -1;

        public int ItemCount { get; set; } = 0;

        public readonly int Size;

        public BatchQueue(int size)
        {
            Size = size;
            queue = new T[Size];
        }

        //Add item to the queue.
        public void EnQueue(T item)
        {
            //If queue is not full...
            if (!IsFull())
            {
                //If there are no itmes in the queue (i.e. the front and rear pointers are set to -1)...
                if (frontPointer == -1 && rearPointer == -1)
                {
                    frontPointer = 0;
                    rearPointer = 0;
                }//If the rearPointer is currently equal to last index of the queue array...
                else if (rearPointer == Size - 1)
                {
                    //The index wraps around to zero.
                    rearPointer = 0;
                }
                else //Else the rearPointer is incremented.
                {
                    rearPointer++;
                }


                queue[rearPointer] = item;

                ItemCount++;

            }

        }

        //Remove an item from the queue.
        public void DeQueue()
        {
            //If the queue is not empty...
            if (!IsEmpty())
            {
                queue[frontPointer] = default(T);

                //If the frontPointer is currently equal to the last index of the queue array...
                if (frontPointer == Size - 1)
                {
                    //The index wraps around to zero.
                    frontPointer = 0;
                }
                else//Else the rearPointer is incremented.
                {
                    frontPointer++;
                }


                ItemCount--;
            }
        }

        //Get from the queue.
        public T GetItem(int index)
        {
            var newIndex = frontPointer + index;

            if (newIndex > Size - 1)
                newIndex -= Size ;

            return queue[newIndex];
        }

        //Return an array containing all items from the queue.
        public T[] GetItems()
        {
            var queueContent = new T[ItemCount];

            for (int i = 0; i < ItemCount ; i++)
            {
                var item = GetItem(i);
                    queueContent[i] = item;
            }

            return queueContent;
        }

        //Clear queue and reset to default state.
        public void Clear()
        {
            for(int i = 0; i < Size; i++)
            {
                queue[i] = default(T);
            }

            frontPointer = -1;
            rearPointer = -1;
            ItemCount = 0;
        }

        //Check whether the queue is full.
        public bool IsFull()
        {
            if (ItemCount == Size)                
                return true;
            else
                return false;
        }

        //Check whether the queue is empty.
        public bool IsEmpty()
        {
            if (frontPointer == rearPointer)
                return true;
            else
                return false;
        }
        
    }
}
