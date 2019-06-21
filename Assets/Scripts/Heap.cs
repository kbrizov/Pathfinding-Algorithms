// Special thanks to my brother Denis.
// My brother's github: https://github.com/dbrizov

namespace System.Collections.Generic
{
    /// <summary>
    /// A binary max heap by default (array implementation). Note that it's NOT stable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heap<T>
    {
        private const int INITIAL_CAPACITY = 4;

        private T[] m_array;
        private int m_lastItemIndex;
        private IComparer<T> m_comparer;

        public Heap()
            : this(INITIAL_CAPACITY, Comparer<T>.Default)
        {
        }

        public Heap(int capacity)
            : this(capacity, Comparer<T>.Default)
        {
        }

        public Heap(Comparison<T> comparison)
            : this(Comparer<T>.Create(comparison))
        {
        }

        public Heap(IComparer<T> comparer)
            : this(INITIAL_CAPACITY, comparer)
        {
        }

        public Heap(int capacity, Comparison<T> comparison)
            : this(capacity, Comparer<T>.Create(comparison))
        {
        }

        public Heap(int capacity, IComparer<T> comparer)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", "Non-negative number required.");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            m_array = new T[capacity];
            m_lastItemIndex = -1;
            m_comparer = comparer;
        }

        public Heap(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {
        }

        public Heap(IEnumerable<T> collection, Comparison<T> comparison)
            : this(collection, Comparer<T>.Create(comparison))
        {
        }

        public Heap(IEnumerable<T> collection, IComparer<T> comparer)
            : this(comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public int Count
        {
            get => (m_lastItemIndex + 1);
        }

        public bool IsEmpty
        {
            get => (Count == 0);
        }

        public void Add(T item)
        {
            if (m_lastItemIndex == m_array.Length - 1)
            {
                Resize();
            }

            m_lastItemIndex++;
            m_array[m_lastItemIndex] = item;

            HeapifyUp(m_lastItemIndex);
        }

        public T Remove()
        {
            if (m_lastItemIndex == -1)
            {
                throw new InvalidOperationException("The heap is empty.");
            }

            T removedItem = m_array[0];
            m_array[0] = m_array[m_lastItemIndex];
            m_lastItemIndex--;

            HeapifyDown(0);

            return removedItem;
        }

        public T Peek()
        {
            if (m_lastItemIndex == -1)
            {
                throw new InvalidOperationException("The heap is empty.");
            }

            return m_array[0];
        }

        public void Clear()
        {
            m_lastItemIndex = -1;
        }

        /// <summary>
        /// Heapify up (max by default).
        /// </summary>
        /// <param name="index">The index of the (sub)root to start from.</param>
        private void HeapifyUp(int index)
        {
            if (index == 0)
            {
                return;
            }

            int childIndex = index;
            int parentIndex = (index - 1) / 2;

            if (m_comparer.Compare(m_array[childIndex], m_array[parentIndex]) > 0)
            {
                // Swap the parent and the child.
                Swap(ref m_array[childIndex], ref m_array[parentIndex]);

                HeapifyUp(parentIndex);
            }
        }

        /// <summary>
        /// Heapify down (max by default).
        /// </summary>
        /// <param name="index">The index of the (sub)root to start from.</param>
        private void HeapifyDown(int index)
        {
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;
            int greaterItemIndex = index;

            if (leftChildIndex <= m_lastItemIndex &&
                m_comparer.Compare(m_array[leftChildIndex], m_array[greaterItemIndex]) > 0)
            {
                greaterItemIndex = leftChildIndex;
            }

            if (rightChildIndex <= m_lastItemIndex &&
                m_comparer.Compare(m_array[rightChildIndex], m_array[greaterItemIndex]) > 0)
            {
                greaterItemIndex = rightChildIndex;
            }

            if (greaterItemIndex != index)
            {
                // Swap the parent with the largest of the child items.
                Swap(ref m_array[index], ref m_array[greaterItemIndex]);

                HeapifyDown(greaterItemIndex);
            }
        }

        private void Resize()
        {
            T[] newArray = new T[m_array.Length * 2];

            for (int i = 0; i < m_array.Length; i++)
            {
                newArray[i] = m_array[i];
            }

            m_array = newArray;
        }

        private void Swap(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}
