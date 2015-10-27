﻿using System;
using System.Collections;
using System.Collections.Generic;


namespace XYS.Lis.Core
{
    public class ParItemCollection : ICollection, IList, IEnumerable, ICloneable
    {
           #region Interfaces

        /// <summary>
        /// Supports type-safe iteration over a <see cref="ParItemCollection"/>.
        /// </summary>
        public interface IParItemCollectionEnumerator
        {
            /// <summary>
            /// 获取集合当前元素
            /// </summary>
            ParItem Current { get; }

            /// <summary>
            ///移动到下一元素
            /// </summary>
            /// <returns>
            /// <c>true</c> if the enumerator was successfully advanced to the next element; 
            /// <c>false</c> if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            bool MoveNext();

            /// <summary>
            /// Sets the enumerator to its initial position, before the first element in the collection.
            /// </summary>
            void Reset();
        }

        #endregion

        private const int DEFAULT_CAPACITY = 16;

        #region 私有变量 用于实现ParItem 集合

        private ParItem[] m_array;//元素容器
        private int m_count = 0; //元素个数
        private int m_version = 0;

        #endregion

        #region 静态包装
        //创建一个只读实例
        public static ParItemCollection ReadOnly(ParItemCollection list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            return new ReadOnlyParItemCollection(list);
        }
        #endregion

        #region 构造函数
        //默认初始化容器大小
        public ParItemCollection()
        {
            m_array = new ParItem[DEFAULT_CAPACITY];
        }

        public ParItemCollection(int capacity)
        {
            m_array = new ParItem[capacity];
        }

        public ParItemCollection(ParItemCollection c)
        {
            m_array = new ParItem[c.Count];
            AddRange(c);
        }
        /// <summary>
        /// Initializes a new instance of the <c>ParItemCollection</c> class
        /// that contains elements copied from the specified <see cref="ParItem"/> array.
        /// </summary>
        /// <param name="a">The <see cref="ParItem"/> array whose elements are copied to the new list.</param>
        public ParItemCollection(ParItem[] a)
        {
            m_array = new ParItem[a.Length];
            AddRange(a);
        }

        /// <summary>
        /// Initializes a new instance of the <c>ParItemCollection</c> class
        /// that contains elements copied from the specified <see cref="ParItem"/> collection.
        /// </summary>
        /// <param name="col">The <see cref="ParItem"/> collection whose elements are copied to the new list.</param>
        public ParItemCollection(ICollection col)
        {
            m_array = new ParItem[col.Count];
            AddRange(col);
        }

        /// <summary>
        /// Type visible only to our subclasses
        /// Used to access protected constructor
        /// </summary>
        protected internal enum Tag
        {
            /// <summary>
            /// A value
            /// </summary>
            Default
        }

        /// <summary>
        /// Allow subclasses to avoid our default constructors
        /// </summary>
        /// <param name="tag"></param>
        protected internal ParItemCollection(Tag tag)
        {
            m_array = null;
        }
        #endregion

        #region Operations (type-safe ICollection)

        /// <summary>
        /// Gets the number of elements actually contained in the <c>ParItemCollection</c>.
        /// </summary>
        public virtual int Count
        {
            get { return m_count; }
        }

        /// <summary>
        /// Copies the entire <c>ParItemCollection</c> to a one-dimensional
        /// <see cref="ParItem"/> array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="ParItem"/> array to copy to.</param>
        public virtual void CopyTo(ParItem[] array)
        {
            this.CopyTo(array, 0);
        }

        /// <summary>
        /// Copies the entire <c>ParItemCollection</c> to a one-dimensional
        /// <see cref="ParItem"/> array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="ParItem"/> array to copy to.</param>
        /// <param name="start">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public virtual void CopyTo(ParItem[] array, int start)
        {
            if (m_count > array.GetUpperBound(0) + 1 - start)
            {
                throw new System.ArgumentException("Destination array was not long enough.");
            }

            Array.Copy(m_array, 0, array, start, m_count);
        }

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized (thread-safe).
        /// </summary>
        /// <value>true if access to the ICollection is synchronized (thread-safe); otherwise, false.</value>
        public virtual bool IsSynchronized
        {
            get { return m_array.IsSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        public virtual object SyncRoot
        {
            get { return m_array.SyncRoot; }
        }

        #endregion

        #region Operations (type-safe IList)

        /// <summary>
        /// Gets or sets the <see cref="ParItem"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="index"/> is less than zero</para>
        /// <para>-or-</para>
        /// <para><paramref name="index"/> is equal to or greater than <see cref="ParItemCollection.Count"/>.</para>
        /// </exception>
        public virtual ParItem this[int index]
        {
            get
            {
                ValidateIndex(index); // throws
                return m_array[index];
            }
            set
            {
                ValidateIndex(index); // throws
                ++m_version;
                m_array[index] = value;
            }
        }

        /// <summary>
        /// Adds a <see cref="ParItem"/> to the end of the <c>ParItemCollection</c>.
        /// </summary>
        /// <param name="item">The <see cref="ParItem"/> to be added to the end of the <c>ParItemCollection</c>.</param>
        /// <returns>The index at which the value has been added.</returns>
        public virtual int Add(ParItem item)
        {
            if (m_count == m_array.Length)
            {
                EnsureCapacity(m_count + 1);
            }

            m_array[m_count] = item;
            m_version++;

            return m_count++;
        }

        /// <summary>
        /// Removes all elements from the <c>ParItemCollection</c>.
        /// </summary>
        public virtual void Clear()
        {
            ++m_version;
            m_array = new ParItem[DEFAULT_CAPACITY];
            m_count = 0;
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="ParItemCollection"/>.
        /// </summary>
        /// <returns>A new <see cref="ParItemCollection"/> with a shallow copy of the collection data.</returns>
        public virtual object Clone()
        {
            ParItemCollection newCol = new ParItemCollection(m_count);
            Array.Copy(m_array, 0, newCol.m_array, 0, m_count);
            newCol.m_count = m_count;
            newCol.m_version = m_version;

            return newCol;
        }

        /// <summary>
        /// Determines whether a given <see cref="ParItem"/> is in the <c>ParItemCollection</c>.
        /// </summary>
        /// <param name="item">The <see cref="ParItem"/> to check for.</param>
        /// <returns><c>true</c> if <paramref name="item"/> is found in the <c>ParItemCollection</c>; otherwise, <c>false</c>.</returns>
        public virtual bool Contains(ParItem item)
        {
            for (int i = 0; i != m_count; ++i)
            {
                if (m_array[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of a <see cref="ParItem"/>
        /// in the <c>ParItemCollection</c>.
        /// </summary>
        /// <param name="item">The <see cref="ParItem"/> to locate in the <c>ParItemCollection</c>.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> 
        /// in the entire <c>ParItemCollection</c>, if found; otherwise, -1.
        ///	</returns>
        public virtual int IndexOf(ParItem item)
        {
            for (int i = 0; i != m_count; ++i)
            {
                if (m_array[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Inserts an element into the <c>ParItemCollection</c> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The <see cref="ParItem"/> to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="index"/> is less than zero</para>
        /// <para>-or-</para>
        /// <para><paramref name="index"/> is equal to or greater than <see cref="ParItemCollection.Count"/>.</para>
        /// </exception>
        public virtual void Insert(int index, ParItem item)
        {
            ValidateIndex(index, true); // throws

            if (m_count == m_array.Length)
            {
                EnsureCapacity(m_count + 1);
            }

            if (index < m_count)
            {
                Array.Copy(m_array, index, m_array, index + 1, m_count - index);
            }

            m_array[index] = item;
            m_count++;
            m_version++;
        }

        /// <summary>
        /// Removes the first occurrence of a specific <see cref="ParItem"/> from the <c>ParItemCollection</c>.
        /// </summary>
        /// <param name="item">The <see cref="ParItem"/> to remove from the <c>ParItemCollection</c>.</param>
        /// <exception cref="ArgumentException">
        /// The specified <see cref="ParItem"/> was not found in the <c>ParItemCollection</c>.
        /// </exception>
        public virtual void Remove(ParItem item)
        {
            int i = IndexOf(item);
            if (i < 0)
            {
                throw new System.ArgumentException("Cannot remove the specified item because it was not found in the specified Collection.");
            }

            ++m_version;
            RemoveAt(i);
        }

        /// <summary>
        /// Removes the element at the specified index of the <c>ParItemCollection</c>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="index"/> is less than zero</para>
        /// <para>-or-</para>
        /// <para><paramref name="index"/> is equal to or greater than <see cref="ParItemCollection.Count"/>.</para>
        /// </exception>
        public virtual void RemoveAt(int index)
        {
            ValidateIndex(index); // throws

            m_count--;

            if (index < m_count)
            {
                Array.Copy(m_array, index + 1, m_array, index, m_count - index);
            }

            // We can't set the deleted entry equal to null, because it might be a value type.
            // Instead, we'll create an empty single-element array of the right type and copy it 
            // over the entry we want to erase.
            ParItem[] temp = new ParItem[1];
            Array.Copy(temp, 0, m_array, m_count, 1);
            m_version++;
        }

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        /// <value>true if the collection has a fixed size; otherwise, false. The default is false</value>
        public virtual bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the IList is read-only.
        /// </summary>
        /// <value>true if the collection is read-only; otherwise, false. The default is false</value>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Operations (type-safe IEnumerable)

        /// <summary>
        /// Returns an enumerator that can iterate through the <c>ParItemCollection</c>.
        /// </summary>
        /// <returns>An <see cref="Enumerator"/> for the entire <c>ParItemCollection</c>.</returns>
        public virtual IParItemCollectionEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Public helpers (just to mimic some nice features of ArrayList)

        /// <summary>
        /// Gets or sets the number of elements the <c>ParItemCollection</c> can contain.
        /// </summary>
        public virtual int Capacity
        {
            get
            {
                return m_array.Length;
            }
            set
            {
                if (value < m_count)
                {
                    value = m_count;
                }

                if (value != m_array.Length)
                {
                    if (value > 0)
                    {
                        ParItem[] temp = new ParItem[value];
                        Array.Copy(m_array, 0, temp, 0, m_count);
                        m_array = temp;
                    }
                    else
                    {
                        m_array = new ParItem[DEFAULT_CAPACITY];
                    }
                }
            }
        }

        /// <summary>
        /// Adds the elements of another <c>ParItemCollection</c> to the current <c>ParItemCollection</c>.
        /// </summary>
        /// <param name="x">The <c>ParItemCollection</c> whose elements should be added to the end of the current <c>ParItemCollection</c>.</param>
        /// <returns>The new <see cref="ParItemCollection.Count"/> of the <c>ParItemCollection</c>.</returns>
        public virtual int AddRange(ParItemCollection x)
        {
            if (m_count + x.Count >= m_array.Length)
            {
                EnsureCapacity(m_count + x.Count);
            }

            Array.Copy(x.m_array, 0, m_array, m_count, x.Count);
            m_count += x.Count;
            m_version++;

            return m_count;
        }

        /// <summary>
        /// Adds the elements of a <see cref="ParItem"/> array to the current <c>ParItemCollection</c>.
        /// </summary>
        /// <param name="x">The <see cref="ParItem"/> array whose elements should be added to the end of the <c>ParItemCollection</c>.</param>
        /// <returns>The new <see cref="ParItemCollection.Count"/> of the <c>ParItemCollection</c>.</returns>
        public virtual int AddRange(ParItem[] x)
        {
            if (m_count + x.Length >= m_array.Length)
            {
                EnsureCapacity(m_count + x.Length);
            }

            Array.Copy(x, 0, m_array, m_count, x.Length);
            m_count += x.Length;
            m_version++;

            return m_count;
        }

        /// <summary>
        /// Adds the elements of a <see cref="ParItem"/> collection to the current <c>ParItemCollection</c>.
        /// </summary>
        /// <param name="col">The <see cref="ParItem"/> collection whose elements should be added to the end of the <c>ParItemCollection</c>.</param>
        /// <returns>The new <see cref="ParItemCollection.Count"/> of the <c>ParItemCollection</c>.</returns>
        public virtual int AddRange(ICollection col)
        {
            if (m_count + col.Count >= m_array.Length)
            {
                EnsureCapacity(m_count + col.Count);
            }

            foreach (object item in col)
            {
                Add((ParItem)item);
            }

            return m_count;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements.
        /// </summary>
        public virtual void TrimToSize()
        {
            this.Capacity = m_count;
        }

        #endregion

        #region Implementation (helpers)

        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="i"/> is less than zero</para>
        /// <para>-or-</para>
        /// <para><paramref name="i"/> is equal to or greater than <see cref="ParItemCollection.Count"/>.</para>
        /// </exception>
        private void ValidateIndex(int i)
        {
            ValidateIndex(i, false);
        }

        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="i"/> is less than zero</para>
        /// <para>-or-</para>
        /// <para><paramref name="i"/> is equal to or greater than <see cref="ParItemCollection.Count"/>.</para>
        /// </exception>
        private void ValidateIndex(int i, bool allowEqualEnd)
        {
            int max = (allowEqualEnd) ? (m_count) : (m_count - 1);
            if (i < 0 || i > max)
            {
                throw new ArgumentOutOfRangeException("i", (object)i, "Index was out of range. Must be non-negative and less than the size of the collection. [" + (object)i + "] Specified argument was out of the range of valid values.");
            }
        }

        private void EnsureCapacity(int min)
        {
            int newCapacity = ((m_array.Length == 0) ? DEFAULT_CAPACITY : m_array.Length * 2);
            if (newCapacity < min)
            {
                newCapacity = min;
            }

            this.Capacity = newCapacity;
        }

        #endregion

        #region Implementation (ICollection)

        void ICollection.CopyTo(Array array, int start)
        {
            Array.Copy(m_array, 0, array, start, m_count);
        }

        #endregion

        #region Implementation (IList)

        object IList.this[int i]
        {
            get { return (object)this[i]; }
            set { this[i] = (ParItem)value; }
        }

        int IList.Add(object x)
        {
            return this.Add((ParItem)x);
        }

        bool IList.Contains(object x)
        {
            return this.Contains((ParItem)x);
        }

        int IList.IndexOf(object x)
        {
            return this.IndexOf((ParItem)x);
        }

        void IList.Insert(int pos, object x)
        {
            this.Insert(pos, (ParItem)x);
        }

        void IList.Remove(object x)
        {
            this.Remove((ParItem)x);
        }

        void IList.RemoveAt(int pos)
        {
            this.RemoveAt(pos);
        }

        #endregion

        #region Implementation (IEnumerable)

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)(this.GetEnumerator());
        }

        #endregion

        #region Nested enumerator class

        /// <summary>
        /// Supports simple iteration over a <see cref="ParItemCollection"/>.
        /// </summary>
        private sealed class Enumerator : IEnumerator, IParItemCollectionEnumerator
        {
            #region Implementation (data)

            private readonly ParItemCollection m_collection;
            private int m_index;
            private int m_version;

            #endregion

            #region Construction

            /// <summary>
            /// Initializes a new instance of the <c>Enumerator</c> class.
            /// </summary>
            /// <param name="tc"></param>
            internal Enumerator(ParItemCollection tc)
            {
                m_collection = tc;
                m_index = -1;
                m_version = tc.m_version;
            }

            #endregion

            #region Operations (type-safe IEnumerator)

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public ParItem Current
            {
                get { return m_collection[m_index]; }
            }

            /// <summary>
            /// Advances the enumerator to the next element in the collection.
            /// </summary>
            /// <returns>
            /// <c>true</c> if the enumerator was successfully advanced to the next element; 
            /// <c>false</c> if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext()
            {
                if (m_version != m_collection.m_version)
                {
                    throw new System.InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }

                ++m_index;
                return (m_index < m_collection.Count);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                m_index = -1;
            }

            #endregion

            #region Implementation (IEnumerator)

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            #endregion
        }

        #endregion

        #region Nested Read Only Wrapper class

        private sealed class ReadOnlyParItemCollection : ParItemCollection
        {
            #region Implementation (data)

            private readonly ParItemCollection m_collection;

            #endregion

            #region Construction

            internal ReadOnlyParItemCollection(ParItemCollection list)
                : base(Tag.Default)
            {
                m_collection = list;
            }
            #endregion

            #region 类型安全ICollection重写

            public override void CopyTo(ParItem[] array)
            {
                m_collection.CopyTo(array);
            }
            public override void CopyTo(ParItem[] array, int start)
            {
                m_collection.CopyTo(array, start);
            }
            public override int Count
            {
                get { return m_collection.Count; }
            }
            public override bool IsSynchronized
            {
                get { return m_collection.IsSynchronized; }
            }
            public override object SyncRoot
            {
                get { return this.m_collection.SyncRoot; }
            }
            #endregion

            #region 类型安全IList重写

            public override ParItem this[int i]
            {
                get { return m_collection[i]; }
                set { throw new NotSupportedException("This is a Read Only Collection and can not be modified"); }
            }
            public override int Add(ParItem x)
            {
                throw new NotSupportedException("This is a Read Only Collection and can not be modified");
            }
            public override void Clear()
            {
                throw new NotSupportedException("This is a Read Only Collection and can not be modified");
            }
            public override bool Contains(ParItem x)
            {
                return m_collection.Contains(x);
            }
            public override int IndexOf(ParItem x)
            {
                return m_collection.IndexOf(x);
            }
            public override void Insert(int pos, ParItem x)
            {
                throw new NotSupportedException("This is a Read Only Collection and can not be modified");
            }
            public override void Remove(ParItem x)
            {
                throw new NotSupportedException("This is a Read Only Collection and can not be modified");
            }
            public override void RemoveAt(int pos)
            {
                throw new NotSupportedException("This is a Read Only Collection and can not be modified");
            }
            public override bool IsFixedSize
            {
                get { return true; }
            }
            public override bool IsReadOnly
            {
                get { return true; }
            }
            #endregion

            #region 类型安全 IEnumerable重写

            public override IParItemCollectionEnumerator GetEnumerator()
            {
                return m_collection.GetEnumerator();
            }
            #endregion

            #region Public Helpers
            // (just to mimic some nice features of ArrayList)
            public override int Capacity
            {
                get { return m_collection.Capacity; }
                set { throw new NotSupportedException("This is a Read Only Collection and can not be modified"); }
            }
            public override int AddRange(ParItemCollection x)
            {
                throw new NotSupportedException("This is a Read Only Collection and can not be modified");
            }
            public override int AddRange(ParItem[] x)
            {
                throw new NotSupportedException("This is a Read Only Collection and can not be modified");
            }
            #endregion
        }
        #endregion
    }
}
