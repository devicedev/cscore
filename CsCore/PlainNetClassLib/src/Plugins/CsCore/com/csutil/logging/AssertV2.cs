using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace com.csutil {
    public static class AssertV2 {


        private static void Assert(bool condition, string errorMsg, object[] args) {
            args = Add(new StackTrace().GetFrame(2), args);
            if (!condition) { Log.e(errorMsg, args); Debugger.Break(); }
        }

        private static object[] Add(StackFrame stackFrame, object[] args) {
            return new object[1] { stackFrame }.Concat(args).ToArray();
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string errorMsg, params object[] args) {
            Assert(condition, "Assert.IsTrue() FAILED: " + errorMsg, args);
        }

        [Conditional("DEBUG")]
        public static void IsFalse(bool condition, string errorMsg, params object[] args) {
            Assert(!condition, "Assert.IsFalse() FAILED: " + errorMsg, args);
        }

        [Conditional("DEBUG")]
        public static void IsNull(object o, string varName, params object[] args) {
            string errorMsg = "Assert.IsNull(" + varName + ") FAILED";
            Assert(o == null, errorMsg, args);
        }

        [Conditional("DEBUG")]
        public static void IsNotNull(object o, string varName, params object[] args) {
            string errorMsg = "Assert.IsNotNull(" + varName + ") FAILED";
            Assert(o != null, errorMsg, args);
        }

        [Conditional("DEBUG")]
        public static void AreEqual<T>(IEquatable<T> expected, IEquatable<T> actual, string varName = "", params object[] args) {
            var errorMsg = "Assert.AreEqual() FAILED: expected " +
                varName + "= " + expected + " NOT equal to actual " + varName + "= " + actual;
            Assert(expected.Equals(actual), errorMsg, args);
        }

        [Conditional("DEBUG")]
        public static void AreNotEqual<T>(IEquatable<T> expected, IEquatable<T> actual, string varName = "", params object[] args) {
            string msg1 = "Assert.AreNotEqual() FAILED: " + varName + " is same reference (expected == actual)";
            Assert(expected != actual, msg1, args);
            var errorMsg = "Assert.AreNotEqual() FAILED: expected " + varName + "= " + expected + " IS equal to actual " + varName + "= " + actual;
            Assert(!expected.Equals(actual), errorMsg, args);
        }

        [Conditional("DEBUG")]
        public static void AreNotEqualLists<T>(IEnumerable<T> expected, IEnumerable<T> actual, string varName = "", params object[] args) {
            string msg1 = "Assert.AreNotEqual() FAILED: " + varName + " is same reference (expected == actual)";
            Assert(expected != actual, msg1, args);
            string msg2 = "Assert.AreNotEqual() FAILED: expected " + varName + "= " + expected + " IS equal to actual " + varName + "= " + actual;
            Assert(!expected.SequenceEqual(actual), msg2, args);
        }

        public static Stopwatch TrackTiming() { return Stopwatch.StartNew(); }

        [Conditional("DEBUG")]
        public static void AssertUnderXms(this Stopwatch self, int maxTimeInMs, params object[] args) {
            var ms = self.ElapsedMilliseconds;
            int p = (int)(ms * 100f / maxTimeInMs);
            var errorText = Log.CallingMethodName() + " took " + p + "% (" + ms + "ms) longer then allowed (" + maxTimeInMs + "ms)!";
            Assert(IsUnderXms(self, maxTimeInMs), errorText, args);
        }

        public static bool IsUnderXms(this Stopwatch self, int maxTimeInMs) { return self.ElapsedMilliseconds <= maxTimeInMs; }

    }
}