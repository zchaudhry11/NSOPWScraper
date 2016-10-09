using System.Collections.Generic;

namespace NSOPWScraper
{
    /// <summary>
    /// This class contains the information of a single offender within the NSOPW database.
    /// </summary>
    class NSOPWEntry
    {
        // Core offender details
        public string Name;
        public int Age;
        public List<string> Aliases;
        public string StreetAddress;
        public string CityAddress;
        public string AddressCounty;
        public string AddressType;
        public string imgURL;

        public NSOPWEntry()
        {
            Name = "N/A";
            Age = -1;
            Aliases = new List<string>();
            StreetAddress = "N/A";
            CityAddress = "N/A";
            AddressCounty = "N/A";
            AddressType = "N/A";
            imgURL = "N/A";
        }
    }
}