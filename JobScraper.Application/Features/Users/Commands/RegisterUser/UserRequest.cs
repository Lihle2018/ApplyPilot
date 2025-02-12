namespace JobScraper.Application.Features.Users.Commands.RegisterUser
{
    public class UserRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LinkedinLink { get; set; }
        public string GitHubLink { get; set; }
        public string ProfileLink { get; set; }
        public IEnumerable<string> Projects { get; set; }
        public string Summary { get; set; }
    }
}
