using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace IronConvert;

/// <summary>
/// ViewModel (DataContext in WPF Window) class for <see cref="MainWindow"/>
/// </summary>
public class MainDataContext : INotifyPropertyChanged
{

    internal static Configuration Config => new();


    public event PropertyChangedEventHandler? PropertyChanged;

    private DirectoryInfo directory = new(Config.GetString("RecentDirectory", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));

    private FileItem? source;
    private FileSelection? destination;
    private FileType? destinationType;
    private int listBoxIndex = 0;
    private int destinationTypeIndex = 0;
    private bool openWhenConverted = Config.GetBoolean("OpenWhenConverted", true);
    private bool overwrite = false;

    public DirectoryInfo Directory
    {
        get => directory;
        set
        {
            if(directory.FullName != value.FullName)
            {
                directory = value;
                Config.Set("RecentDirectory", value.FullName);
                PropertyChanged?.Invoke(this, new(nameof(Directory)));
                PropertyChanged?.Invoke(this, new(nameof(Items)));
            }
        }
    }

    public FileItem? SourceFile
    {
        get => source; 
        set
        {
            source = value;
            PropertyChanged?.Invoke(this, new(nameof(SourceFile)));
            PropertyChanged?.Invoke(this, new(nameof(ConvertibleTypes)));
        }
    }

    public FileSelection? DestinationFile
    {
        get => destination;
        set
        {
            destination = value;
            PropertyChanged?.Invoke(this, new(nameof(DestinationFile)));
        }
    }

    public FileType? DestinationType
    {
        get => destinationType;
        set
        {
            destinationType = value;
            PropertyChanged?.Invoke(this, new(nameof(DestinationType)));
        }
    }

    public int ListBoxIndex
    {
        get => listBoxIndex;
        set
        {
            listBoxIndex = value;
            PropertyChanged?.Invoke(this, new(nameof(ListBoxIndex)));
        }
    }

    public int DestinationTypeIndex 
    { 
        get => destinationTypeIndex; 
        set
        {
            destinationTypeIndex = value;
            PropertyChanged?.Invoke(this, new(nameof(DestinationTypeIndex)));
        }
    }


    //public bool KeepSourceName
    //{
    //    get => keepSourceName;
    //    set
    //    {
    //        keepSourceName = value;
    //        Config.Set("KeepSourceName", value);
    //        PropertyChanged?.Invoke(this, new(nameof(KeepSourceName)));
    //    }
    //}

    public bool OpenWhenConverted
    {
        get => openWhenConverted;
        set
        {
            openWhenConverted = value;
            Config.Set("OpenWhenConverted", value);
            PropertyChanged?.Invoke(this, new(nameof(OpenWhenConverted)));
        }
    }

    public bool Overwrite
    {
        get => overwrite;
        set
        {
            overwrite = value;
            PropertyChanged?.Invoke(this, new(nameof(Overwrite)));
        }
    }
    public IEnumerable<FileType> ConvertibleTypes => SourceFile is not null ? FileType.RetrieveFileType<FileType>(SourceFile!.File).ConvertibleTypes : [];

    public IEnumerable<FileItem> Items => Directory.EnumerateFiles()
        .Where(f => FileType.SupportedExtensions.Contains(f.Extension) && !f.Attributes.HasFlag(FileAttributes.Hidden))
        .Select(f => new FileItem(f));



    #region Commands
    public RelayCommand ChangeDirectory => new(ChangeDirectoryCommand);

    public RelayCommand Convert => new(ConvertCommand);

    #endregion

    #region Command Implementations
    private void ChangeDirectoryCommand(object? parameter)
    {
        try
        {
            if (parameter is DirectoryInfo dir)
                Directory = dir;

            else if (parameter is string path)
            {
                path = FileSystemHelper.ExpandSpecialFolder(Environment.ExpandEnvironmentVariables(path));

                // Resolve to the absolute path if the relative path was given
                 if (!Path.IsPathRooted(path))
                    path = Path.GetFullPath(path, Directory.FullName);

                Directory = new(path);
            }
            else 
            {
                OpenFolderDialog ofd = new()
                {
                    DefaultDirectory = Directory.FullName,
                    Title = "Select a Folder",
                    ValidateNames = true,
                };

                if (ofd.ShowDialog() == true)
                {
                    Directory = new(ofd.FolderName);
                }
            }
            ListBoxIndex = 0;
        }
        catch
        {

        }
    }

    private void ConvertCommand(object? parameter)
    {
        if(SourceFile is null)
        {
            MessageBox.Show("You must select a file first to convert",
                "IronConvert Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if(DestinationFile is null)
        {
            MessageBox.Show($"You must specify the file type to convert from ${SourceFile.Name}",
                "IronConvert Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if (!Overwrite && DestinationFile.File.Exists)
        {
            MessageBox.Show($"""
                You have already converted {SourceFile.Name} to {DestinationType?.Name ?? DestinationFile.Name}.
                If you want to reconvert modified version of {SourceFile.Name}, Check 'Overwrite conversion content...' or delete previous converted file before converting.
                """, "IronConvert Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            FileType.Convert(SourceFile.File, DestinationFile.File);

            if (openWhenConverted)
            {
                using Process? open = Process.Start(new ProcessStartInfo(DestinationFile.FullName)
                {
                    UseShellExecute = true,
                    ErrorDialog = true,
                });
                open?.WaitForExit();
            }
                
        }
    }



    #endregion
}
