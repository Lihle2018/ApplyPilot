namespace JobScraper.Domain.ValueObjects
{
    public class Company
    {
        public string Name { get; private set; }
        public string Website { get; private set; }

        public Company(string name, string website)
        {
            Name = name;
            Website = website;
        }
    }

}
