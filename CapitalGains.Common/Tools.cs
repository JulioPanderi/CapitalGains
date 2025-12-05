using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalGains.Common
{
    public class Tools
    {
        public static string ReadFile(string fileName)
        {
            string retValue = string.Empty;
            try
            {
                retValue = File.ReadAllText(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: File not found at {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return retValue;
        }
    }
}
