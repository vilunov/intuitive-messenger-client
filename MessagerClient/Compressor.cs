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
                    Arr arr = HuffmanEncode((IntPtr) p, (UIntPtr) input.Length);
                    byte[] output = new byte[(int) arr.len];
                    Marshal.Copy(arr.ptr, output, 0, (int) arr.len);
                    Free(arr);
                    return output;
                }
            }
        }
        public static byte[] Decompress(byte[] input)
        {
            unsafe
            {
                fixed (byte* p = input)
                {
                    Arr arr = HuffmanDecode((IntPtr)p, (UIntPtr)input.Length);
                    if (arr.ptr == (IntPtr) 0) return null;
                    byte[] output = new byte[(int)arr.len];
                    Marshal.Copy(arr.ptr, output, 0, (int)arr.len);
                    Free(arr);
                    return output;
                }
            }
        }

        [DllImport("compressors", EntryPoint="huff_encode", ExactSpelling = true)]
        private static extern Arr HuffmanEncode(IntPtr arr, UIntPtr len);

        [DllImport("compressors", EntryPoint = "huff_decode", ExactSpelling = true)]
        private static extern Arr HuffmanDecode(IntPtr arr, UIntPtr len);

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