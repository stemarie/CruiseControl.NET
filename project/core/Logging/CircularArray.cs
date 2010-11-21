using System;
using System.Collections;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Logging
{
    /// <summary>
    /// 	
    /// </summary>
	public class CircularArray : IEnumerable
	{
		private const int AssumedAverageLineLength = 80;
		private static readonly EnumeratorDirection DefaultDirection = EnumeratorDirection.Forward;
		private int currentIndex = 0;
		private object[] items;
		private bool isFull;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularArray" /> class.	
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <remarks></remarks>
		public CircularArray(int capacity)
		{
			this.items = new object[capacity];
		}

        /// <summary>
        /// Adds the specified item.	
        /// </summary>
        /// <param name="item">The item.</param>
        /// <remarks></remarks>
		public void Add(object item)
		{
			items[currentIndex] = item;
			currentIndex = IncrementIndex(currentIndex);
		}

        /// <summary>
        /// Gets the <see cref="object" /> at the specified index.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public object this[int index]
		{
			get { return items[index]; }
		}

		private int IncrementIndex(int index)
		{
			int nextIndex = (index + 1)%items.Length;
			if (nextIndex == 0) isFull = true;
			return nextIndex;
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			return ToString(DefaultDirection);
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string ToString(EnumeratorDirection direction)
		{
			StringBuilder builder = new StringBuilder(items.GetUpperBound(0)*AssumedAverageLineLength);
			IEnumerator enumerator = new CircularArrayEnumerator(this, direction);
			while (enumerator.MoveNext())
			{
				if (builder.Length > 0) builder.Append(Environment.NewLine);
				builder.Append(enumerator.Current);
			}
			return builder.ToString();
		}

        /// <summary>
        /// Gets the enumerator.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public IEnumerator GetEnumerator()
		{
			return new CircularArrayEnumerator(this, DefaultDirection);
		}
	
		internal class CircularArrayEnumerator : IEnumerator
		{
			private const int InitialIndex = -1;
			private readonly CircularArray array;
			private readonly EnumeratorDirection direction;
			private int index = InitialIndex;

			public CircularArrayEnumerator(CircularArray array, EnumeratorDirection direction)
			{
				this.array = array;
				this.direction = direction;
			}

			public bool MoveNext()
			{
				if (array.currentIndex == 0 && ! array.isFull) return false;	// array is empty
				if (index == InitialIndex)
				{
					index = StartIndex();
					return true;
				}

				if (direction == EnumeratorDirection.Backward)
				{
					index = Decrement(index);
					return index != StartIndex();					
				}
				else
				{
					index = Increment(index);
					return index != StartIndex();										
				}
			}

			private int StartIndex()
			{
				return (direction == EnumeratorDirection.Backward) ? Decrement(array.currentIndex) : Increment(array.currentIndex - 1);
			}

			private int Increment(int index)
			{
				if (! array.isFull && index == array.currentIndex - 1) return 0;
				return (index + 1) % array.items.Length;
			}

			private int Decrement(int currentIndex)
			{
				if (! array.isFull && currentIndex == 0) return array.currentIndex - 1;
				return (currentIndex - 1 + array.items.Length) % array.items.Length;
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public object Current
			{
				get { return array[index]; }
			}
		}
	}
}