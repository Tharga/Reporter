using System;
using System.Drawing.Printing;
using System.Threading.Tasks;

namespace Tharga.Reporter.Engine
{
    public class Printing
    {
        //C:\Dev\Tharga\Florida\Dev\Main\Florida\Tharga.Florida.Client\bin\Debug\Resources
        //private Printing(string defaultPrinterName = null, string adobeReaderPath = @"Resources\AcroRd32.exe")
        private Printing(string defaultPrinterName = null, string adobeReaderPath = @"C:\Program Files (x86)\Adobe\Reader 10.0\Reader\AcroRd32.exe")
        {
            var settings = new PrinterSettings();
            PdfSharp.Pdf.Printing.PdfFilePrinter.DefaultPrinterName = defaultPrinterName ?? settings.PrinterName;
            PdfSharp.Pdf.Printing.PdfFilePrinter.AdobeReaderPath = adobeReaderPath; //AcroRd32.exe or Acrobat.exe
        }

        public static Task PrintAsync(byte[] binaryData)
        {
            var printing = new Printing();

            var task = new Task(() => printing.WithoutException(binaryData));
            task.Start();
            return task;
        }

        private void WithoutException(byte[] binaryData)
        {
            try
            {
                PrintEx(binaryData);
            }
            catch (Exception exception)
            {
                //TODO: Fire an event with the exception.
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }
        }

        private void PrintEx(byte[] binaryData)
        {
            string tempFileName = null;
            try
            {
                tempFileName = string.Format("{0}.pdf", System.IO.Path.GetTempFileName());

                System.IO.File.WriteAllBytes(tempFileName, binaryData);

                //var pdfFilePrinter = new PdfSharp.Pdf.Printing.PdfFilePrinter(tempFileName);
                //pdfFilePrinter.Print(30000);

                //DoPrintNow(tempFileName, PdfSharp.Pdf.Printing.PdfFilePrinter.DefaultPrinterName);
                //var x = new ceTe.DynamicPDF.Printing.PrintJob(PdfSharp.Pdf.Printing.PdfFilePrinter.DefaultPrinterName, tempFileName);
                //x.Print();
                //System.Threading.Thread.Sleep(20000);
                
            }
            finally
            {
                if (tempFileName != null)
                {
                    if (System.IO.File.Exists(tempFileName))
                        System.IO.File.Delete(tempFileName);

                    if (System.IO.File.Exists(tempFileName))
                        throw new InvalidOperationException(string.Format("Cannot delete file {0}.", tempFileName));
                }
            }
        }

        private void DoPrintNow(string fileToPrint, string printerName)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.

                //var command = string.Format("{0} /N /T {1} {2} [ PrinterDriver [ PrinterPort ] ]", PdfSharp.Pdf.Printing.PdfFilePrinter.AdobeReaderPath, fileToPrint, printerName);
                //var command = string.Format("\"{0}\" /N /T \"{1}\" \"{2}\"", PdfSharp.Pdf.Printing.PdfFilePrinter.AdobeReaderPath, fileToPrint, printerName);
                //var arguments = string.Format("/N /T \"{0}\" \"{1}\"", fileToPrint, printerName);
                //var arguments = string.Format("/p /h \"{0}\" \"{1}\"", fileToPrint, printerName);
                var arguments = string.Format("/s /o /h /t \"{0}\" \"{1}\"", fileToPrint, printerName);
                //var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
                var procStartInfo = new System.Diagnostics.ProcessStartInfo(PdfSharp.Pdf.Printing.PdfFilePrinter.AdobeReaderPath, arguments);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true; // true;
                procStartInfo.UseShellExecute = false; // false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true; // true;
                // Now we create a process, assign its ProcessStartInfo and start it
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                //proc.WaitForExit();

                // Get the output into a string
                var result = proc.StandardOutput.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(result);
            }
            catch (Exception exception)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Swallowing {0}", exception.Message);
            }
        }
    }
}