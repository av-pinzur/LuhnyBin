using System;
using System.IO;
using AvP.Joy;
using AvP.Joy.Sequences;

namespace AvP.LuhnyBin.CSharpFnStyle
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.Out.WriteChars(Console.In.ReadingChars().ViaSequence(
                chars => LuhnUtility.FilterCardNumbers(chars, 'X')));
        }
    }
}