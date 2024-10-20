[NuGet.org](https://www.nuget.org/packages/TempFile)

# TempFile
A Library that helps the User to deal with short-lived Streams that are too large to just keep in memory.
An Example of such a usecase could be:
Your logic needs to download a file, perform some operations on it and upload it somewhere else.

'TempFile' helps by providing a Stream to a Temp-File that you wan write to/read from and gets deleted as soon as it is disposed.

### Add Namespace
```csharp
using TempFile;
```

### Basic Usage
Writing to a 'TempFileStream' is very similar to a normal Stream.
To delete of the File, either use a using-block or manually dispose the Stream.
```csharp
await using (var tempFile = new TempFileStream())
{
    await tempFile.WriteAsync(Encoding.UTF8.GetBytes("Test"));
    
    // Flush forces write to disk
    await tempFile.FlushAsync();
    
    // At this point, the Temp-File still exists
}
// As soon as we leave the using-block, the Temp-File is deleted
```

### Using StreamWriter and StreamReader
When working with the 'TempFile' you normally also want to read from it.
'TempFileStream' acts like any normal Stream, where you can reset the Position.
```csharp
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
```