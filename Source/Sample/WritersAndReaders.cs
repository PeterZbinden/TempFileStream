using System.Text;
using TempFile;

namespace Sample;

public class WritersAndReaders
{
    public static async Task WriteAndRead()
    {
        var path = string.Empty;

        await using (var tempFile = new TempFileStream())
        {
            path = tempFile.FilePath;
            var writer = new StreamWriter(tempFile, Encoding.UTF8);
            for (int i = 0; i < 5; i++)
            {
                await writer.WriteLineAsync($"Test {i}");
            }
            
            // Making sure that the data is written to disk and not still in a buffer
            await writer.FlushAsync();

            Console.WriteLine("Data has been written");
            Console.WriteLine($"Temp-File is at: {path}");
            Console.WriteLine("Press ENTER to proceed");
            Console.ReadLine();

            // We need to reset the Position of the Stream before we can read what was written before
            tempFile.Position = 0;

            var reader = new StreamReader(tempFile, Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                Console.WriteLine(await reader.ReadLineAsync());
            }

            Console.WriteLine($"File Exists before exiting using-block: {File.Exists(path)}");
        }
        Console.WriteLine($"File Exists after exiting using-block: {File.Exists(path)}");

        Console.ReadLine();
    }
}