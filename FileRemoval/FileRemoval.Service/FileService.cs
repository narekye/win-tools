using ProcessController.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            if (_computersToBeScanned == null || !_computersToBeScanned.Any())
            {
                _computersToBeScanned = _library.GetCurrentDomainComputers();
            }
        }

        public FileService(FileServiceConfiguration configuration, params string[] computersToBeScanned)
        {
            _computersToBeScanned = computersToBeScanned;
            _configuration = configuration;

            if (_computersToBeScanned == null || !_computersToBeScanned.Any())
            {
                _computersToBeScanned = _library.GetCurrentDomainComputers();
            }
        }

        public List<FileModel> GetLargeFilesFromComputers()
        {
            var result = new List<FileModel>();

            foreach (var computer in _computersToBeScanned)
            {
                var isOnline = _library.Ping(computer, 200);

                if (!isOnline)
                {
                    result.Add(new FileModel { ComputerName = computer, Note = "Offline" });
                    continue;
                }

                var path = string.Format(PathToUsersFolderTemplate, computer);

                IEnumerable<string> usersInComputer = null;

                try
                {
                    usersInComputer = Directory.EnumerateDirectories(path);
                }
                catch (Exception ex)
                {
                    result.Add(new FileModel { Note = ex.Message, Status = StatusEnum.Fail });
                }

                if (usersInComputer == null || !usersInComputer.Any())
                    continue;

                foreach (var user in usersInComputer)
                {
                    DirectoryInfo inf = new DirectoryInfo(user);
                    try
                    {
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
                                    ComputerName = computer,
                                    Name = file.Name,
                                    Extension = file.Extension,
                                    Size = file.Length,
                                    FullName = file.FullName,
                                    ReadableSize = BytesToString(file.Length)
                                });

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // write log and continue;
                        result.Add(new FileModel { Note = ex.Message, ComputerName = computer, Status = StatusEnum.Fail });
                        continue;
                    }
                }
            }

            return result;
        }

        public List<FileModel> GetLargeFilesFromComputers(string username)
        {
            var result = new List<FileModel>();
            foreach (var computer in _computersToBeScanned)
            {
                var isOnline = _library.Ping(computer, 200);

                if (!isOnline)
                {
                    result.Add(new FileModel { ComputerName = computer, Note = "Offline", Status = StatusEnum.Fail });
                    continue;
                }

                var path = string.Format(PathToUsersFolderTemplate, computer);

                var currentUserPath = string.Format("{0}{1}", string.Format(PathToUsersFolderTemplate, computer), username);

                if (!Directory.Exists(currentUserPath))
                {
                    result.Add(new FileModel { ComputerName = computer, Note = "User account not found!", Status = StatusEnum.Fail });
                    continue;
                }

                var directoryInfo = new DirectoryInfo(currentUserPath);

                IEnumerable<FileInfo> filesForCurrentUser = null;

                try
                {
                    filesForCurrentUser = directoryInfo.EnumerateFiles(AllFiles, SearchOption.AllDirectories);

                    ExcludeKnownFiles(ref filesForCurrentUser);

                    if (filesForCurrentUser == null || !filesForCurrentUser.Any())
                        continue;

                    foreach (var file in filesForCurrentUser)
                    {
                        if (file.Length > _configuration.MinimumSize)
                        {
                            if (file.CreationTime <= _configuration.MinimumAge)
                                continue;

                            result.Add(new FileModel
                            {
                                ComputerName = computer,
                                Name = file.Name,
                                Extension = file.Extension,
                                Size = file.Length,
                                FullName = file.FullName,
                                ReadableSize = BytesToString(file.Length),
                                Status = StatusEnum.Success
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // write log and continue
                    result.Add(new FileModel { Note = ex.Message, ComputerName = computer, Status = StatusEnum.Fail });
                    continue;
                }
            }
            return result;
        }

        public List<KeyValuePair<string, StatusEnum>> DeleteLargeFiles()
        {
            var result = new List<KeyValuePair<string, StatusEnum>>();
            var largeFiles = GetLargeFilesFromComputers();

            foreach (var file in largeFiles)
            {
                try
                {
                    // log
                    File.Delete(file.FullName);
                    result.Add(new KeyValuePair<string, StatusEnum>(file.Name, StatusEnum.Success));

                }
                catch (Exception ex)
                {
                    result.Add(new KeyValuePair<string, StatusEnum>(file.Name, StatusEnum.Fail));
                }
            }
            return result;

        }

        public List<KeyValuePair<string, StatusEnum>> DeleteLargeFiles(string username)
        {
            var result = new List<KeyValuePair<string, StatusEnum>>();
            var largeFiles = GetLargeFilesFromComputers(username);

            foreach (var file in largeFiles)
            {
                try
                {
                    File.Delete(file.FullName);
                    result.Add(new KeyValuePair<string, StatusEnum>(file.Name, StatusEnum.Success));

                }
                catch (Exception ex)
                {
                    result.Add(new KeyValuePair<string, StatusEnum>(file.Name, StatusEnum.Fail));
                }
            }
            return result;
        }

        static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        void ExcludeKnownFiles(ref IEnumerable<FileInfo> files)
        {
            files = files.Where(x => !x.Extension.Equals(".lnk") || !x.Extension.Equals(".ini"));
        }
    }
}
