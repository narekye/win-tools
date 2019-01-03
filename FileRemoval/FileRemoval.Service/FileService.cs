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
                    var model = new FileModel { ComputerName = computer, Note = "Offline", Status = StatusEnum.Fail };
                    LogMessage(model);
                    result.Add(model);
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
                    var model = new FileModel { Note = ex.Message, Status = StatusEnum.Fail };
                    LogMessage(model);
                    result.Add(model);
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
                                if (file.CreationTime <= _configuration.MinimumAge && _configuration.MinimumSize != default(long))
                                    continue;

                                var newResult = new FileModel
                                {
                                    ComputerName = computer,
                                    Name = file.Name,
                                    Extension = file.Extension,
                                    Size = file.Length,
                                    FullName = file.FullName,
                                    ReadableSize = BytesToString(file.Length),
                                    Status = StatusEnum.Success,
                                    Username = user,
                                    CreatedDate = file.CreationTime
                                };

                                LogMessage(newResult);

                                result.Add(newResult);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var newResult = new FileModel { Note = ex.Message, ComputerName = computer, Status = StatusEnum.Fail };
                        LogMessage(newResult);
                        result.Add(newResult);
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
                    var model = new FileModel { ComputerName = computer, Note = "Offline", Status = StatusEnum.Fail };
                    LogMessage(model);
                    result.Add(model);
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
                            if (file.CreationTime <= _configuration.MinimumAge && _configuration.MinimumSize != default(long))
                                continue;

                            var newResult = new FileModel
                            {
                                ComputerName = computer,
                                Name = file.Name,
                                Extension = file.Extension,
                                Size = file.Length,
                                FullName = file.FullName,
                                ReadableSize = BytesToString(file.Length),
                                Status = StatusEnum.Success,
                                Username = username,
                                CreatedDate = file.CreationTime
                            };

                            LogMessage(newResult);

                            result.Add(newResult);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var newResult = new FileModel { Note = ex.Message, ComputerName = computer, Status = StatusEnum.Fail };
                    LogMessage(newResult);
                    result.Add(newResult);
                    continue;
                }
            }
            return result;
        }

        public List<KeyValuePair<string, StatusEnum>> DeleteLargeFiles()
        {
            var result = new List<KeyValuePair<string, StatusEnum>>();
            var largeFiles = GetLargeFilesFromComputers();

            foreach (var file in largeFiles.Where(x => x.Status != StatusEnum.Fail))
            {
                try
                {
                    File.Delete(file.FullName);
                    var newResult = new KeyValuePair<string, StatusEnum>(file.Name, StatusEnum.Success);
                    result.Add(newResult);

                }
                catch (Exception ex)
                {
                    var model = new KeyValuePair<string, StatusEnum>(file.Name, StatusEnum.Fail);
                    result.Add(model);
                    LogMessage(model, ex);
                }
            }
            return result;

        }

        public List<KeyValuePair<string, StatusEnum>> DeleteLargeFiles(string username)
        {
            var result = new List<KeyValuePair<string, StatusEnum>>();
            var largeFiles = GetLargeFilesFromComputers(username);

            foreach (var file in largeFiles.Where(x => x.Status != StatusEnum.Fail))
            {
                try
                {
                    File.Delete(file.FullName);
                    result.Add(new KeyValuePair<string, StatusEnum>(file.FullName, StatusEnum.Success));

                }
                catch (Exception ex)
                {
                    var model = new KeyValuePair<string, StatusEnum>(file.FullName, StatusEnum.Fail);
                    LogMessage(model, ex);
                    result.Add(model);
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

        void LogMessage(KeyValuePair<string, StatusEnum> keyValue, Exception ex)
        {
            if (_configuration.ServiceLogger == null)
                return;

            FileModel fileModel = new FileModel
            {
                FullName = keyValue.Key,
                Status = keyValue.Value,
                Note = ex.Message
            };

            switch (_configuration.ServiceLogger.LogBehavior)
            {
                case LogBehaviorEnum.LogEverything:
                    LogException();
                    LogInfo();
                    break;
                case LogBehaviorEnum.LogOnlyExceptions:
                    LogException();
                    break;
                case LogBehaviorEnum.LogOnlyInformation:
                    LogInfo();
                    break;
            }

            void LogException()
            {
                if (fileModel.Status == StatusEnum.Fail)
                    _configuration.ServiceLogger.Error(fileModel);
            }

            void LogInfo()
            {
                if (fileModel.Status == StatusEnum.Success)
                    _configuration.ServiceLogger.Info(fileModel);
            }
        }

        void LogMessage(FileModel file)
        {
            if (_configuration.ServiceLogger == null)
                return;

            switch (_configuration.ServiceLogger.LogBehavior)
            {
                case LogBehaviorEnum.LogEverything:
                    LogException();
                    LogInfo();
                    break;
                case LogBehaviorEnum.LogOnlyExceptions:
                    LogException();
                    break;
                case LogBehaviorEnum.LogOnlyInformation:
                    LogInfo();
                    break;
            }

            void LogException()
            {
                if (file.Status == StatusEnum.Fail)
                    _configuration.ServiceLogger.Error(file);
            }

            void LogInfo()
            {
                if (file.Status == StatusEnum.Success)
                    _configuration.ServiceLogger.Info(file);
            }
        }
    }
}
