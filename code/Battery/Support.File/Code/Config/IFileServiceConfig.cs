using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Support.File.Code.Config
{
    public interface IFileServiceConfig
    {
        string UploadFileConfig(string targetDir, string filename);
        string UploadFileReturnResult(string filePath, string targetDir, string filename);
        string CropImgReturnResult(string filePath, string targetDir, string filename);
        Tuple<string, string> CropImgConfig(string targetDir, string sourceDir, string filename);
        Tuple<string, string> MoveFile(string targetDir, string sourceDir, string SourceFileName, string SaveFileName);
    }
}