using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DropboxIntegrationSample
{
    public class DropboxHandler
    {
        DropboxClient dc = new DropboxClient(Constants.DropboxToken);
        public async Task Authenticate()
        {
            var full = await dc.Users.GetCurrentAccountAsync();
        }

        public async Task Upload(string folder, string file, byte[] content)
        {
            using (var mem = new MemoryStream(content))
            {
                var updated = await dc.Files.UploadAsync(
                    folder + "/" + file,
                    WriteMode.Overwrite.Instance,
                    body: mem);
                string v = updated.Rev;
            }
        }

        public async Task Upload(string folder, string file, string content)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var updated = await dc.Files.UploadAsync(
                    folder + "/" + file,
                    WriteMode.Overwrite.Instance,
                    body: mem);
                string v = updated.Rev;
            }
        }

        public async Task CreateFolder(string folderPath)
        {
            CreateFolderArg cfa = new CreateFolderArg(folderPath);
            var updated = await dc.Files.CreateFolderAsync(cfa);
        }

        public async Task<Stream> Download(string folder, string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                var response = await dc.Files.DownloadAsync(folder + "/" + file);
                return await response.GetContentAsStreamAsync().ConfigureAwait(false);
            }
            else
            {
                var response = await dc.Files.DownloadAsync(folder);
                return await response.GetContentAsStreamAsync().ConfigureAwait(false);
            }
            
        }

        async Task ListRootFolder(DropboxClient dbx)
        {
            var list = await dbx.Files.ListFolderAsync(string.Empty);

            // show folders then files
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                Console.WriteLine("D  {0}/", item.Name);
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            }
        }
    }
}
