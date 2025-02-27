using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ChummerDataViewer.Model
{
    public sealed class CrashReport
    {
        public EventHandler ProgressChanged { get; set; }

        private readonly Database.DatabasePrivateApi _database;
        private readonly string _strKey;
        private readonly string _strDownloadedZip;
        private readonly string _strFolderlocation;

        public bool IsDownloadStarted => !string.IsNullOrEmpty(_strDownloadedZip);
        public bool IsUnpackStarted => !string.IsNullOrEmpty(_strFolderlocation);

        public Guid Guid { get; }

        public DateTime Timestamp { get; }
        public Version Version { get; }
        public string BuildType { get; }
        public string ErrorFrindly { get; }
        public string WebFileLocation { get; set; }
        public string StackTrace { get; set; }
        public string Userstory { get; set; }

        public CrashReportProcessingProgress Progress
        {
            get => _progress;
            private set
            {
                _progress = value;
                ProgressChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        internal CrashReport(Database.DatabasePrivateApi database, Guid guid, long unixTimeStamp, string buildType, string errorFrindly, string strKey,
            string webFileLocation, Version version, string strDownloadedZip = null, string strFolderlocation = null, string stackTrace = null, string userstory = null)
        {
            _database = database;
            _strKey = strKey;
            _strDownloadedZip = strDownloadedZip;
            _strFolderlocation = strFolderlocation;
            Guid = guid;
            BuildType = buildType;
            ErrorFrindly = errorFrindly;
            WebFileLocation = webFileLocation;
            Version = version;
            StackTrace = stackTrace;
            Userstory = userstory;
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            //Addseconds complains, 1 second = 10000 ns ticks so do that instead
            Timestamp = unixStart.AddTicks(unixTimeStamp * 10000);

            if (strFolderlocation != null)
            {
                Progress = CrashReportProcessingProgress.Unpacked;
            }
            else if (strDownloadedZip != null)
            {
                Progress = CrashReportProcessingProgress.Downloaded;
            }
            else
            {
                Progress = CrashReportProcessingProgress.NotStarted;
            }
        }

        private DownloaderWorker _worker;
        private CrashReportProcessingProgress _progress;

        internal void StartDownload(DownloaderWorker worker)
        {
            if (Uri.TryCreate(WebFileLocation, UriKind.RelativeOrAbsolute, out Uri uriLocation))
            {
                if (Progress != CrashReportProcessingProgress.NotStarted)
                {
                    throw new InvalidOperationException();
                }

                Progress = CrashReportProcessingProgress.Downloading;

                string file = PersistentState.Database.GetKey("crashdumps_zip_folder") + Path.DirectorySeparatorChar + Guid + ".zip";
                _worker = worker;
                _worker.Enqueue(Guid, uriLocation, _strKey, file);

                _worker.StatusChanged += WorkerOnStatusChanged;
            }
            else
                throw new InvalidOperationException();
        }

        private void WorkerOnStatusChanged(INotifyThreadStatus sender, StatusChangedEventArgs args)
        {
            if (args.AttachedData.guid != Guid)
                return;

            _worker.StatusChanged -= WorkerOnStatusChanged;

            _database.SetZipFileLocation(Guid, args.AttachedData.destinationPath);

            WebFileLocation = args.AttachedData.destinationPath;

            string userstory = null;
            string exception = null;
            using (ZipArchive archive = new ZipArchive(File.OpenRead(args.AttachedData.destinationPath), ZipArchiveMode.Read, false))
            {
                ZipArchiveEntry userstoryEntry = archive.GetEntry("userstory.txt");
                if (userstoryEntry != null)
                {
                    using (Stream s = userstoryEntry.Open())
                    {
                        byte[] buffer = new byte[userstoryEntry.Length];
                        s.Read(buffer, 0, buffer.Length);
                        userstory = Encoding.UTF8.GetString(buffer);
                    }
                }

                ZipArchiveEntry exceptionEntry = archive.GetEntry("exception.txt");
                if (exceptionEntry != null)
                {
                    Stream s = exceptionEntry.Open();
                    byte[] buffer = new byte[exceptionEntry.Length];
                    s.Read(buffer, 0, buffer.Length);
                    exception = Encoding.UTF8.GetString(buffer);
                    s.Close();
                }
            }
            Userstory = userstory;
            StackTrace = exception;

            _database.SetStackTrace(Guid, exception);
            if (userstory != null) _database.SetUserStory(Guid, userstory);

            Progress = CrashReportProcessingProgress.Downloaded;
        }
    }

    public enum CrashReportProcessingProgress
    {
        NotStarted,
        Downloading,
        Downloaded,
        Unpacking,
        Unpacked
    }
}
