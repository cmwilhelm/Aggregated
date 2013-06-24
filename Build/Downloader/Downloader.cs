using System;
using System.IO;
using System.Net;

internal class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 2)
        {
            var url = args[0];
            var path = args[1];

            try
            {
                path = Path.GetFullPath(path);

                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, path);

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }
        }
        else
        {
            Console.Error.WriteLine("Usage: downloader.exe <url> <path>");
            return 1;
        }
    }
}

