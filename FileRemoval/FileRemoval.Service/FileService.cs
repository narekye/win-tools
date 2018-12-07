using ProcessController.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRemoval.Service
{
    public class FileService
    {
        private IEnumerable<string> _computersToBeScanned;
        private const string PathToUsersFolderTemplate = @"\\{0}\C$\Users\";
        private Library _library = new Library();
        private FileServiceConfiguration _configuration;
        private const string AllFiles = "*.*";

        public FileService(FileServiceConfiguration configuration, IEnumerable<string> computersToBeScanned = null)
        {
            _computersToBeScanned = computersToBeScanned;
            _configuration = configuration;
            // get all computers list if the argument is null
        }

        public FileService(FileServiceConfiguration configuration, params string[] computersToBeScanned)
        {
            _computersToBeScanned = computersToBeScanned;
            _configuration = configuration;
        }

        public List<FileModel> GetLargeFilesFromComputers()
        {
            var result = new List<FileModel>();

            foreach (var computer in _computersToBeScanned)
            {
                var isOnline = _library.Ping(computer, 200);

                if (!isOnline)
                    continue;

                var path = string.Format(PathToUsersFolderTemplate, computer);

                var usersInComputer = Directory.EnumerateDirectories(path);

                foreach (var user in usersInComputer)
                {
                    DirectoryInfo inf = new DirectoryInfo(user);
                    var filesForCurrentUser = inf.GetFiles(AllFiles, SearchOption.AllDirectories).AsEnumerable();

                    ExcludeKnownFiles(ref filesForCurrentUser);

                    foreach (var file in filesForCurrentUser)
                    {
                        if (file.Length > _configuration.MinimumSize)
                        {

                            if (file.CreationTime <= _configuration.MinimumAge)
                                continue;

                            result.Add(new FileModel
                            {
                                Name = file.Name,
                                Extension = file.Extension,
                                Size = file.Length
                            });

                        }
                    }
                }
            }

            return result;
        }

        public List<FileModel> GetFilesFromComputersByUser(string username)
        {
            var result = new List<FileModel>();
            foreach (var computer in _computersToBeScanned)
            {
                var isOnline = _library.Ping(computer, 200);

                if (!isOnline)
                {
                    continue;
                }

                var path = string.Format(PathToUsersFolderTemplate, computer);

                var currentUserPath = string.Format("{0}{1}", string.Format(PathToUsersFolderTemplate, computer), username);

                if (!Directory.Exists(currentUserPath))
                {
                    return new List<FileModel>();
                }

                var directoryInfo = new DirectoryInfo(currentUserPath);
                var filesForCurrentUser = directoryInfo.EnumerateFiles(AllFiles, SearchOption.AllDirectories);

                ExcludeKnownFiles(ref filesForCurrentUser);

                foreach (var file in filesForCurrentUser)
                {
                    if (file.Length > _configuration.MinimumSize)
                    {
                        if (file.CreationTime <= _configuration.MinimumAge)
                            continue;

                        result.Add(new FileModel
                        {
                            Name = file.Name,
                            Extension = file.Extension,
                            Size = file.Length
                        });
                    }
                }
            }
            return result;
        }

        private void ExcludeKnownFiles(ref IEnumerable<FileInfo> files)
        {
            files = files.Where(x => !x.Extension.Equals(".lnk") || !x.Extension.Equals(".ini"));
        }
    }
}
