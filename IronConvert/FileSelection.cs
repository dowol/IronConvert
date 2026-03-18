using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IronConvert;

public class FileSelection(FileInfo file)
{
    public FileInfo File { get; set; } = file;

    public string Name
    {
        get => File.Name;
        set => File = new(Path.GetFullPath(value, File.DirectoryName!));
    }
    
    public string NameWithoutExtension
    {
        get => Path.GetFileNameWithoutExtension(Name);
        set => Name = value + Extensions;
    }

    public string FullName
    {
        get => File.FullName;
        set => File = new(value);
    }
    public string FullNameWithoutExtension
    {
        get => FullName[0..FullName.LastIndexOf('.')];
        set => FullName = value + Extensions;
    }

    public string Extensions
    {
        get => File.Extension;
        set => File = new(Path.ChangeExtension(FullName, value));
    }
}
