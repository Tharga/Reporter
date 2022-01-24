namespace Tharga.Reporter.Console;

public static class FileManager
{
    public static void ExecuteFile(byte[] byteArray)
    {
        var fileName = $"{Path.GetTempFileName()}.pdf";
        File.WriteAllBytes(fileName, byteArray);
        System.Console.WriteLine($"Created: {fileName}");
        //Process.Start(fileName);

        //System.Threading.Thread.Sleep(5000);

        //while (System.IO.File.Exists(fileName))
        //{
        //    try
        //    {
        //        System.IO.File.Delete(fileName);
        //    }
        //    catch (System.IO.IOException)
        //    {
        //        System.Console.WriteLine("Waiting for the document to close before it can be deleted...");
        //        System.Threading.Thread.Sleep(5000);
        //    }
        //}
    }
}