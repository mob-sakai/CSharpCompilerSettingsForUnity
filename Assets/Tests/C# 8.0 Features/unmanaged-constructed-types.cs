/*
# unmanaged constructed types

In C# 8.0, we extended the concept of an *unmanaged* type to include constructed (generic) types.
This is a placeholder for its specification.
*/

using NUnit.Framework;

namespace CSharp_8_Features
{
    struct KeyValuePair<T1, T2>
    {
        public T1 Key;
        public T2 Value;

        public KeyValuePair(T1 key, T2 value)
        {
            Key = key;
            Value = value;
        }

        public void SetValue(T2 value) => Value = value;

        public override string ToString() => $"{Key}:{Value}";
    }

    internal partial class Tests
    {
        [Test]
        public static void UnmanagedConstructedTypes()
        {
            var kv = new KeyValuePair<int, int>(1, 2);
            kv.Value = 3;

            var actual = kv.ToString();
            var expected = "1:3";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void UnmanagedConstructedTypes_Pointer_1()
        {
            unsafe
            {
                var kv = new KeyValuePair<int, int>(1, 2);
                var pkv = &kv;
                kv.Value = 3;

                var actual = (*pkv).ToString();
                var expected = "1:3";

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public static void UnmanagedConstructedTypes_Pointer_2()
        {
            unsafe
            {
                var kv = new KeyValuePair<int, int>(1, 2);
                var pkv = &kv;
                (*pkv).Value = 3;

                var actual = kv.ToString();
                var expected = "1:3";

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public static void UnmanagedConstructedTypes_ShallowCopy()
        {
            var kv = new KeyValuePair<int, int>(1, 2);
            var ckv = kv;
            kv.Value = 3;

            var actual = ckv.ToString();
            var expected = "1:2";

            Assert.AreEqual(expected, actual);
        }
    }
}
