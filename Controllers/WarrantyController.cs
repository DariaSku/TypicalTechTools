using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using TypicalTechTools.Services;

namespace TypicalTechTools.Controllers
{
    /// <summary>
    /// Handles warranty file upload, download, and deletion.
    /// </summary>
    /// <remarks>
    /// - Any logged-in user can upload a completed warranty form.
    /// - Only ADMIN users can view and delete uploaded files.
    /// - A static blank warranty form is always available for download (not encrypted).
    /// </remarks>
    public class WarrantyController : Controller
    {
        private IWebHostEnvironment Environment;
        private readonly FileUploaderService _fileUploader;


        public WarrantyController(IWebHostEnvironment _environment, FileUploaderService fileUploader)
        {
            Environment = _environment;
            _fileUploader = fileUploader;
        }

        /// <summary>
        /// Displays the warranty index page with file upload option and admin file list.
        /// </summary>
        /// <returns>Warranty view with available file data.</returns>
        public IActionResult Index()
        {
            //Retrieve a list of the currently uploaded files and put it in the Viewbag to be passed to the view when opened.
            ViewBag.FileList = GetUploadFileList();
            return View();
        }

        /// <summary>
        /// Handles file upload from authenticated users and stores encrypted file.
        /// </summary>
        /// <param name="file">Uploaded file from user</param>
        /// <returns>Redirects to Index page with updated file list or error.</returns>
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.WarrantyErrorMessage = "No File Selected";
                ViewBag.FileList = GetUploadFileList();
                return View(nameof(Index));
            }

            string fileName = GenerateUniqueFileName(file.FileName);
            var renamedFile = new FormFile(file.OpenReadStream(), 0, file.Length, file.Name, fileName);
            _fileUploader.SaveFile(renamedFile);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Downloads a static warranty claim form (not encrypted).
        /// </summary>
        /// <returns>Downloadable .docx file.</returns>
        public IActionResult DownloadClaimForm()
        {
            //Get the file path of the warranty form.
            string filePath = Path.Combine(this.Environment.WebRootPath, "Forms\\TypicalTools_WarrantyForm.docx");

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", Path.GetFileName(filePath));
        }

        /// <summary>
        /// Downloads an uploaded file (decrypted).
        /// </summary>
        /// <param name="fileName">Name of the file to download</param>
        /// <returns>Decrypted file as byte stream.</returns>
        public IActionResult DownloadFile(string fileName) 
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return RedirectToAction(nameof(Index));

            byte[] fileData = _fileUploader.DownloadFile(fileName);

            if (fileData == null)
                return RedirectToAction(nameof(Index));

            return File(fileData, "application/octet-stream", fileName);
        }

        /// <summary>
        /// Displays confirmation page before deleting file (admin only).
        /// </summary>
        /// <param name="filePath">Full path to the file</param>
        /// <returns>View with file details for deletion confirmation.</returns>
        public IActionResult Delete(string filePath) 
        {
            //Creates an anonymous object list where each entry holds 2 values, the filename and the filepath.
            ViewBag.File = new { Name = Path.GetFileName(filePath), Path = filePath };    
            return View();
        }

        /// <summary>
        /// Deletes file from server (admin only).
        /// </summary>
        /// <param name="id">Unused ID field</param>
        /// <param name="filePath">Path to the file to delete</param>
        /// <returns>Redirects to Index page after deletion.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, string filePath)
        {
            //Try catch to handl if the file delete has any issues
            try
            {
                //Tell the system to delete the file based upon the provided filepath.
                System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Gets a list of all uploaded files stored in wwwroot/Uploads.
        /// </summary>
        /// <returns>Enumerable list of file details.</returns>
        private IEnumerable<UploadedFileDetails> GetUploadFileList()
        {
            //Retrieves all the current files located in the Uploads folder of the wwwroot directory.
            string[] filePaths = Directory.GetFiles(Path.Combine(this.Environment.WebRootPath, "Uploads\\"));
            //Creates an list of UploadFileDetails objects to hold the filename and the filepath of each file in the folder.
            var files = filePaths.Select(file => new UploadedFileDetails{ Name = Path.GetFileName(file), Path = file });
            //Return the list to the caller.
            return files;
        }

        /// <summary>
        /// Generates a unique file name by appending a number if a duplicate exists.
        /// </summary>
        /// <param name="fileName">Original uploaded file name</param>
        /// <returns>Modified file name ensuring uniqueness.</returns>
        private string GenerateUniqueFileName(string fileName)
        {
            //Takes the statring file name and separates the name and extension sections.
            string startingName = fileName.Split('.')[0];
            string fileExt = fileName.Split('.')[1];
            //Sets the initial updatedFileName to be the same as the starting name. This will be changed if this name
            //is not found to be unique.
            string updatedFileName = startingName;

            //Retrieves all the current files located in the Uploads folder of the wwwroot directory.
            string[] filePaths = Directory.GetFiles(Path.Combine(this.Environment.WebRootPath, "Uploads\\"));
            //Sets a counter to be used for the numeric modifier for the file name if needed.
            int counter = 1;

            //Checks if any current files in the folder match the file name, if a match is found the loop will run to
            //modify the name and try again.
            while(filePaths.Any(file => Path.GetFileName(file).Split('.')[0].Equals(updatedFileName)))
            {
                //Change the updated name to be the starting name plus a numeric modifier based upon the counter value
                updatedFileName = $"{startingName}({counter})";
                //Increase the counter for if it needs to be higher in the next loop.
                counter++;
            }
            
            //Return the updated file name after it is found to be unique and add the extension back to the name.
            return $"{updatedFileName}.{fileExt}";
        }
    }

    public class UploadedFileDetails
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
