using System.Collections;

namespace CADExtract
{
    public abstract class Collection<T> : IList<T>
    {
        private List<T> items = new List<T>();

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= items.Count)
                {
                    throw new ArgumentOutOfRangeException();


                }
                else
                {
                    return items[index];
                }
            }

            set
            {
                if (index < 0 || index >= items.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    items[index] = value;
                }

            }
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            items.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
