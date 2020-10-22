/*
# obsolete on property accessor

In C# 8.0, we added support for declaring a property accessor `[Obsolete]`.
This is a placeholder for the specification.
*/

using System;
using NUnit.Framework;
using UnityEngine;

namespace CSharp_8_Features
{
    class Point3
    {
        public int X
        {
            get;
            [Obsolete("setter is obsolete (warning)", false)]
            set;
        }

        public int Y
        {
            get;
            [Obsolete("setter is obsolete (error)", true)]
            set;
        }
    }

    internal partial class Tests
    {
        [Test]
        public static void ObsoleteOnPropertyAccessor_Test()
        {
            var a = new Point3();
            a.X = 1; // warning
            //a.Y = 1; // error
            Debug.Log(a.X);
            Debug.Log(a.Y);
        }
    }
}
