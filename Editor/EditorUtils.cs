using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace Coffee.CSharpCompilerSettings
{
    internal static class EditorUtils
    {
        /// <summary>
        /// Copy the file (over write).
        /// NOTE: If there are no changes to MD5, the file will not be copied.
        /// </summary>
        public static void CopyFileIfNeeded(string src, string dst)
        {
            if (IsSameContent(src, dst)) return;

            Directory.CreateDirectory(Path.GetDirectoryName(dst));
            File.Copy(src, dst, true);
        }

        /// <summary>
        /// Returns whether or not the two files are the same.
        /// </summary>
        private static bool IsSameContent(string file1, string file2)
        {
            if (file1 == file2) return true;
            if (string.IsNullOrEmpty(file1) || string.IsNullOrEmpty(file2) || !File.Exists(file1) || !File.Exists(file2)) return false;

            using (var md5 = MD5.Create())
            using (var srcStream = File.OpenRead(file1))
            using (var dstStream = File.OpenRead(file2))
                return md5.ComputeHash(srcStream).SequenceEqual(md5.ComputeHash(dstStream));
        }
    }
}
