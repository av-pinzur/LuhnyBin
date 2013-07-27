﻿using System;
using System.Collections.Generic;
using System.Linq;
using AvP.Joy;
using AvP.Joy.Enumerables;
using AvP.Joy.Sequences;

namespace AvP.LuhnyBin.CSharpFnStyle
{
    public static class LuhnUtility
    {
        private static readonly IReadOnlyList<int> validCardNumberLengths = new List<int> { 16, 15, 14 };
        private static readonly IReadOnlyList<char> cardNumberSpacers = new List<char> { ' ', '-' };

        private static bool IsCardNumberChar(char value)
        {
            return value.IsDigit() || cardNumberSpacers.Contains(value);
        }

        public static ISequence<char> FilterCardNumbers(ISequence<char> source, char mask)
        {
            return F<ISequence<char>>.YEval(source, 0,
                self => (src, maskCount) => src.None() ? src 
                    : F.Let(maskCount.MaxVs(MaxCardNumberLength(src)),
                        newMaskCount => new LazySequence<char>(
                            newMaskCount > 0 && src.Head.IsDigit() ? mask : src.Head, 
                            () => self(src.GetTail(), newMaskCount - 1) ) ) );
        }

        private static int MaxCardNumberLength(ISequence<char> chars)
        {
            var consecutiveIndexedDigits = chars
                .TakeWhile(IsCardNumberChar)
                .Index()
                .Where(p => p.Value.IsDigit());

            var luhnLength = validCardNumberLengths.FirstOrDefault(
                len => IsLuhnAtLength(len, consecutiveIndexedDigits.Unindex() ));

            return luhnLength == 0 ? 0 
                : consecutiveIndexedDigits.Nth(luhnLength - 1).Index + 1;
        }

        public static bool IsLuhnAtLength(int length, ISequence<char> digits)
        {
            var list = digits.Take(length).ToList();
            return list.Count() == length && IsLuhn(list);
        }

        public static bool IsLuhn(IEnumerable<char> digits)
        {
            return IsLuhn(digits.Select(c => c.ToDigit()));
        }

        public static bool IsLuhn(IEnumerable<int> digits)
        {
            return digits.Reverse()
                .SelectMany((digit, i) => i.IsEven() 
                    ? digit.InSingleton() 
                    : (digit * 2).Digits() )
                .Sum()
                .DividesBy(10);
        }
    }
}