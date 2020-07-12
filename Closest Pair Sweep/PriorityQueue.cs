using System;
using System.Collections.Generic;

namespace Closest_Pair_Sweep {
    public class PriorityQueue<P, V> where P : IComparable<P> {

        private readonly List<(P Priority, V Value)> prv_List = new List<(P, V)>();
        private int prv_LastIndex = -1;

        public PriorityQueue() { }

        public PriorityQueue(IEnumerable<(P, V)> aInit) {
            foreach (var lcl_Element in aInit) {
                Push(lcl_Element);
            }
        }

        public void AddRange(IEnumerable<(P Priority, V Value)> aEnumerable) {
            foreach (var lcl_Item in aEnumerable) {
                Push(lcl_Item);
            }
        }

        public void Push(P aPriority, V aValue) => Push((aPriority, aValue));

        public void Push((P Priority, V Value) aElement) {
            prv_LastIndex++;
            prv_List.Insert(prv_LastIndex, aElement);
            UpdateUp(prv_LastIndex);
        }

        public V Pop() {
            V lcl_Result = prv_List[0].Value;
            RemoveMax();
            return lcl_Result;
        }

        public bool Empty => prv_LastIndex < 0;

        public int Count => prv_LastIndex + 1;

        public void RemoveMax() {
            if (Empty)
                throw new IndexOutOfRangeException();
            prv_List[0] = prv_List[prv_LastIndex];
            prv_List.RemoveAt(prv_LastIndex);
            prv_LastIndex--;
            UpdateDown(0);
        }

        // Method based on https://en.wikipedia.org/w/index.php?title=Binary_heap&oldid=951213406#Extract
        private void UpdateDown(int aStart) {
            int lcl_Min = aStart;
            if (Left(aStart) < prv_List.Count && prv_List[Left(aStart)].Priority.CompareTo(prv_List[aStart].Priority) < 0)
                lcl_Min = Left(aStart);
            if (Right(aStart) < prv_List.Count && prv_List[Right(aStart)].Priority.CompareTo(prv_List[lcl_Min].Priority) < 0)
                lcl_Min = Right(aStart);
            if (lcl_Min != aStart) {
                Swap(aStart, lcl_Min);
                UpdateDown(lcl_Min);
            }
        }

        private int Left(int aParent) => (aParent + 1) * 2 - 1;
        private int Right(int aParent) => (aParent + 1) * 2;

        private void UpdateUp(int aStart) {
            if (aStart < 1)
                return;
            int lcl_ParentIdx = ((aStart + 1) / 2) - 1;
            if (prv_List[aStart].Priority.CompareTo(prv_List[lcl_ParentIdx].Priority) < 0) {
                Swap(aStart, lcl_ParentIdx);
                UpdateUp(lcl_ParentIdx);
            }
        }

        private void Swap(int aA, int aB) {
            (P Priority, V Value) lcl_A = prv_List[aA];
            prv_List[aA] = prv_List[aB];
            prv_List[aB] = lcl_A;
        }

        public override string ToString() => string.Join(", ", prv_List);

    }
}