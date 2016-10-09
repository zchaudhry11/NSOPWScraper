using System;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace NSOPWScraper
{
    class Program
    {
        // Paths of csv file for each table
        private static string coreCSVPath = "C:\\Users\\Zia\\Desktop\\NSOPW_Core.csv";
        private static string imgCSVPath = "C:\\Users\\Zia\\Desktop\\NSOPW_img.csv";
        private static string aliasCSVPath = "C:\\Users\\Zia\\Desktop\\NSOPW_alias.csv";

        private static StringBuilder offenderEntry = new StringBuilder(); // Final offender string that is written to csv

        private static List<string> parseList = new List<string>(); // List of all files to parse

        static void Main(string[] args)
        {
            // Gets a list of all files to parse
            string rootHTMLPath = "C:\\Users\\Zia\\Desktop\\NSOPW_Data";

            DirSearch(rootHTMLPath);

            // Try to parse all files in the parseList
            for (int i = 0; i < parseList.Count; i++)
            {
                Console.WriteLine(parseList[i] + "\n");
                ParseHTMLFile(parseList[i]);
                break;
            }
        }
        
        /// <summary>
        /// Parses the html file located at the input file path and tries to build an NSOPWEntry object from the data.
        /// </summary>
        /// <param name="filePath">String containing the location of the HTML file to parse.</param>
        private static void ParseHTMLFile(string filePath)
        {
            // Load HTML File
            HtmlDocument doc = new HtmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch(Exception e)
            {
                doc = null;
            }            

            if (doc == null)
            {
                Console.WriteLine("Document could not be opened or found!");
                return;
            }

            NSOPWEntry entry = new NSOPWEntry();

            // Parse HTML for data
            int itr = 0; // Iterator that controls which person in table's data to parse
            while (true)
            {
                /*Name*/
                HtmlNodeCollection name = doc.DocumentNode.SelectNodes("//*[starts-with(@class,'name')]");

                // Name guard checks
                if (name != null)
                {
                    // End of document check
                    if (itr >= name.Count)
                    {
                        Console.WriteLine("End of document!");
                        break;
                    }

                    // Child node checks
                    if (name[itr].HasChildNodes)
                    {
                        if (name[itr].ChildNodes.Count >= 1)
                        {
                            if (name[itr].ChildNodes[1] != null)
                            {
                                string cleanedName = name[itr].ChildNodes[1].InnerText.Trim();
                                cleanedName = cleanedName.Replace(", ", " ");
                                entry.Name = cleanedName; // Populate object with name
                                //Console.WriteLine(name[itr].ChildNodes[1].InnerText.Trim()); 
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Name is null at index " + itr);
                    break;
                }

                /*Age*/
                HtmlNodeCollection age = doc.DocumentNode.SelectNodes("//*[starts-with(@class,'age')]");

                // Age guard checks
                if (age != null)
                {
                    // End of document check
                    if (itr >= age.Count)
                    {
                        Console.WriteLine("End of document!");
                        break;
                    }

                    try
                    {
                        entry.Age = int.Parse(age[itr].InnerText.Trim()); // Populate object with age
                    }
                    catch (Exception e)
                    {
                        entry.Age = -1;
                    }
                    
                    //Console.WriteLine(age[itr].InnerText); 
                }
                else
                {
                    Console.WriteLine("Age is null at index " + itr);
                    break;
                }

                /*Aliases*/
                HtmlNodeCollection aliases = doc.DocumentNode.SelectNodes("//*[starts-with(@class,'aliases')]");

                // Alias guard checks
                if (aliases != null)
                {
                    // End of document check
                    if (itr >= aliases.Count)
                    {
                        Console.WriteLine("End of document!");
                        break;
                    }

                    if (aliases[itr].HasChildNodes)
                    {
                        // Child node checks
                        if (aliases[itr].ChildNodes.Count >= 1)
                        {
                            // Get list of all aliases
                            HtmlNode aliasList = aliases[itr].ChildNodes[1];

                            if (aliasList != null)
                            {
                                entry.Aliases.Clear();
                                // Loop through list of aliases, clean the string value then add to NSOPWEntry object
                                for (int i = 1; i < aliasList.ChildNodes.Count; i++)
                                {
                                    // Remove all nulls and non-breaking spaces of string
                                    string offenderAlias = aliasList.ChildNodes[i].InnerText.Trim();
                                    string replace1 = " , ";
                                    string replace2 = "&nbsp;";
                                    string replace3 = "null";
                                    offenderAlias = offenderAlias.Replace(replace1, " ");
                                    offenderAlias = offenderAlias.Replace(replace2, " ");
                                    offenderAlias = offenderAlias.Replace(replace3, " ");

                                    entry.Aliases.Add(offenderAlias.Trim()); // Populate object with aliases
                                    //Console.WriteLine(offenderAlias); 
                                }
                            }
                            else
                            {
                                Console.WriteLine("aliasList is null at index " + itr);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Alias is null at index " + itr);
                    break;
                }

                /*Address*/
                HtmlNodeCollection addressInfo = doc.DocumentNode.SelectNodes("//*[starts-with(@class,'addr')]");

                // Address guard checks
                if (addressInfo != null)
                {
                    // End of document check
                    if (itr >= aliases.Count)
                    {
                        Console.WriteLine("End of document!");
                        break;
                    }

                    // Child node checks
                    if (addressInfo[itr].HasChildNodes)
                    {
                        if (addressInfo[itr].ChildNodes.Count >= 1)
                        {
                            if (addressInfo[itr].ChildNodes[1] != null)
                            {
                                string address = addressInfo[itr].ChildNodes[1].InnerText;
                                string replace = "&nbsp;";

                                address = address.Replace(replace, " ");

                                // Split entire address string into separate lines to get each address detail separately
                                string[] tokens = address.Split('\n');

                                for (int i = 0; i < tokens.Length; i++)
                                {
                                    tokens[i] = tokens[i].Trim();
                                    tokens[i] = Regex.Replace(tokens[i], @"\s+", " ");
                                }
                                if (tokens.Length >= 2)
                                {
                                    tokens[1] = tokens[1].Replace(", ", " ");
                                    entry.StreetAddress = tokens[1].Trim(); // Populate object with street
                                    //Console.WriteLine(tokens[1]); 
                                }

                                if (tokens.Length >= 3)
                                {
                                    tokens[2] = tokens[2].Replace(", ", " ");
                                    entry.CityAddress = tokens[2].Trim(); // Populate object with city, state, zipcode
                                    //Console.WriteLine(tokens[2]); 
                                }

                                if (tokens.Length >= 4)
                                {
                                    tokens[3] = tokens[3].Replace(", ", " ");
                                    entry.AddressCounty = tokens[3].Trim(); // Populate object with county
                                    //Console.WriteLine(tokens[3]); 
                                }

                                if (tokens.Length >= 5)
                                {
                                    tokens[4] = tokens[4].Replace(", ", " ");
                                    entry.AddressType = tokens[4].Trim(); // Populate object with type of address
                                    //Console.WriteLine(tokens[4]); 
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Address is null at index " + itr);
                    break;
                }

                /*Image*/
                HtmlNodeCollection img = doc.DocumentNode.SelectNodes("//*[starts-with(@class,'offender-thumbnail')]");

                // Image guard checks
                if (img != null)
                {
                    // End of document check
                    if (itr >= img.Count)
                    {
                        Console.WriteLine("End of document!");
                        break;
                    }

                    // Attribute checks
                    if (img[itr].HasAttributes)
                    {
                        if (img[itr].Attributes.Count >= 1)
                        {
                            if (img[itr].Attributes[1] != null)
                            {
                                entry.imgURL = img[itr].Attributes[1].Value.Trim(); // Populate object with image url
                                //Console.WriteLine(img[itr].Attributes[1].Value); 
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Image is null at index " + itr);
                    break;
                }

                itr++; // Increment itr to get the next person in the table

                WriteEntry(entry);

                /*TESTING
                if (itr == 2)
                {
                    break;
                }
                */
            }
            // return
        }

        /// <summary>
        /// Writes a NSOPWEntry object to a csv file.
        /// </summary>
        /// <param name="csvEntry">Entry to be added to the csv file.</param>
        private static void WriteEntry(NSOPWEntry csvEntry)
        {
            /*WRITE CORE*/

            // Write to core csv
            string coreEntry = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", csvEntry.Name, csvEntry.Age, csvEntry.StreetAddress, csvEntry.CityAddress, csvEntry.AddressCounty, csvEntry.AddressType);
            offenderEntry.AppendLine(coreEntry);

            // If csv doesn't exist, create a new one with the proper heading
            if (!File.Exists(coreCSVPath))
            {
                string heading = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", "NAME", "AGE", "STREET_ADDRESS", "CITY_ADDRESS", "COUNTY", "ADDRESS_TYPE");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(heading);

                File.AppendAllText(coreCSVPath, sb.ToString());
            }

            // Insert data into core csv
            File.AppendAllText(coreCSVPath, offenderEntry.ToString());
            offenderEntry = offenderEntry.Clear();

            /*WRITE IMG*/

            // Write to img csv
            string imgEntry = string.Format("{0}, {1}", csvEntry.Name, csvEntry.imgURL);
            offenderEntry.AppendLine(imgEntry);

            // If csv doesn't exist, create a new one with the proper heading
            if (!File.Exists(imgCSVPath))
            {
                string heading = string.Format("{0}, {1}", "NAME", "IMAGE");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(heading);

                File.AppendAllText(imgCSVPath, sb.ToString());
            }

            // Insert data into img csv
            File.AppendAllText(imgCSVPath, offenderEntry.ToString());
            offenderEntry = offenderEntry.Clear();

            /*WRITE ALIAS*/
            int ctr = 0;
            // Build alias insertion
            for (int i = 0; i < csvEntry.Aliases.Count; i++)
            {
                
                if (!string.IsNullOrWhiteSpace(csvEntry.Aliases[i]))
                {
                    string aliasEntry = string.Format("{0}, {1}", csvEntry.Name, csvEntry.Aliases[i]);
                    offenderEntry.AppendLine(aliasEntry);
                    ctr++;
                }
            }

            Console.WriteLine(ctr);

            // If csv doesn't exist, create a new one with the proper heading
            if (!File.Exists(aliasCSVPath))
            {
                string heading = string.Format("{0}, {1}", "NAME", "ALIAS");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(heading);

                File.AppendAllText(aliasCSVPath, sb.ToString());
            }

            // Write to alias csv
            File.AppendAllText(aliasCSVPath, offenderEntry.ToString());
            offenderEntry = offenderEntry.Clear();
        }


        /// <summary>
        /// Recursively get the path of all files to parse.
        /// </summary>
        /// <param name="rootPath">Path of the root folder to search.</param>
        private static void DirSearch(string rootPath)
        {
            try
            {
                foreach (string directoryName in Directory.GetDirectories(rootPath))
                {
                    foreach (string fileName in Directory.GetFiles(directoryName))
                    {
                        if (fileName.EndsWith(".htm") || fileName.EndsWith(".html"))
                        {
                            parseList.Add(fileName);
                            //Console.WriteLine(fileName);
                        }
                    }
                    DirSearch(directoryName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

    }
}