

using System.Text.RegularExpressions;

namespace JobScraper.ConsoleApp.Helpers
{
    public static class Validators
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^\+?\d{10,15}$", RegexOptions.Compiled);
        private static readonly Regex UrlRegex = new(@"^https?:\/\/[^\s]+$", RegexOptions.Compiled);

        public static string ValidateEmail(string email)
        {
            while (!EmailRegex.IsMatch(email))
            {
                Console.Write("Invalid email. Enter a valid email: ");
                email = Console.ReadLine();
            }
            return email;
        }

        public static string ValidatePhoneNumber(string phoneNumber)
        {
            while (!PhoneRegex.IsMatch(phoneNumber))
            {
                Console.Write("Invalid phone number. Enter a valid phone number (10-15 digits, optional +): ");
                phoneNumber = Console.ReadLine();
            }
            return phoneNumber;
        }

        public static string ValidateUrl(string url, string fieldName)
        {
            while (!UrlRegex.IsMatch(url))
            {
                Console.Write($"Invalid {fieldName} URL. Enter a valid {fieldName} URL: ");
                url = Console.ReadLine();
            }
            return url;
        }

        public static string ValidateSummary(string summary)
        {
            while (summary.Length < 200)
            {
                Console.WriteLine($"Summary is too short. You need {200 - summary.Length} more characters.");
                Console.Write("Enter your summary (at least 200 characters): ");
                summary = Console.ReadLine();
            }
            return summary;
        }
    }
}
