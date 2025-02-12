using JobScraper.Domain.Contracts.Services;
using System.Text;
using System.Text.Json;

namespace JobScraper.Infrastructure.AIProcessing
{

    public class OpenAIClient(IHttpClientService httpClientService, string baseUrl, string token) : IAIProcessingService
    {
        public async Task<string> UpdateCvAsync(string jobDescription, string currentCv)
        {
            var prompt = CreateCvUpdatePrompt(jobDescription, currentCv);
            var response = await CallOpenAIAsync(prompt);
            return response.Choices[0].Message.Content;
        }

        public async Task<(string Subject, string Body)> GenerateEmailAsync(string jobDescription)
        {
            var prompt = CreateEmailPrompt(jobDescription);
            var response = await CallOpenAIAsync(prompt);
            var emailContent = response.Choices[0].Message.Content.Split("\n\n", 2);

            var subject = emailContent.Length > 1 ? emailContent[0] : "Job Application";
            var body = emailContent.Length > 1 ? emailContent[1] : emailContent[0];

            return (subject, body);
        }

        private async Task<OpenAiResponse> CallOpenAIAsync(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-4-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                temperature = 0.7,
                max_tokens = 1000
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );

            using var request = new HttpRequestMessage(HttpMethod.Post, baseUrl)
            {
                Headers = { { "Authorization", $"Bearer {token}" } },
                Content = requestContent
            };

             var responseContent = await httpClientService.PostAsync(request);
           
            return JsonSerializer.Deserialize<OpenAiResponse>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        private string CreateCvUpdatePrompt(string jobDescription, string currentCv)
        {
            return $"""
        You are an expert resume writer. Given the following job description:
        "{jobDescription}"

        And the current resume:
        "{currentCv}"

        Please update the resume to better align with the job requirements while maintaining the candidate’s authenticity and experience. Keep it concise and professional.
        """;
        }

        private string CreateEmailPrompt(string jobDescription)
        {
            return $"""
        You are an AI specializing in professional job applications. Based on the following job description:
        "{jobDescription}"

        Please generate an email application with:
        - A professional and attention-grabbing subject line.
        - A concise, well-structured email body that expresses enthusiasm, highlights key qualifications, and includes a call to action.

        Format:
        Subject: [Your generated subject here]

        Body:
        [Your generated email body here]
        """;
        }
    }

    public class OpenAiResponse
    {
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}

