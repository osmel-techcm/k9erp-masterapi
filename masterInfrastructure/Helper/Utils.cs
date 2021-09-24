using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace masterInfrastructure.Helper
{
    public class Utils
    {
        public static bool isValidEmailAddress(string email) {
            Regex rx = new Regex(@"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

        public string getSearchListFilterData(string filterData)
        {
            var filterDataSt = string.Empty;

            if (string.IsNullOrEmpty(filterData))
            {
                return filterDataSt;
            }

            var filterArray = JObject.Parse(filterData);

            foreach (var filt in filterArray)
            {
                if (!string.IsNullOrEmpty(filt.Value.ToString()))
                {
                    filterDataSt += " AND LOWER(ISNULL([" + filt.Key + "],'')) LIKE LOWER('%" + filt.Value.ToString().Replace("'", "''").Replace("%", "[%]") + "%') ";
                }
            }

            return filterDataSt;
        }
    }    
}
