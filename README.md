# Dropbox Integration

## Install nuget package
Search for the *Dropbox.Api* on https://www.nuget.org and install to your project using "Manage NuGet Packages..." window or run following command in your package manager console.
> Install-Package Dropbox.Api

## Add following key to appSettings in Web.Config

    <add key="DropboxUploadRootFolder" value="/Share/myuploadedfile" />
    <add key="DropboxToken" value="XXXXXXXXXXXXXXXX" />
> How to obtain dropbox token? Follow this link: [http://99rabbits.com/get-dropbox-access-token/](http://99rabbits.com/get-dropbox-access-token/) 

## Add helper class
> Refer: DropBoxHandler.cs

## Create following functions in your controller 
Copy and paste below functions to your controller wherever you wanted to use:

### Upload

     [HttpPost]
     public async Task UploadFiles(IEnumerable files)
     {
            foreach (HttpPostedFileBase file in files)
            {
                try
                {
                    string newFileName = file.FileName; //Change the file name if you need to
                    string filePath = ConfigurationManager.AppSettings["DropboxUploadRootFolder"];
                    await new DropboxHandler().Upload(filePath,newFileName, ReadData(file.InputStream));
                    
                }
                catch (IOException ioex) {
                    //TODO: Log your exception
                }
                catch (Exception ex) {
                    //TODO: Log your exception
                }
            }
    }

        /// <summary>
        /// Read the file data
        /// </summary>
        /// <param name="stream">Memory Buffer</param>
        /// <returns>Byte array</returns>
        private byte[] ReadData(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
### Download

	/// <summary>
        /// Download individual files with their original name
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        [HttpGet]
        public ActionResult Download(string fileName)
        {
           var task = Task<Stream>.Run(() => DownloadFile(ConfigurationManager.AppSettings["DropboxUploadRootFolder"], fileName));
                task.Wait();
                
                if (task.IsCompleted)
                {
                    return File(task.Result, "application/x-download", fileName);
                }         
            return null;
        }
        /// <summary>
        /// Get the stream data from dropbox
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        async Task<Stream> DownloadFile(string folder, string fileName)
        {
            return await new DropboxHandler().Download(folder, fileName).ConfigureAwait(false);
        }
> Good idea to move ReadData and DownloadFile functions to helper, so that can be used anywhere in your project.

