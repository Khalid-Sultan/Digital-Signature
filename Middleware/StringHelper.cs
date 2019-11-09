using System;
using System.Collections.Generic;
using System.Text;

namespace Digital_Signature_Verification
{
    public static class StringHelper
    {
        internal static byte[] convertToByteArray(string text) 
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            byte[] bytesInUnicode = unicode.GetBytes(text);
            byte[] bytesInAscii = Encoding.Convert(unicode, ascii, bytesInUnicode);
            return bytesInAscii;
        }
        internal static string convertToString(byte[] byteArray)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            char[] charsInAscii = new char[ascii.GetCharCount(byteArray, 0, byteArray.Length)];
            ascii.GetChars(byteArray, 0, byteArray.Length, charsInAscii, 0);
            return new string(charsInAscii);
        }

    }
}
