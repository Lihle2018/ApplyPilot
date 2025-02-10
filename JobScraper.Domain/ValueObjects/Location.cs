namespace JobScraper.Domain.ValueObjects
{

    public class Location
    {
        public string City { get; private set; }
        public string Country { get; private set; }

        public Location(string city, string country)
        {
            City = city;
            Country = country;
        }
    }
}
