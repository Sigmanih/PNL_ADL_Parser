namespace PNL_ADL_Parser.Utils;

public static class FileHelper
{
    public static string[] ReadFileLines(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file {filePath} does not exist.");

        return File.ReadAllLines(filePath);
    }
}
