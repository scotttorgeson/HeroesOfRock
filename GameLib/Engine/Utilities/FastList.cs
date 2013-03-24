using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib
{
    // fast list, that provides only add and clear functionality
    // iterate over the data array using for () to get the best performance

    // Access times:
    // Foreach Dictionary.Values: 6.8
    // Foreach List: 5.72
    // For List: 2.48
    // Foreach FastList: 1.6
    // For FastList: 1
    // Foreach Array: 1.25
    // For Array: 1

    // Add times:
    // FastList: 1
    // List: 1.7

    public class FastList<T>
    {
        public T[] Data;
        public int Count;
        private int Capacity;

        private const int DEFAULT_LENGTH = 10;

        public FastList()
        {
            Data = new T[DEFAULT_LENGTH];
            Capacity = DEFAULT_LENGTH;
        }

        public FastList(int size)
        {
            Data = new T[size];
            Capacity = size;
        }

        public void Add(T item)
        {
            if (Capacity == Count)
            {
                Capacity *= 2;
                Array.Resize<T>(ref Data, Capacity);
            }

            Data[Count++] = item;
        }

        public void Add(ref T item)
        {
            if (Capacity == Count)
            {
                Capacity *= 2;
                Array.Resize<T>(ref Data, Capacity);
            }

            Data[Count++] = item;
        }

        public void Clear()
        {
            Count = 0;
        }

        public void ClearReferences()
        {
            Count = 0;
            Array.Clear(Data, 0, Data.Length);
        }

        public void Sort()
        {
            Array.Sort<T>(Data, 0, Count); // array.sort is a quicksort
        }

        //public void Remove(T item)
        //{
        //    int index = 0;
        //    for (; index < Count; index++)
        //        if (Data[index].Equals(item))
        //            break;

        //    if (index < Count)
        //    {
        //        // lower instance count and move all instances after the one we removed down a spot
        //        Count--;
        //        for (int i = index; i < Count; i++)
        //            Data[i] = Data[i + 1];
        //    }
        //    else
        //    {
        //        System.Diagnostics.Debug.Assert(false, "Couldn't find the item specified.");
        //    }
        //}

        //public void Remove(ref T item)
        //{
        //    int index = 0;
        //    for (; index < Count; index++)
        //        if (Data[index].Equals(item))
        //            break;

        //    if (index < Count)
        //    {
        //        // lower instance count and move all instances after the one we removed down a spot
        //        Count--;
        //        for (int i = index; i < Count; i++)
        //            Data[i] = Data[i + 1];
        //    }
        //    else
        //    {
        //        System.Diagnostics.Debug.Assert(false, "Couldn't find the item specified.");
        //    }
        //}
    }
}
