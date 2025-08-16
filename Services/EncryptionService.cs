using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace TypicalTechTools.Services

{
    public class EncryptionService
    {
        // Secret key used for encryption/decryption, loaded from configuration
        string _secretKey;

        // Constructor that retrieves the secret key from the app's configuration
        public EncryptionService(IConfiguration config)
        {
            _secretKey = config["SecretKey"]; // The key must be 16, 24, or 32 bytes for AES-128, AES-192, or AES-256
        }

        // Encrypts a byte array using AES symmetric encryption
        public byte[] EncryptByteArray(byte[] fileData)
        {
            // Create a new AES instance. AES is a symmetric encryption algorithm.
            // When called, it automatically generates a new random IV (Initialization Vector).
            using (var aesAlg = Aes.Create())
            {
                // Convert the secret key string to a byte array using UTF8 encoding
                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(_secretKey);

                // Create an encryptor using the AES key and IV
                // The IV ensures that the same plaintext encrypted multiple times results in different ciphertexts
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create a new memory stream to hold the encrypted data
                using (var memStream = new MemoryStream())
                {
                    // Prepend the IV to the start of the encrypted data (needed for decryption)
                    memStream.Write(aesAlg.IV, 0, 16); // IV is 16 bytes (128 bits)

                    // Create a crypto stream that will encrypt data as it’s written to the memory stream
                    using (var cryStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        // Write the raw data through the crypto stream, which encrypts it and sends it to memStream
                        cryStream.Write(fileData, 0, fileData.Length);

                        // Ensure all data is fully processed and written out
                        cryStream.FlushFinalBlock();

                        // Return the entire memory stream as a byte array (IV + encrypted data)
                        return memStream.ToArray();
                    }
                }
            }
        }

        // Decrypts a previously encrypted byte array (with the IV at the start)
        public byte[] DecryptByteArray(byte[] encryptedData)
        {
            // Create an AES instance for decryption
            using (var aesAlg = Aes.Create())
            {
                // Set the AES key using the same key used for encryption
                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(_secretKey);

                // Extract the IV from the first 16 bytes of the encrypted data
                byte[] IV = new byte[16];
                Array.Copy(encryptedData, IV, IV.Length); // Copy first 16 bytes into IV

                // Create a decryptor using the key and the extracted IV
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, IV);

                // Create a memory stream to hold the decrypted data
                using (var memStream = new MemoryStream())
                {
                    // Create a crypto stream for decryption using the decryptor
                    using (var cryStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Write))
                    {
                        // Write the encrypted portion of the data (excluding IV) to the crypto stream
                        cryStream.Write(encryptedData, IV.Length, encryptedData.Length - IV.Length);

                        // Ensure final decrypted blocks are flushed to the stream
                        cryStream.FlushFinalBlock();

                        // Return the decrypted content as a byte array
                        return memStream.ToArray();
                    }
                }
            }
        }
    }
}
