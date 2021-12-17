using System;
using System.Globalization;

namespace InternationalizationGettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //1. coordinate strings
            //2. decimal types

            //string coordinates wont parse in Germany
            string lat = "46.2237845708924";
            string lng = "6.05437338352202";

   
            SetRegion(CountryType.Germany);
            string test1 = "-60.9";
            double.TryParse(test1, out double result1a);
            double.TryParse(test1, NumberStyles.Any, CultureInfo.InvariantCulture, out double result1b);

            Console.WriteLine($"{CultureInfo.CurrentCulture.Name} - US culture specific string -60.9: {result1a}, INCORRECT");
            Console.WriteLine($"{CultureInfo.CurrentCulture.Name} - US culture specific string and InvariantCulture -60.9: {result1b}, GOOD");
            Console.WriteLine();

            SetRegion(CountryType.Germany);
            decimal numberDecimal = -60.9M;
            string test3 = numberDecimal.ToString();
            double.TryParse(test3, out double result3a);
            double.TryParse(test3, NumberStyles.Any, CultureInfo.InvariantCulture, out double result3b);

            Console.WriteLine($"{CultureInfo.CurrentCulture.Name} - decimal.ToString -60.9: {result3a}, GOOD");
            Console.WriteLine($"{CultureInfo.CurrentCulture.Name} - decimal.ToString and InvariantCulture -60.9: {result3b}, INCORRECT");
            Console.WriteLine();

            SetRegion(CountryType.US);
            string test2 = "-60.9";
            double.TryParse(test2, out double result2a);
            double.TryParse(test2, NumberStyles.Any, CultureInfo.InvariantCulture, out double result2b);

            Console.WriteLine($"{CultureInfo.CurrentCulture.Name} - US culture specific string -60.9: {result2a}, GOOD");
            Console.WriteLine($"{CultureInfo.CurrentCulture.Name} - US culture specific string and InvariantCulture -60.9: {result2b}, GOOD");
            Console.WriteLine();

            Console.ReadKey();

            //decimal numberDecimal = -60.9M;
            //string test = numberDecimal.ToString();
            //double.TryParse(test, out double result);

            //double.TryParse(test, NumberStyles.Any, CultureInfo.InvariantCulture, out double result2);

            //Console.WriteLine($"result: {result}");
            //Console.WriteLine($"result2: {result2}");


            //var culture = CultureInfo.CreateSpecificCulture("en-US");
            //Double.TryParse("72.9", NumberStyles.Any, culture, out double result3);

            //Console.WriteLine($"result3: {result3}");

            //var cultureDE = CultureInfo.CreateSpecificCulture("de-DE");
            //Double.TryParse("72,9", NumberStyles.Any, cultureDE, out double result4);

            //Console.WriteLine($"result4: {result4}");

        }
        enum CountryType : ushort
        {
            US = 0,
            Germany
        }

        static void SetRegion(CountryType countryType)
        {
            switch (countryType)
            {
                case CountryType.US:
                    CultureInfo.CurrentCulture = new CultureInfo("en-US");
                    CultureInfo.CurrentUICulture = new CultureInfo("en-US");
                    break;
                case CountryType.Germany:
                    CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                    CultureInfo.CurrentUICulture = new CultureInfo("de-DE");
                    break;
                default:
                    break;
            }
        }
    }
}
