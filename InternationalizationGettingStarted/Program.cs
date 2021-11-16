using System;
using System.Globalization;

namespace InternationalizationGettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = "60.9";
            double.TryParse(test, out double result);

            double.TryParse(test, NumberStyles.Any, CultureInfo.InvariantCulture, out double result2);

            Console.WriteLine($"result: {result}");
            Console.WriteLine($"result2: {result2}");


            var culture = CultureInfo.CreateSpecificCulture("en-US");
            Double.TryParse("72.9", NumberStyles.Any, culture, out double result3);

            Console.WriteLine($"result3: {result3}");

            var cultureDE = CultureInfo.CreateSpecificCulture("de-DE");
            Double.TryParse("72,9", NumberStyles.Any, cultureDE, out double result4);

            Console.WriteLine($"result4: {result4}");

        }
    }
}
