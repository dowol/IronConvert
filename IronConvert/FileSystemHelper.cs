using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IronConvert;

public static partial class FileSystemHelper
{
    /// <summary>
    /// Expands any recognized special folder symbols in the specified path to their corresponding absolute paths.
    /// </summary>
    /// <remarks>Special folder symbols should match the names of values in the Environment.SpecialFolder
    /// enumeration. This method is case-insensitive when matching symbols.</remarks>
    /// <param name="path">The file system path that may contain special folder symbols to expand. Leading and trailing whitespace is
    /// ignored.</param>
    /// <returns>A string with all recognized special folder symbols replaced by their absolute paths. Unrecognized symbols are
    /// left unchanged.</returns>
    public static string ExpandSpecialFolder(string path)
    {
        return re_sdsym().Replace(path.Trim(), m => 
            Enum.TryParse(m.Groups["symbol"].Value, true, out Environment.SpecialFolder result)
            ? Environment.GetFolderPath(result)
            : m.Value);
        }

    /// <summary>
    /// Gets an <see cref="ImageSource"/> of file type icons associated on the current system.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    public static ImageSource? GetAssociatedIcon(FileSystemInfo fs)
    {
        return Icon.ExtractAssociatedIcon(fs.FullName) is Icon icon
        ? Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
        : null;
    }

    /// <summary>
    /// Regular expressions for special directory symbol
    /// </summary>
    [GeneratedRegex(@"^\$(?<symbol>\w+)[\/]?")]
    private static partial Regex re_sdsym();
}
