# IRONCONVERT

**IronConvert** is a .NET based file format converter by simple controlling.

## SUPPORTED FILE FORMATS

### Documents (requires Pandoc)
- MS Word (`.docx`)
- ePub (`.epub`, `.epub2`, `.epub3`)
- HTML (`.html`, `.htm`)
- LaTeX (`.tex`, `.latex`)
- MediaWiki (`.wiki`)
- GitHub-Flavored Markdown (`.md`, `.markdown`)
- OpenDocument Text (`.odt`) 
- reStructuredText (`.rst`, `.rest`)
- Rich Text Format (`.rtf`)

### Images
- Adobe Illustrator (`.ai`)
- MS BitMap (`.bmp`)
- GIF (`.gif`)
- JPEG (`.jpg`, `.jpeg`)
- PNG (`.png`)
- Adobe PhotoShop (`.psd`)
- SVG (`.svg`)
- WebP (`.webp`)

### Audio files (requires FFmpeg)
- WAV (`.wav`)
- AIFF (`.aiff`)
- MP3 (`.mp3`)
- FLAC (`.flac`)
- OGG (`.ogg`, `.oga`)
- Opus (`.opus`)

## Dependencies
- [FluentIcons.Wpf](https://github.com/davidxuang/FluentIcons) - UI Icons
- [Magick.NET](https://github.com/dlemstra/Magick.NET) - Format converting for Image files
- [Pandoc.NET](https://github.com/SimonCropp/PandocNet) - Format converting for Document files
- [xFFmpeg.NET](https://github.com/cmxl/FFmpeg.NET) - Format converting for Audio files

## License
**IronConvert** is distributed under the GNU General Public License v2.  
You may use program, read and modify its source code, or redistribute for free.  
For more details, see [License file](LICENSE.txt).