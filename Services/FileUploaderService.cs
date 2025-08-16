namespace TypicalTechTools.Services
{
    public class FileUploaderService
    {
        // Private field to store the upload directory path
        string _uploadPath = string.Empty;

        // Dependency: EncryptionService for encrypting/decrypting files
        EncryptionService _encryptionService;

        // Constructor that receives hosting environment and encryption service via dependency injection
        public FileUploaderService(IWebHostEnvironment env, EncryptionService encryptionService)
        {
            // Set the path to the "Uploads" folder inside the wwwroot directory
            _uploadPath = Path.Combine(env.WebRootPath, "Uploads");

            // Store the reference to the encryption service
            _encryptionService = encryptionService;
        }

        // Method to save an uploaded file to the server with encryption
        public void SaveFile(IFormFile file)
        {
            // Get the original file name from the uploaded file
            string fileName = file.FileName;

            // Byte array to hold the raw contents of the file
            byte[] fileContents;

            // Read the uploaded file into memory
            using (var stream = new MemoryStream())
            {
                // Copy the uploaded file to the memory stream
                file.CopyTo(stream);

                // Convert the memory stream to a byte array
                fileContents = stream.ToArray();
            }

            // Encrypt the byte array
            var encryptedFile = _encryptionService.EncryptByteArray(fileContents);

            // Save the encrypted byte array to disk
            using (var dataStream = new MemoryStream(encryptedFile))
            {
                // Build the full path to the target file
                var targetFile = Path.Combine(_uploadPath, fileName);

                // Create a file stream to write the encrypted data
                using (var fileStream = new FileStream(targetFile, FileMode.Create))
                {
                    // Write the encrypted data to the file
                    dataStream.WriteTo(fileStream);
                }
            }
        }

        // Method to locate and return a file info object by file name
        public FileInfo LoadFile(string fileName)
        {
            // Get a directory reference to the upload path
            DirectoryInfo directory = new DirectoryInfo(_uploadPath);

            // Search for a file in the directory that matches the provided name
            var file = directory.EnumerateFiles()
                .Where(f => f.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            // Return null if no file is found, else return the FileInfo object
            if (file == null)
                return null;

            return file;
        }

        // Private helper method to read a file's contents into a byte array
        private byte[] readFileIntoMemory(string fileName)
        {
            // Get the file using LoadFile method
            var file = LoadFile(fileName);

            // If file not found, return null
            if (file == null)
                return null;

            // Read the file into memory and return the byte array
            using (var stream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(file.FullName))
                {
                    // Copy the file content to the memory stream
                    fileStream.CopyTo(stream);

                    // Return the stream as a byte array
                    return stream.ToArray();
                }
            }
        }

        // Method to download and decrypt a file
        public byte[] DownloadFile(string fileName)
        {
            // Read the encrypted file into memory
            var originalFile = readFileIntoMemory(fileName);

            // If the file doesn't exist or is empty, return null
            if (originalFile == null || originalFile.Length == 0)
            {
                return null;
            }

            // Decrypt the file contents
            var decryptedFile = _encryptionService.DecryptByteArray(originalFile);

            // Return the decrypted byte array
            return decryptedFile;
        }
    }
}
