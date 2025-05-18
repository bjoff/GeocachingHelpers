using System.Text.RegularExpressions;

namespace bjoff.GeocachingHelpers;

public class WaldmeisterReverseWherigo
{
    public static string ConvertString(string stringInput)
    {
        var input = GetNumbers(stringInput);
        if (input.Length != 18)
        {
            throw new ArgumentException("Invalid input format. Provide one string (18 digits)");
        }

        var first = input.Substring(0, ComponentLength);
        var second = input.Substring(ComponentLength, ComponentLength);
        var third = input.Substring(2 * ComponentLength);

        if (int.TryParse(first, out var a) && int.TryParse(second, out var b) && int.TryParse(third, out var c))
        {
            (var latitude, var longitude) = CalculateCoordinates(a, b, c);
            return $"N {DecimalToWgs84(latitude)} E {DecimalToWgs84(longitude)}";
        }
        return $"Not able to create a coordinate from {first}-{second}-{third}";
    }

    static string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }

    static string DecimalToWgs84(double dec)
    {
        // Split into degrees and fractional part
        int degrees = (int)dec;
        double minutes = (dec - degrees) * 60;
        return $"{degrees} {minutes:00.000}";
    }

    static (double latitude, double longitude) CalculateCoordinates(int a, int b, int c)
    {
        // Parity check: sum of 5th and 2nd digits from right in 'c'
        int fifthDigit = (c % 100000 - c % 10000) / 10000;
        int secondDigit = (c % 100 - c % 10) / 10;
        bool isEven = (fifthDigit + secondDigit) % 2 == 0;

        double latitude, longitude;

        if (isEven)
        {
            longitude =
                (a % 100000 - a % 10000) / 10000.0 * 100 +
                (c % 1000000 - c % 100000) / 100000.0 * 10 +
                c % 10 +
                (b % 1000 - b % 100) / 100.0 * 0.1 +
                (b % 1000000 - b % 100000) / 100000.0 * 0.01 +
                (a % 100 - a % 10) / 10.0 * 0.001 +
                (c % 100000 - c % 10000) / 10000.0 * 0.0001 +
                b % 10 * 0.00001;

            latitude =
                (a % 10000 - a % 1000) / 1000.0 * 10 +
                (b % 100 - b % 10) / 10.0 +
                (b % 100000 - b % 10000) / 10000.0 * 0.1 +
                (c % 1000 - c % 100) / 100.0 * 0.01 +
                (a % 1000000 - a % 100000) / 100000.0 * 0.001 +
                (c % 100 - c % 10) / 10.0 * 0.0001 +
                a % 10 * 0.00001;
        }
        else
        {
            longitude =
                (b % 100 - b % 10) / 10.0 * 100 +
                c % 10 * 10 +
                (a % 100 - a % 10) / 10.0 +
                (a % 100000 - a % 10000) / 10000.0 * 0.1 +
                (b % 1000 - b % 100) / 100.0 * 0.01 +
                b % 10 * 0.001 +
                (c % 100000 - c % 10000) / 10000.0 * 0.0001 +
                (b % 100000 - b % 10000) / 10000.0 * 0.00001;

            latitude =
                (b % 1000000 - b % 100000) / 100000.0 * 10 +
                a % 10 +
                (a % 10000 - a % 1000) / 1000.0 * 0.1 +
                (c % 1000000 - c % 100000) / 100000.0 * 0.01 +
                (c % 1000 - c % 100) / 100.0 * 0.001 +
                (c % 100 - c % 10) / 10.0 * 0.0001 +
                (a % 1000000 - a % 100000) / 100000.0 * 0.00001;
        }

        return (latitude, longitude);
    }

    private const int ComponentLength = 6;
}
