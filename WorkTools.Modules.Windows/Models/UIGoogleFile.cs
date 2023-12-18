using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace WorkTools.Modules.Windows.Models
{
    public class UIGoogleFile
    {
        private GoogleFile _googleFile = null;

        public string DisplayName => $"({(IsDirectory ? "Directory" : "File")}){GoogleFile.Name}";
        public string Id => GoogleFile.Id;
        public bool IsDirectory => GoogleFile.MimeType == "application/vnd.google-apps.folder";
        public GoogleFile GoogleFile { get => _googleFile; set => _googleFile = value; }

        public UIGoogleFile(GoogleFile googleFile)
        {
            GoogleFile = googleFile;
            if (GoogleFile == null)
                throw new ArgumentNullException(nameof(GoogleFile));
        }
    }
}
