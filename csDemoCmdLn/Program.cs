using System;
using ServiceLibrary;

namespace vsDemoCmdLn
{
    class Program
    {
        static void Main(string[] args)
        {
            String importFile = "/Users/dbedoya/dev/csharp/vsDemoCmdLn/balances.csv";
            String exportFile = "/Users/dbedoya/dev/csharp/vsDemoCmdLn/summary.csv";
            DateTime dtdatetime = DateTime.Now;

            ImportService importService = new ImportService(importFile, dtdatetime);
            ExportService exportService = new ExportService(importFile, dtdatetime, exportFile);

            int ip = importService.ProcessImport();

            Console.WriteLine("ProcessImport: {0}", ip);  //Debug Info

            int ep = exportService.ProcessExport();

            Console.WriteLine("ProcessExport: {0}", ep);  //Debug Info

            return;
        }
    }
}
