﻿using System;
using Snowy.Tools;
using UnityEngine;

namespace Snowy.NumericEntities
{
#if UNITY
    [Serializable]
#endif
    public struct AccumInt : IAccumEntity<int>, IEquatable<AccumInt>
    {
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private int _got;
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private int _spent;

        public int Value => _got - _spent;
        public bool IsEmpty => _got == _spent;
        public int Got => _got;
        public int Spent => _spent;

#if UNITY_EDITOR
        internal static string GotFieldName => nameof(_got);
#endif

        public AccumInt(int got, int spent)
        {
            if (spent > got)
                throw ThrowErrors.MinMax(nameof(spent), nameof(got));

            _got = got;
            _spent = spent;
        }

        public void Add(int addValue)
        {
            if (addValue < 0)
                throw ThrowErrors.NegativeParameter(nameof(addValue));

            checked { _got += addValue; }
        }

        public bool Spend(int spendValue)
        {
            if (spendValue < 0)
                throw ThrowErrors.NegativeParameter(nameof(spendValue));

            if (spendValue <= Value)
            {
                checked { _spent += spendValue; }
                return true;
            }

            return false;
        }

        public void Merge(AccumInt other)
        {
            _got = Math.Max(_got, other._got);
            _spent = Math.Max(_spent, other._spent);
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is AccumInt accumInt && Equals(accumInt);
        }

        public bool Equals(AccumInt other)
        {
            return other._got == _got && other._spent == _spent;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_got, _spent);
        }

        public static implicit operator int(AccumInt entity)
        {
            return entity.Value;
        }

        // -- //

        public static bool operator ==(AccumInt a, AccumInt b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(AccumInt a, AccumInt b)
        {
            return !a.Equals(b);
        }

        public static AccumInt operator +(AccumInt value1, int value2)
        {
            if (value2 < 0)
                throw ThrowErrors.NegativeParameter(nameof(value2));

            checked { value1._got += value2; }
            return value1;
        }

        public static AccumInt operator -(AccumInt value1, int value2)
        {
            if (value2 < 0)
                throw ThrowErrors.NegativeParameter(nameof(value2));

            checked { value1._spent += value2; }
            return value1;
        }
    }
}
