
namespace JobScraper.ConsoleApp.Common
{
    public static class DisplayBanner
    {
        public static void Show()
        {
            string banner = @"
  ██   ██ ██ ██████  ███████     ███    ███ ███████ 
  ██   ██ ██ ██   ██ ██          ████  ████ ██      
  ███████ ██ ██████  █████       ██ ████ ██ █████   
  ██   ██ ██ ██   ██ ██          ██  ██  ██ ██      
  ██   ██ ██ ██   ██ ███████     ██      ██ ███████ 
-------------------------------------------------------";

            Random random = new Random();
            int index = random.Next(Quotes.Count);
            string quote = Quotes[index];
            string footer = $"  {quote}\n-------------------------------------------------------";

            Console.ForegroundColor = ConsoleColor.Green; // Using Green for a hiring theme

            // Fade-in effect: Print line by line with a delay
            foreach (string line in banner.Split('\n'))
            {
                Console.WriteLine(line);
                Thread.Sleep(100);  // Delay for smooth effect
            }

            // Wait before printing footer for better effect
            Thread.Sleep(300);
            Console.WriteLine(footer);

            Console.ResetColor();
        }
        
        private static readonly List<string> Quotes = new()
        {
            "Opportunities don't happen, you create them. 🌟",
            "Your dream job is closer than you think! 🚀",
            "Success is where preparation and opportunity meet. 🔥",
            "Stay persistent. Every 'no' is closer to a 'yes'! 💪",
            "Your career breakthrough starts here! 🎯",
            "A little progress each day adds up to big results. 📈",
            "Hard work beats talent when talent doesn’t work hard. ⚡",
            "Keep applying, keep growing, keep winning! 🏆"
        };

    }
}
