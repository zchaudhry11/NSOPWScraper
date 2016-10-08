using System;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace NSOPWScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load("C:\\Users\\Zia\\Desktop\\NSOPW_Data\\a\\a_a.htm");
            HtmlNodeCollection name = doc.DocumentNode.SelectNodes("//*[starts-with(@class,'name')]");

            Console.WriteLine(name[0].ChildNodes[1].InnerText.Trim()); // Print name

            HtmlNodeCollection age = doc.DocumentNode.SelectNodes("//*[starts-with(@class,'age')]");

            Console.WriteLine(age[0].InnerText); // Print age

            HtmlNodeCollection aliases = doc.DocumentNode.SelectNodes("//*[starts-with(@class,'aliases')]");

            // Get list of all aliases
            HtmlNode aliasList = aliases[0].ChildNodes[1];

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

                /* // Keep commas
                if (offenderAlias.Contains("null"))
                {
                    offenderAlias = offenderAlias.Replace(replace, ", ");
                    replace = "&nbsp;";
                    offenderAlias = offenderAlias.Replace(replace, "");
                    replace = "null";
                    offenderAlias = offenderAlias.Replace(replace, "");
                }
                else
                {
                    offenderAlias = offenderAlias.Replace(replace, ", ");
                    replace = "&nbsp;";

                    // Check if non-breaking spaces are at end of string
                    if (offenderAlias.EndsWith(replace))
                    {
                        offenderAlias = offenderAlias.Replace(replace, "");
                    }
                    else
                    {
                        offenderAlias = offenderAlias.Replace(replace, ", ");
                    }                    
                }
                */


                Console.WriteLine(offenderAlias); // Print alias
            }

            HtmlNodeCollection addressInfo = doc.DocumentNode.SelectNodes("//*[starts-with(@class,'addr')]");

            string address = addressInfo[0].ChildNodes[1].InnerText;
            string replace = "&nbsp;";

            address = address.Replace(replace, " ");
            
            //address = address.Trim();

            string[] tokens = address.Split('\n');

            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = tokens[i].Trim();
                tokens[i] = Regex.Replace(tokens[i], @"\s+", " ");
            }

            Console.WriteLine(tokens[1]); // street
            Console.WriteLine(tokens[2]); // city, state, zipcode
            Console.WriteLine(tokens[3]); // county
            Console.WriteLine(tokens[4]); // type of address
        }
    }
}