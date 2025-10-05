using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using ABC_Retail.Models;

namespace ABC_Retail.Services
{
    public class FileShareStorageService
    {
        private readonly ShareClient _share;

        public FileShareStorageService(StorageOptions opt)
        {
            _share = new ShareClient(opt.ConnectionString, opt.FileShare);
            _share.CreateIfNotExists();
        }

        public async Task UploadAsync(IFormFile file)
        {
            var directory = _share.GetRootDirectoryClient();
            var shareFile = directory.GetFileClient(file.FileName);

            using var stream = file.OpenReadStream();
            await shareFile.CreateAsync(file.Length);
            await shareFile.UploadAsync(stream);
        }

        public async Task<List<FileItemVm>> ListAsync()
        {
            var directory = _share.GetRootDirectoryClient();
            var files = new List<FileItemVm>();

            await foreach (ShareFileItem item in directory.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                {
                    var fileClient = directory.GetFileClient(item.Name);
                    files.Add(new FileItemVm(item.Name, fileClient.Uri.ToString()));
                }
            }

            return files;
        }
    }
}
