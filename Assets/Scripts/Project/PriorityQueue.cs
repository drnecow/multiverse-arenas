using System;
using UnityEngine.Assertions;
 
namespace Gemini
{
    public sealed class IndexedPriorityQueue<T> where T : IComparable
    {
        #region public
        public int Count
        {
            get { return m_count; }
        }

        public T this[int index]
        {
            get
            {
                Assert.IsTrue(index < m_objects.Length && index >= 0,
                               string.Format("IndexedPriorityQueue.[]: Index '{0}' out of range", index));
                return m_objects[index];
            }

            set
            {
                Assert.IsTrue(index < m_objects.Length && index >= 0,
                               string.Format("IndexedPriorityQueue.[]: Index '{0}' out of range", index));
                Set(index, value);
            }
        }

        public IndexedPriorityQueue(int maxSize)
        {
            Resize(maxSize);
        }

        /// 
        /// Inserts a new value with the given index
        /// 
        /// index to insert at
        /// value to insert
        public void Insert(int index, T value)
        {
            Assert.IsTrue(index < m_objects.Length && index >= 0,
                           string.Format("IndexedPriorityQueue.Insert: Index '{0}' out of range", index));

            ++m_count;

            // add object
            m_objects[index] = value;

            // add to heap
            m_heapInverse[index] = m_count;
            m_heap[m_count] = index;

            // update heap
            SortHeapUpward(m_count);
        }

        /// 
        /// Gets the top element of the queue
        /// 
        /// The top element
        public T Top()
        {
            // top of heap [first element is 1, not 0]
            return m_objects[m_heap[1]];
        }

        /// 
        /// Removes the top element from the queue
        /// 
        /// The removed element
        public T Pop()
        {
            Assert.IsTrue(m_count > 0, "IndexedPriorityQueue.Pop: Queue is empty");

            if (m_count == 0)
            {
                return default(T);
            }

            // swap front to back for removal
            Swap(1, m_count--);

            // re-sort heap
            SortHeapDownward(1);

            // return popped object
            return m_objects[m_heap[m_count + 1]];
        }

        /// 
        /// Updates the value at the given index. Note that this function is not
        /// as efficient as the DecreaseIndex/IncreaseIndex methods, but is
        /// best when the value at the index is not known
        /// 
        /// index of the value to set
        /// new value
        public void Set(int index, T obj)
        {
            if (obj.CompareTo(m_objects[index]) <= 0)
            {
                DecreaseIndex(index, obj);
            }
            else
            {
                IncreaseIndex(index, obj);
            }
        }

        /// 
        /// Decrease the value at the current index
        /// 
        /// index to decrease value of
        /// new value
        public void DecreaseIndex(int index, T obj)
        {
            Assert.IsTrue(index < m_objects.Length && index >= 0,
                           string.Format("IndexedPriorityQueue.DecreaseIndex: Index '{0}' out of range",
                           index));
            Assert.IsTrue(obj.CompareTo(m_objects[index]) <= 0,
                           string.Format("IndexedPriorityQueue.DecreaseIndex: object '{0}' isn't less than current value '{1}'",
                           obj, m_objects[index]));

            m_objects[index] = obj;
            SortUpward(index);
        }

        /// 
        /// Increase the value at the current index
        /// 
        /// index to increase value of
        /// new value
        public void IncreaseIndex(int index, T obj)
        {
            Assert.IsTrue(index < m_objects.Length && index >= 0,
                          string.Format("IndexedPriorityQueue.DecreaseIndex: Index '{0}' out of range",
                          index));
            Assert.IsTrue(obj.CompareTo(m_objects[index]) >= 0,
                           string.Format("IndexedPriorityQueue.DecreaseIndex: object '{0}' isn't greater than current value '{1}'",
                           obj, m_objects[index]));

            m_objects[index] = obj;
            SortDownward(index);
        }

        public void Clear()
        {
            m_count = 0;
        }

        /// 
        /// Set the maximum capacity of the queue
        /// 
        /// new maximum capacity
        public void Resize(int maxSize)
        {
            Assert.IsTrue(maxSize >= 0,
                           string.Format("IndexedPriorityQueue.Resize: Invalid size '{0}'", maxSize));

            m_objects = new T[maxSize];
            m_heap = new int[maxSize + 1];
            m_heapInverse = new int[maxSize];
            m_count = 0;
        }
        #endregion // public

        #region private
        private T[] m_objects;
        private int[] m_heap;
        private int[] m_heapInverse;
        private int m_count;

        private void SortUpward(int index)
        {
            SortHeapUpward(m_heapInverse[index]);
        }

        private void SortDownward(int index)
        {
            SortHeapDownward(m_heapInverse[index]);
        }

        private void SortHeapUpward(int heapIndex)
        {
            // move toward top if better than parent
            while (heapIndex > 1 &&
                    m_objects[m_heap[heapIndex]].CompareTo(m_objects[m_heap[Parent(heapIndex)]]) < 0)
            {
                // swap this node with its parent
                Swap(heapIndex, Parent(heapIndex));

                // reset iterator to be at parents old position
                // (child's new position)
                heapIndex = Parent(heapIndex);
            }
        }

        private void SortHeapDownward(int heapIndex)
        {
            // move node downward if less than children
            while (FirstChild(heapIndex) <= m_count)
            {
                int child = FirstChild(heapIndex);

                // find smallest of two children (if 2 exist)
                if (child < m_count &&
                     m_objects[m_heap[child + 1]].CompareTo(m_objects[m_heap[child]]) < 0)
                {
                    ++child;
                }

                // swap with child if less
                if (m_objects[m_heap[child]].CompareTo(m_objects[m_heap[heapIndex]]) < 0)
                {
                    Swap(child, heapIndex);
                    heapIndex = child;
                }
                // no swap necessary
                else
                {
                    break;
                }
            }
        }

        private void Swap(int i, int j)
        {
            // swap elements in heap
            int temp = m_heap[i];
            m_heap[i] = m_heap[j];
            m_heap[j] = temp;

            // reset inverses
            m_heapInverse[m_heap[i]] = i;
            m_heapInverse[m_heap[j]] = j;
        }

        private int Parent(int heapIndex)
        {
            return (heapIndex / 2);
        }

        private int FirstChild(int heapIndex)
        {
            return (heapIndex * 2);
        }

        private int SecondChild(int heapIndex)
        {
            return (heapIndex * 2 + 1);
        }
        #endregion // private
    }
}