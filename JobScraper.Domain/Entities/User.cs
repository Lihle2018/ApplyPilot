namespace JobScraper.Domain.Entities
{
    public class User
    {
        public string Id { get; init; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string LinkedinLink { get; private set; }
        public string GitHubLink { get; private set; }
        public string ProfileLink { get; private set; }
        public IEnumerable<string> Projects { get; private set; }
        public string Summary { get; private set; }

        public User(string name,
                    string surname,
                    string email,
                    string phoneNumber,
                    string linkedinLink,
                    string githubLink,
                    IEnumerable<string> projects,
                    string summary) 
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Surname = surname;
            Email = email;
            LinkedinLink = linkedinLink;
            GitHubLink = githubLink;
            ProfileLink = ProfileLink;
            Projects = projects;
            Summary = summary; 
        }

    }
}
