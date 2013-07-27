using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AvP.LuhnyBin.CSharpFnStyle;
using AvP.Joy;
using AvP.Joy.Sequences;
using System.Diagnostics;

namespace AvP.LuhnyBin.Test
{
    [TestClass]
    public class CSharpFnStyleTest
    {
        [TestMethod]
        public void TestIsLuhn()
        {
            Assert.IsTrue(LuhnUtility.IsLuhn("0"));
            Assert.IsTrue(LuhnUtility.IsLuhn("42"));
            Assert.IsTrue(LuhnUtility.IsLuhn("4111111111111111"));
        }

        [TestMethod]
        public void TestFilteringCardNumbers()
        {
            Assert.AreEqual("XYZ4111111111111113ABC", LuhnUtility.FilterCardNumbers("XYZ4111111111111113ABC".AsSequence(), 'X').ToStrings().Join(""));
            Assert.AreEqual("XXXXXXXXXXXXXXXXABC", LuhnUtility.FilterCardNumbers("4111111111111111ABC".AsSequence(), 'X').ToStrings().Join(""));
            Assert.AreEqual("XYZXXXXXXXXXXXXXXXX", LuhnUtility.FilterCardNumbers("XYZ4111111111111111".AsSequence(), 'X').ToStrings().Join(""));
            Assert.AreEqual("XYZXXXX XXXX XXXX XXXXABC", LuhnUtility.FilterCardNumbers("XYZ4111 1111 1111 1111ABC".AsSequence(), 'X').ToStrings().Join(""));
            Assert.AreEqual("XYZXXXXXX-XXX-XXX-XXXXXABC", LuhnUtility.FilterCardNumbers("XYZ411111-111-111-11110ABC".AsSequence(), 'X').ToStrings().Join(""));
        }

        [TestMethod]
        public void TestProgram()
        {
            var output = PipeIO(
                "00000000000000000000000000000\n4111111111111111111111111111111111114111111111111111111111111111",
                typeof(Program).Assembly.Location, "");
            Assert.AreEqual("XXXXXXXXXXXXXXXXXXXXXXXXXXXXX\nXXXXXXXXXXXXXXXX111111XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX111111111111", output);
        }

        private static string PipeIO(string standardInput, string commandFile, string arguments)
        {
            string standardOutput;
            string standardError;
            using (Process p = new Process())
            {
                p.StartInfo.FileName = commandFile;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.Start();

                p.StandardInput.Write(standardInput);
                p.StandardInput.Close();
                standardOutput = p.StandardOutput.ReadToEnd();
                standardError = p.StandardError.ReadToEnd();
                p.WaitForExit();
            }
            if (standardError != string.Empty)
                throw new Exception("Process reported an error.\n  Standard Error:\n" + standardError + "\n  Standard Out:\n" + standardOutput);
            return standardOutput;
        }
    }
}