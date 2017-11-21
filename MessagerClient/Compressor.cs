using System;
using System.Runtime.InteropServices;

namespace MessagerClient
{
    public static class Compressor
    {
        public enum Compression
        {
            HUFFMAN,
            ARITHMETIC,
            SHANNON_FANO
        }
        public enum Encoding
        {
            REPETITION,
            PARITY_CHECK,
            HAMMING
        }

        public static Compression CurrCompression { set; get; }
        public static Encoding CurrEncoding { set; get; }

        public static byte[] Compress(byte[] input)
        {
            unsafe
            {
                fixed (byte* p = input)
                {
                    Arr arr;
                    switch (CurrCompression)
                    {
                        case Compression.ARITHMETIC:
                            arr = ArithmeticEncode((IntPtr) p, (UIntPtr) input.Length);
                            break;
                        case Compression.SHANNON_FANO:
                            arr = SFEncode((IntPtr) p, (UIntPtr) input.Length);
                            break;
                        default:
                            arr = HuffmanEncode((IntPtr)p, (UIntPtr)input.Length);
                            break;
                    }
                    byte[] output = new byte[(int) arr.len];
                    Marshal.Copy(arr.ptr, output, 0, (int) arr.len);
                    return output;
                }
            }
        }

        public static byte[] Encode(byte[] input)
        {
            unsafe
            {
                fixed (byte* p = input)
                {
                    Arr arr;
                    switch (CurrEncoding)
                    {
                        case Encoding.REPETITION:
                            arr = RepetitionEncode((IntPtr)p, (UIntPtr)input.Length);
                            break;
                        case Encoding.PARITY_CHECK:
                            arr = ParityCheckEncode((IntPtr)p, (UIntPtr)input.Length);
                            break;
                        default:
                            arr = HammingEncode((IntPtr)p, (UIntPtr)input.Length);
                            break;
                    }
                    byte[] output = new byte[(int)arr.len];
                    Marshal.Copy(arr.ptr, output, 0, (int)arr.len);
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
                    Arr arr;
                    switch (CurrCompression)
                    {
                        case Compression.ARITHMETIC:
                            arr = ArithmeticDecode((IntPtr) p, (UIntPtr) input.Length);
                            break;
                        case Compression.SHANNON_FANO:
                            arr = SFDecode((IntPtr) p, (UIntPtr) input.Length);
                            break;
                            default: 
                            arr = HuffmanDecode((IntPtr) p, (UIntPtr) input.Length);
                            break;
                    }
                    if (arr.ptr == (IntPtr) 0) return null;
                    byte[] output = new byte[(int)arr.len];
                    Marshal.Copy(arr.ptr, output, 0, (int)arr.len);
                    return output;
                }
            }
        }

        public static byte[] Decode(byte[] input)
        {
            unsafe
            {
                fixed (byte* p = input)
                {
                    Arr arr;
                    switch (CurrEncoding)
                    {
                        case Encoding.REPETITION:
                            arr = RepetitionDecode((IntPtr)p, (UIntPtr)input.Length);
                            break;
                        case Encoding.PARITY_CHECK:
                            arr = ParityCheckDecode((IntPtr)p, (UIntPtr)input.Length);
                            break;
                        default:
                            arr = HammingDecode((IntPtr)p, (UIntPtr)input.Length);
                            break;
                    }
                    if (arr.ptr == (IntPtr)0) return null;
                    byte[] output = new byte[(int)arr.len];
                    Marshal.Copy(arr.ptr, output, 0, (int)arr.len);
                    return output;
                }
            }
        }

        //Huffman
        [DllImport("compressors", EntryPoint="huff_encode", ExactSpelling = true)]
        private static extern Arr HuffmanEncode(IntPtr arr, UIntPtr len);

        [DllImport("compressors", EntryPoint = "huff_decode", ExactSpelling = true)]
        private static extern Arr HuffmanDecode(IntPtr arr, UIntPtr len);
        
        //Arithmetic
        [DllImport("compressors", EntryPoint="ar_encode", ExactSpelling = true)]
        private static extern Arr ArithmeticEncode(IntPtr arr, UIntPtr len);

        [DllImport("compressors", EntryPoint = "ar_decode", ExactSpelling = true)]
        private static extern Arr ArithmeticDecode(IntPtr arr, UIntPtr len);
        
        //Shannon-Fano
        [DllImport("compressors", EntryPoint="sf_encode", ExactSpelling = true)]
        private static extern Arr SFEncode(IntPtr arr, UIntPtr len);

        [DllImport("compressors", EntryPoint = "sf_decode", ExactSpelling = true)]
        private static extern Arr SFDecode(IntPtr arr, UIntPtr len);
        
        //Repetition
        [DllImport("compressors", EntryPoint="rep_encode", ExactSpelling = true)]
        private static extern Arr RepetitionEncode(IntPtr arr, UIntPtr len);

        [DllImport("compressors", EntryPoint = "rep_decode", ExactSpelling = true)]
        private static extern Arr RepetitionDecode(IntPtr arr, UIntPtr len);
        
        //Parity Check
        [DllImport("compressors", EntryPoint="pc_encode", ExactSpelling = true)]
        private static extern Arr ParityCheckEncode(IntPtr arr, UIntPtr len);

        [DllImport("compressors", EntryPoint = "pc_decode", ExactSpelling = true)]
        private static extern Arr ParityCheckDecode(IntPtr arr, UIntPtr len);

        //Hamming
        [DllImport("compressors", EntryPoint="ham_encode", ExactSpelling = true)]
        private static extern Arr HammingEncode(IntPtr arr, UIntPtr len);

        [DllImport("compressors", EntryPoint = "ham_decode", ExactSpelling = true)]
        private static extern Arr HammingDecode(IntPtr arr, UIntPtr len);
    
        [StructLayout(LayoutKind.Sequential)]
        private struct Arr
        {
            public IntPtr ptr;
            public UIntPtr len;
            private readonly UIntPtr cap;
        }
    }
}