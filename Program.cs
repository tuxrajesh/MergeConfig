using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MergeConfig
{
    class Program
    {
        static int Main(string[] args)
        {            
            try
            {
                if (args == null || args.Length < 1)
                {
                    DisplayHelp();
                    return 0;
                }

                string regionName = string.Empty;
                bool verbose = false;

                foreach (string arg in args)
                {
                    if (arg.Contains("/h") || arg.Contains("/help"))
                    {
                        DisplayHelp();
                        return 0;
                    }

                    if (arg.Contains("/r:") || arg.Contains("/region:"))
                    {
                        regionName = arg.Split(':')[1];
                        continue;
                    }

                    if (arg.Contains("/v") || args.Contains("/verbose"))
                        verbose = true;
                }

                if (string.IsNullOrEmpty(regionName))
                    throw new Exception("regionname was not supplied");

                ManageMerge manager = new ManageMerge(regionName, verbose);
                List<string> errorMessages = manager.Merge();

                if (errorMessages.Count > 0)
                {
                    Console.WriteLine("-------------------------------------------------------------------------------------------------------");
                    foreach (string errorMessage in errorMessages)
                        Console.WriteLine(errorMessage);

                    Console.WriteLine();
                    Console.WriteLine("Total {0} errors.", errorMessages.Count);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("-------------------------------------------------------------------------------------------------------");
                Console.WriteLine("Error occured: {0}",ex.Message);
                Console.WriteLine("Process Exited.");
                return -1;
            }

            Console.WriteLine("Process Complete.");
            return 0;
        }

        static void DisplayHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Syntax:");
            Console.WriteLine("\t MergeConfig.exe {/r | /region}:<region_name> {/v | /verbose}");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("\t MergeConfig.exe /r:Development /v");
            Console.WriteLine("\t MergeConfig.exe /r:Development /verbose");
            Console.WriteLine("\t MergeConfig.exe /region:Development");
            Console.WriteLine();
        }
    }
}
