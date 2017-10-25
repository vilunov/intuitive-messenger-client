using System;
using System.Runtime.InteropServices;

namespace MessagerClient
{
    public static class Compressor
    {
        public static byte[] Compress(byte[] input)
        {
            unsafe
            {
                fixed (byte* p = input)
                {
                    Arr arr = BasicRaw((IntPtr) p, (UIntPtr) input.Length);
                    byte[] output = new byte[(int) arr.len];
                    Marshal.Copy(arr.ptr, output, 0, (int) arr.len);
                    Free(arr);
                    return output;
                }
            }
        }
    
        [DllImport("compressors", EntryPoint="basic", ExactSpelling = true)]
        private static extern Arr BasicRaw(IntPtr arr, UIntPtr len);
        
        [DllImport("compressors", EntryPoint="drop", ExactSpelling = true)]
        private static extern void Free(Arr arr);
    
        [StructLayout(LayoutKind.Sequential)]
        private struct Arr
        {
            public IntPtr ptr;
            public UIntPtr len;
            private readonly UIntPtr cap;
        }
    }
}