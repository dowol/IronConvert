using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IronConvert;

public class FileItem(FileInfo file) : FileSelection(file)
{
    internal readonly FileInfo fs = file;

    public ImageSource? FileIcon => FileSystemHelper.GetAssociatedIcon(fs);

    public string Type => FileType.RetrieveFileType<FileType>(fs).Name;

    public FileClass Class => FileType.RetrieveFileType<FileType>(fs).Class;

}
