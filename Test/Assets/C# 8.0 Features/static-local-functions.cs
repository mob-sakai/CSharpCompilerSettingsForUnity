/*
# static local functions

Support local functions that disallow capturing state from the enclosing scope.
*/

namespace CSharp_8_Features.StaticLocalFunctions
{
    class Program
    {
        void M(int a)
        {
            // a local function with closure (non-static) -> OK.
            int f(int x) => a * x;

            // a local function without closure (static) -> OK.
            static int g(int x) => x;

            // a local function with closure (static) -> NG.
            //static int h(int x) => a * x;
        }
    }
}
