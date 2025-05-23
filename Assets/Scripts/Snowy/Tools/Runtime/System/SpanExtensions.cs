﻿using System.Collections.Generic;
using Snowy.CSharp;

namespace System
{
    public static class SpanExtensions
    {
#if !UNITY_2021_2_OR_NEWER
        public static void CopyTo<T>(this Span<T> self, Span<T> destination) where T : unmanaged, IEquatable<T>
        {
            if (self.Length > destination.Length)
                throw new ArgumentException("Destination too short.");

            for (int i = 0; i < self.Length; i++)
            {
                destination[i] = self[i];
            }
        }

        public static bool TryCopyTo<T>(this Span<T> self, Span<T> destination) where T : unmanaged, IEquatable<T>
        {
            if ((uint)self.Length <= (uint)destination.Length)
            {
                for (int i = 0; i < self.Length; i++)
                {
                    destination[i] = self[i];
                }
                return true;
            }
            return false;
        }

        public static int IndexOf<T>(this Span<T> self, T item) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (item.Equals(self[i]))
                    return i;
            }

            return -1;
        }

        public static void Reverse<T>(this Span<T> self) where T : unmanaged
        {
            self.Reverse(0, self.Length);
        }
#endif

        public static void Sort<T>(this Span<T> self) where T : unmanaged, IComparable<T>
        {
            SortUtility.Sort(self, 0, self.Length - 1, Comparer<T>.Default);
        }

        public static void Sort<T>(this Span<T> self, Comparison<T> comparison) where T : unmanaged
        {
            SortUtility.Sort(self, 0, self.Length - 1, new SortUtility.DefaultComparer<T> { Comparison = comparison });
        }

        public static void Sort<T, TComparer>(this Span<T> self, TComparer comparer)
            where T : unmanaged
            where TComparer : IComparer<T>
        {
            SortUtility.Sort(self, 0, self.Length - 1, comparer);
        }

        public static bool Contains<T>(this Span<T> self, T item) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (item.Equals(self[i]))
                    return true;
            }

            return false;
        }
    }
}
