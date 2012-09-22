using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace MergeConfig
{
    class ManageMerge
    {
        /// <summary>
        /// Region of Deployment
        /// </summary>
        private string region;
        private bool verbose;

        /// <summary>
        /// Manage Merge
        /// </summary>
        /// <param name="region">Region</param>
        public ManageMerge(string region, bool verbose)
        {
            this.region = region;
            this.verbose = verbose;
        }

        /// <summary>
        /// Perform Merge
        /// </summary>
        public List<string> Merge()
        {
            List<string> errorMessages = new List<string>();

            // Load Configuration file for this Application
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            XDocument mergeConfig = XDocument.Load(fileInfo.Directory.FullName + "\\MergeConfig.xml");

            // Select File List to Update
            var files = mergeConfig.XPathSelectElements("//deploymentRegion[@name='" + region + "']/fileInfo[@fileName]");

            if (files == null)
                throw new Exception(string.Format("Region Name: {0} specified is not valid.", region));

            foreach (var file in files)
            {
                try
                {
                    // Process File
                    XDocument configFile = XDocument.Load(file.Attribute("fileName").Value);

                    if (verbose)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Processing File: {0}", file.Attribute("fileName").Value);
                    }

                    var manageConfigs = file.Elements();
                    foreach (var manageConfig in manageConfigs)
                    {
                        // Locate element to update
                        XElement element = configFile.XPathSelectElement(manageConfig.Attribute("findXPath").Value);

                        if (element == null)
                        {
                            errorMessages.Add(string.Format("In File: {0}, Path: {1} was not found.", file.Attribute("fileName").Value, manageConfig.Attribute("findXPath").Value));
                            continue;
                        }

                        foreach (XAttribute attribute in element.Attributes())
                        {
                            // Find Attribute to update
                            if (attribute.Name == manageConfig.Attribute("replaceAttribute").Value)
                            {
                                // Make the update
                                attribute.SetValue(manageConfig.Attribute("replaceWith").Value);
                                
                                if(verbose)
                                    Console.WriteLine("\t Updated attribute: {0} with {1}", attribute.Name, attribute.Value);
                            }
                        }
                    }

                    // Save the changes to the file
                    configFile.Save(file.Attribute("fileName").Value);

                    if (verbose)
                        Console.WriteLine();
                }
                catch (Exception ex)
                {
                    errorMessages.Add(string.Format("Error Occured For File: {0}. Error: {1}", file.Attribute("fileName").Value, ex.Message));
                    continue;
                }
            }

            return errorMessages;
        }
    }
}
