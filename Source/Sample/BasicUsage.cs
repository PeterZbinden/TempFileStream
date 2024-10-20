using System.Text;
using TempFile;

namespace Sample;

public class BasicUsage
{
    public static async Task WriteTempFile()
    {
        await using (var tempFile = new TempFileStream())
        {
            await tempFile.WriteAsync(Encoding.UTF8.GetBytes("Test"));

            // Flush forces write to disk
            await tempFile.FlushAsync();

            // At this point, the Temp-File still exists
        }
        // As soon as we leave the using-block, the Temp-File is deleted
    }
}