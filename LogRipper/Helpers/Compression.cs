using System.IO;

using ICSharpCode.SharpZipLib.Zip;

using LogRipper.Exceptions;
using LogRipper.Models;

namespace LogRipper.Helpers;

internal static class Compression
{
    internal static bool CompressData(MemoryStream data, string destination, int bufferSize, bool forceReplace, IProgressionPackUnpack progress = null)
    {
        FileStream fileStreamOut;
        ZipOutputStream zipOutStream;

        if (data == null || data.Length == 0)
            throw new LogRipperException("No data to compress");

        data.Seek(0, SeekOrigin.Begin);
        data.Position = 0;
        if (File.Exists(destination))
            if (forceReplace)
                File.Delete(destination);
            else
                return false;

        fileStreamOut = File.Create(destination);
        int size;
        byte[] buffer = new byte[bufferSize];
        zipOutStream = new ZipOutputStream(fileStreamOut);
        ZipEntry entry = new(Path.GetFileNameWithoutExtension(destination) + ".state");
        zipOutStream.SetLevel(9);
        zipOutStream.PutNextEntry(entry);
        do
        {
            size = data.Read(buffer, 0, buffer.Length);
            zipOutStream.Write(buffer, 0, size);
            progress?.SetProgress(data.Position, zipOutStream.Length);
        } while (size > 0);
        zipOutStream.Flush();
        zipOutStream.Close();

        return true;
    }

    internal static bool DecompressData(string cheminSource, out MemoryStream data, int bufferSize, IProgressionPackUnpack progress = null)
    {
        FileStream fileStreamIn;
        ZipInputStream zipInStream;

        if (!File.Exists(cheminSource))
        {
            data = null;
            return false;
        }

        data = new MemoryStream();

        int size;
        fileStreamIn = new FileStream(cheminSource, FileMode.Open, FileAccess.Read);
        zipInStream = new ZipInputStream(fileStreamIn);
        zipInStream.GetNextEntry();
        byte[] buffer = new byte[bufferSize];
        do
        {
            size = zipInStream.Read(buffer, 0, buffer.Length);
            data.Write(buffer, 0, size);
            progress?.SetProgress(zipInStream.Length, data.Position);
        } while (size > 0);
        fileStreamIn.Close();
        zipInStream.Close();
        data.Flush();
        data.Seek(0, SeekOrigin.Begin);
        data.Position = 0;

        return true;
    }
}
