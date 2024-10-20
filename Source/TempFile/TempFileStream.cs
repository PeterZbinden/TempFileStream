using Microsoft.Extensions.Logging;


namespace TempFile;

/// <summary>
/// A Stream that creates a Temp-File to temporarily store data.
/// This file is deleted once <see cref="Dispose"/> is called on this Stream.
/// Best to apply the 'using'-Pattern
/// </summary>
public class TempFileStream : Stream
{
    private readonly ILogger _logger;

    private readonly string _tempFilePath;
    private readonly Stream _fileStream;

    /// <summary>
    /// Using rootTemp Path provided by the configuration
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    /// <param name="fileMode"></param>
    /// <param name="fileAccess"></param>
    /// <param name="fileShare"></param>
    public TempFileStream(
        ILogger logger, 
        TempFileStreamConfiguration configuration, 
        FileMode fileMode = FileMode.Create,
        FileAccess fileAccess = FileAccess.ReadWrite,
        FileShare fileShare = FileShare.ReadWrite)
    : this(logger, configuration.RootTempFolder, fileMode, fileAccess, fileShare)
    {
    }

    /// <summary>
    /// Uses the default temp-Path provided by the OS
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="fileMode"></param>
    /// <param name="fileAccess"></param>
    /// <param name="fileShare"></param>
    public TempFileStream(
        ILogger logger, 
        FileMode fileMode = FileMode.Create,
        FileAccess fileAccess = FileAccess.ReadWrite,
        FileShare fileShare = FileShare.ReadWrite)
        : this(logger, Path.Combine(Path.GetTempPath(), "Temp-FileStreams"), fileMode, fileAccess, fileShare)
    {
    }

    /// <summary>
    /// Using the explicit temp folder path
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="tempFolderPath"></param>
    /// <param name="fileMode"></param>
    /// <param name="fileAccess"></param>
    /// <param name="fileShare"></param>
    public TempFileStream(
        ILogger logger, 
        string tempFolderPath, 
        FileMode fileMode = FileMode.Create,
        FileAccess fileAccess = FileAccess.ReadWrite,
        FileShare fileShare = FileShare.ReadWrite)
    {
        _logger = logger;

        // Ensure the folder exists
        var dirInfo = new DirectoryInfo(tempFolderPath);
        if (!dirInfo.Exists)
        {
            dirInfo.Create();
        }

        // Ensure the file does not yet exist
        FileInfo fileInfo;
        do
        {
            _tempFilePath = Path.Combine(tempFolderPath, Guid.NewGuid().ToString());
            fileInfo = new FileInfo(_tempFilePath);
        } while (fileInfo.Exists);

        _fileStream = new FileStream(_tempFilePath, fileMode, fileAccess, fileShare);
    }

    /// <summary>
    /// Creates an instance without a logger
    /// </summary>
    /// <param name="fileMode"></param>
    /// <param name="fileAccess"></param>
    /// <param name="fileShare"></param>
    public TempFileStream(
        FileMode fileMode = FileMode.Create,
        FileAccess fileAccess = FileAccess.ReadWrite,
        FileShare fileShare = FileShare.ReadWrite)
    : this(null, fileMode, fileAccess, fileShare)
    {

    }

    public override void Flush()
    {
        _fileStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _fileStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _fileStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _fileStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _fileStream.Write(buffer, offset, count);
    }

    public override bool CanRead => _fileStream.CanRead;
    public override bool CanSeek => _fileStream.CanSeek;
    public override bool CanWrite => _fileStream.CanWrite;
    public override long Length => _fileStream.Length;
    public override long Position
    {
        get => _fileStream.Position;
        set => _fileStream.Position = value;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        try
        {
            _fileStream.Close();
            File.Delete(_tempFilePath);
        }
        catch (Exception e)
        {
            if (_logger == null)
            {
                throw;
            }

            _logger.LogError(e, $"Temporary File '{_tempFilePath}' could not be removed on {nameof(Dispose)}.");
        }
    }
}