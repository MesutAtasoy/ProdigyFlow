namespace ProdigyFlow.AI.Services;

public class FileService
{
    public FileService()
    {
    }

    public async Task WriteFileAsync(string fileName, string content)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        await File.WriteAllLinesAsync(filePath, new List<string> { content });
    }
    
    public async Task WriteFileAsync(string fileName, List<string> content)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        await File.WriteAllLinesAsync(filePath, content);
    }
    
}