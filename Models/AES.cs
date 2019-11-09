using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Digital_Signature_Verification
{
    class AES
    {
        // <summary>A symmetric key algorithm used to encrypt/decrypt text data to AES256 standard.
        // using Rijndael's ("Rin-dal") algorithm
        // </summary> 


        // Constants used in our AES256 (Rijndael) Encryption / Decryption        
        const string initVector = "@1B2c3D4e5F6g7H8";
        // Must be 16 bytes        
        const string passPhrase = "Pas5pr@se";
        // Any string        
        const string saltValue = "s@1tValue";
        // Any string        
        const string hashAlgorithm = "SHA1";
        // Can also be "MD5", "SHA1" is stronger        
        const int passwordIterations = 2;
        // Can be any number, usually 1 or 2               
        const int keySize = 256;
        // Allowed values: 192, 128 or 256          
        private string randomKeyText = string.Empty;

        /// <summary>        
        /// /// Convert random key byte array into a ASCII string        
        /// /// </summary>               
        /// /// <param name="bytes">Random key byte array to be converted.</param>            
        /// /// <returns>Random key formatted as a string</returns>               
        public string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        /// <summary>        
        /// /// Convert random key ASCII string into a byte array         
        /// /// </summary>               
        /// /// <param name="randomKeyText">Random key string to be converted.</param>            
        /// /// <returns>Random key formatted as a byte array</returns>               
        public byte[] GetBytes(string randomKeyText)
        {
            byte[] bytes = new byte[randomKeyText.Length * sizeof(char)];
            System.Buffer.BlockCopy(randomKeyText.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>        
        /// /// Get the random key string        
        /// /// </summary>                    
        /// /// <returns>Random key formatted as a string</returns>               
        public string GetRandomKeyText()
        {
            return randomKeyText;
        }

        /// <summary>   
        /// /// Encrypts text using Rijndael symmetric key algorithm and returns base64-encoded result.        
        /// /// </summary>               
        /// /// <param name="plainText">Plain text data to be encrypted.</param>            
        /// /// <returns>Encrypted value formatted as a base64-encoded string.</returns>        
        public string Encrypt(string plainText)
        {
            // Convert init vector / salt value ASCII strings into byte arrays                      
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 encoding            
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            // Convert our plain text into a byte array            
            // Assume plain text can contains UTF8 characters            
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            // Using the specified hash algorithm, create a password from the specified passphrase             
            // and salt value from which we will derive the key            
            // Password creation can be done in one or more iterations            
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                passPhrase,
                saltValueBytes,
                hashAlgorithm,
                passwordIterations);
            // Use  password to generate random bytes for encryption key.            
            // Specify the size of the key in bytes (instead of bits)            
            // Set string equivalent of the random key byte array.            
            byte[] keyBytes = password.GetBytes(keySize / 8);
            randomKeyText = GetString(keyBytes);
            // Create uninitialized Rijndael encryption object.            
            RijndaelManaged symmetricKey = new RijndaelManaged();
            // Set encryption mode to Cipher Block Chaining                        
            symmetricKey.Mode = CipherMode.CBC;
            // Generate encryptor from the key bytes and initialization vector.             
            // Key size will be based on the number of key bytes            
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                keyBytes, initVectorBytes
                );
            // Define memory stream to contain encrypted data            
            MemoryStream memoryStream = new MemoryStream();
            // Define cryptographic stream (always use Write mode for encryption)            
            CryptoStream cryptoStream = new CryptoStream(
                memoryStream,
                encryptor,
                CryptoStreamMode.Write);
            // Start and finish encrypting.            
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            // Convert encrypted data from a memory stream into a byte array            
            byte[] cipherTextBytes = memoryStream.ToArray();
            // Close memory and cryptographic streams            
            memoryStream.Close();
            cryptoStream.Close();
            // Convert encrypted data into a base64-encoded string and return             
            return Convert.ToBase64String(cipherTextBytes);
        }

        /// <summary>        
        /// /// Decrypts text using Rijndael symmetric key algorithm.        
        /// /// </summary>        
        /// /// <param name="cipherText">Base64-formatted text value.</param>     
        /// /// <param name="keyBytes">The public key in byte array format</param>           
        /// /// <param name="initVector">Vector required to encrypt 1st block of text data, exactly 16 bytes long</param>          
        /// /// <returns>Decrypted UTF8-encoded string value</returns>               
        public string Decrypt(string cipherText, string randomKeyText)
        {
            // Convert strings defining encryption key characteristics into byte            
            // arrays. Let us assume that strings only contain ASCII codes.            
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8            
            // encoding.            
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            // Convert our ciphertext into a byte array.            
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            // Create uninitialized Rijndael encryption object.            
            RijndaelManaged symmetricKey = new RijndaelManaged();
            // It is reasonable to set encryption mode to Cipher Block Chaining           
            // (CBC). Use default options for other symmetric key parameters.         
            symmetricKey.Mode = CipherMode.CBC;
            // Generate decryptor from existing key bytes and initialization vector.         
            // Key size will be defined based on the number of the key bytes            
            byte[] keyBytes = GetBytes(randomKeyText);
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                keyBytes,
                initVectorBytes);
            // Define memory stream which will be used to hold encrypted data       
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            // Define cryptographic stream (always use Read mode for encryption)         
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                decryptor,
                CryptoStreamMode.Read);
            // We don't know what the size of decrypted data will be, so allocate buffer    
            // long enough to hold ciphertext plaintext is never longer than ciphertext  
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            // Start decrypting.            
            int decryptedByteCount = cryptoStream.Read(
                plainTextBytes,
                0,
                plainTextBytes.Length);
            // Close both streams.      
            memoryStream.Close();
            cryptoStream.Close();
            // Convert decrypted data into a string.      
            // Let us assume that the original plaintext string was UTF8-encoded.       
            string plainText = Encoding.UTF8.GetString(
                plainTextBytes,
                0,
                decryptedByteCount);
            // Return decrypted string           
            return plainText;
        }
    }

}
