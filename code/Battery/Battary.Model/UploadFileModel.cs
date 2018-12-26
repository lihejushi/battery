using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model
{
    public class File
    {
        public string fileName { get; set; }
        public int fileret { get; set; }//成功与否 1成功 0失败
        public string message { get; set; }//成功或失败原因
    }
    public class UploadFileModel
    {
        public int files { get; set; }//总文件数
        public int success { get; set; }//成功数
        public List<File> filelist { get; set; }
    }
}
