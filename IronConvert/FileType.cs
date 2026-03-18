using FFmpeg.NET;
using ImageMagick;
using Pandoc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace IronConvert;

public enum FileClass : byte { None, Document, Image, Multimedia }

public abstract class FileType(FileClass fileClass, string name, string[] extensions)
{
    private static readonly List<FileType> registry = [];

    public static IEnumerable<string> SupportedExtensions => registry.SelectMany(f => f.Extensions);

    public FileClass Class { get; } = fileClass;
    public string Name { get; } = name;
    public string[] Extensions { get; } = extensions;

    public IEnumerable<FileType> ConvertibleTypes => registry.Where(item => Class == item.Class && !Name.Equals(item.Name));

    public static void Convert(FileInfo input, FileInfo output)
    {
        switch (RetrieveFileType<FileType>(input).Class)
        {
            case FileClass.Document:
                ConvertDocumentFile(input, output); return;
            case FileClass.Multimedia:
                ConvertMultimediaFile(input, output); return;
            case FileClass.Image:
                ConvertImageFile(input, output); return;
        }
    }

    private static void Register(FileType ft)
    {
        registry.Add(ft);
    }

    internal static TFileType RetrieveFileType<TFileType>(FileInfo file) where TFileType : FileType
    {
        return (TFileType)registry.Single(item => item.Extensions.Contains(file.Extension.ToLowerInvariant()));
    }

    private static void ConvertDocumentFile(FileInfo input, FileInfo output)
    {
        var (inputType, outputType) = (RetrieveFileType<DocumentFileType>(input), RetrieveFileType<DocumentFileType>(output));

        ProcessStartInfo psi = new("pandoc.exe")
        {
            CreateNoWindow = true
        };
        string[] args = ["--from", inputType.PandocType + "+styles", "--to", outputType.PandocType, "--output", output.FullName, input.FullName];
        foreach (string arg in args)
            psi.ArgumentList.Add(arg);

        Process? process = Process.Start(psi);
        if (process is not null)
        {
            process.WaitForExit();
            MessageBox.Show("File conversion completed!", "IronConvert", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        else if (MessageBox.Show("To convert between document files, 'Pandoc' program is required.\nDo you want to download and install Pandoc now?",
                "IronConvert Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
        {
            process = Process.Start("start", "https://pandoc.org/installing.html");
            process.WaitForExit();
        }
        process?.Dispose();
    }

    private static async void ConvertMultimediaFile(FileInfo input, FileInfo output)
    {
        InputFile inputFile = new(input);
        OutputFile outputFile = new(output);

        Engine ffmpeg = new();
        await ffmpeg.ConvertAsync(inputFile, outputFile, CancellationToken.None);
    }

    private static void ConvertImageFile(FileInfo input, FileInfo output)
    {
        using FileStream inputStream = input.OpenRead();
        using FileStream outputStream = output.OpenWrite();

        ImageFileType outputType = RetrieveFileType<ImageFileType>(output);

        using MagickImage image = new(inputStream);
        image.Format = outputType.Format;
        image.Write(outputStream);
    }

    static FileType()
    {
        Register(new DocumentFileType("MS Word", [".docx"], "docx"));
        Register(new DocumentFileType("HTML", [".html", ".htm"], "html"));
        Register(new DocumentFileType("Markdown", [".md", ".markdown"], "markdown_github"));
        Register(new DocumentFileType("reStructuredText", [".rst", ".rest"], "rst"));
        Register(new DocumentFileType("RTF", [".rtf"], "rtf"));
        Register(new DocumentFileType("EPUB", [".epub"], "epub"));
        Register(new DocumentFileType("LaTeX", [".tex", ".latex"], "latex"));
        Register(new DocumentFileType("MediaWiki", [".wiki"], "mediawiki"));

        Register(new ImageFileType("JPEG", [".jpg", ".jpeg"], MagickFormat.Jpeg));
        Register(new ImageFileType("PNG", [".png"], MagickFormat.Png));
        Register(new ImageFileType("BitMap", [".bmp"], MagickFormat.Bmp3));
        Register(new ImageFileType("GIF", [".gif"], MagickFormat.Gif));
        Register(new ImageFileType("WebP", [".webp"], MagickFormat.WebP));
        Register(new ImageFileType("SVG", [".svg"], MagickFormat.Svg));
        Register(new ImageFileType("Animated PNG", [".apng"], MagickFormat.APng));
        Register(new ImageFileType("Adobe Illustrator", [".ai"], MagickFormat.Ai));
        Register(new ImageFileType("Adobe Photoshop", [".psd"], MagickFormat.Psd));

        Register(new MultimediaFileType("WAV", [".wav"]));
        Register(new MultimediaFileType("MP3", [".mp3"]));
        Register(new MultimediaFileType("FLAC", [".flac"]));
        Register(new MultimediaFileType("OGG", [".ogg"]));
        Register(new MultimediaFileType("Opus", [".opus"]));
    }
}

public class DocumentFileType : FileType
{
    internal DocumentFileType(string name, string[] extensions, string pandocType) : base(FileClass.Document, name, extensions)
    {
        PandocType = pandocType;
    }



    public string PandocType { get; }



}

public class ImageFileType : FileType
{
    public MagickFormat Format { get; }

    internal ImageFileType(string name, string[] extensions, MagickFormat format) : base(FileClass.Image, name, extensions)
    {
        Format = format;
    }
}

public class MultimediaFileType : FileType
{
    internal MultimediaFileType(string name, string[] extensions) : base(FileClass.Multimedia, name, extensions)
    {
    }
}