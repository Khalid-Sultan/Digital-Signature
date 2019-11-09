using System;
using System.Collections.Generic;
using System.Text;

namespace Digital_Signature_Verification
{
    public static class StringHelper
    {
        internal static byte[] convertToByteArray(string text) 
        {
            Encoding ascii = Encoding.UTF8;
            Encoding unicode = Encoding.Unicode;
            byte[] bytesInAscii = ascii.GetBytes(text);
            byte[] bytesInUnicode = Encoding.Convert(ascii, unicode, bytesInAscii);
            return bytesInUnicode; 
        } 
        internal static string convertToString(byte[] byteArray)
        {
            Encoding unicode = Encoding.Unicode;
            char[] charsInUnicode = new char[unicode.GetCharCount(byteArray, 0, byteArray.Length)];
            unicode.GetChars(byteArray, 0, byteArray.Length, charsInUnicode, 0);
            return new string(charsInUnicode); 
        } 

    }
}
