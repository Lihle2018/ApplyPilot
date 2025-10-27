# JobPilot - AI-Powered Job Application Assistant

JobPilot is an AI-powered job application assistant designed to automate job searching, applications, and resume optimization. The system integrates web scraping for job listings, a MongoDB database for storage, and DeepSeek LLM for AI-driven job recommendations and insights.

![JobPilot Dashboard](https://img.shields.io/badge/Status-Active-brightgreen)
![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)
![Next.js](https://img.shields.io/badge/Next.js-15-black)
![TypeScript](https://img.shields.io/badge/TypeScript-blue)

## 🚀 Features

- **Web Scraping**: Automated job listing extraction from Google Jobs
- **Modern Frontend**: Beautiful, responsive React/Next.js interface with Tailwind CSS
- **RESTful API**: Clean .NET 8 Web API with comprehensive endpoints
- **MongoDB Integration**: Robust data storage and retrieval
- **AI Processing**: DeepSeek LLM integration for job recommendations
- **Real-time Updates**: Live job listing management
- **Clean Architecture**: CQRS pattern with MediatR
- **Type Safety**: Full TypeScript implementation

## 🏗️ Architecture

### Backend (.NET 8)
- **JobScraper.Domain**: Core entities and business rules
- **JobScraper.Application**: Application logic and CQRS commands/queries
- **JobScraper.Infrastructure**: External services and data access
- **JobScraper.Api**: Web API controllers and endpoints
- **JobScraper.Console**: Command-line interface

### Frontend (Next.js 15 + TypeScript)
- **Modern UI Components**: Built with Radix UI and Tailwind CSS
- **Type-safe API Client**: Axios-based API communication
- **Form Validation**: Zod schemas with React Hook Form
- **Responsive Design**: Mobile-first approach

## 🛠️ Tech Stack

### Backend
- **.NET 8** - Web API framework
- **MongoDB** - Document database
- **MediatR** - CQRS implementation
- **PuppeteerSharp** - Web scraping
- **FluentValidation** - Input validation

### Frontend
- **Next.js 15** - React framework
- **TypeScript** - Type safety
- **Tailwind CSS** - Utility-first CSS
- **Radix UI** - Accessible UI components
- **Axios** - HTTP client
- **React Hook Form** - Form management
- **Zod** - Schema validation

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- MongoDB (optional for testing)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd JobPilot
   ```

2. **Backend Setup**
   ```bash
   # Restore dependencies
   dotnet restore

   # Build the solution
   dotnet build

   # Update configuration (optional)
   # Edit JobScraper.Api/appsettings.json for MongoDB connection
   ```

3. **Frontend Setup**
   ```bash
   cd job-pilot-frontend

   # Install dependencies
   npm install

   # Set environment variables
   echo "NEXT_PUBLIC_API_BASE_URL=http://localhost:5000" > .env.local
   ```

### Running the Application

1. **Start the API**
   ```bash
   cd JobScraper.Api
   dotnet run --urls="http://localhost:5000"
   ```

2. **Start the Frontend**
   ```bash
   cd job-pilot-frontend
   npm run dev
   ```

3. **Access the Application**
   - Frontend: http://localhost:3000
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000 (API documentation)

## 📊 API Endpoints

### Job Listings
- `POST /api/joblistings/scrape` - Scrape job listings
- `GET /api/joblistings` - Get all job listings
- `GET /api/joblistings/{id}` - Get specific job listing
- `DELETE /api/joblistings/{id}` - Delete job listing

### Test Endpoints (Development)
- `GET /api/test/health` - Health check
- `POST /api/test/mock-scrape` - Mock job scraping for testing

### Health Check
- `GET /health` - Application health status

## 🧪 Testing

### Mock Scraper (No External Dependencies)
The application includes a mock scraper for testing without external dependencies:

```bash
curl -X POST http://localhost:5000/api/test/mock-scrape \
  -H "Content-Type: application/json" \
  -d '{"keyword": "software developer", "location": "San Francisco"}'
```

### Health Check
```bash
curl http://localhost:5000/api/test/health
```

## 🏗️ Project Structure

```
JobPilot/
├── JobScraper.Domain/           # Core domain models
│   ├── Entities/               # Domain entities
│   ├── ValueObjects/           # Value objects
│   ├── Enums/                  # Enumerations
│   └── Contracts/              # Interfaces
├── JobScraper.Application/      # Application layer
│   ├── Features/              # CQRS commands/queries
│   ├── Common/                # Shared application logic
│   └── Extensions/            # Service registration
├── JobScraper.Infrastructure/   # Infrastructure layer
│   ├── Data/                  # Database context
│   ├── Repositories/          # Data access
│   ├── Scrapers/             # Web scraping logic
│   └── AIProcessing/         # AI service integration
├── JobScraper.Api/             # Web API
│   ├── Controllers/          # API controllers
│   ├── DTOs/                 # Data transfer objects
│   └── Program.cs            # Application entry point
├── JobScraper.Console/         # Console application
└── job-pilot-frontend/         # Next.js frontend
    ├── src/
    │   ├── app/              # Next.js app router
    │   ├── components/       # React components
    │   ├── lib/              # Utilities and API client
    │   └── types/            # TypeScript type definitions
    └── public/               # Static assets
```

## 🎨 Frontend Features

- **Job Scraper Form**: Input fields for keyword and location search
- **Job Listings Display**: Beautiful card-based layout for job results
- **Real-time Feedback**: Loading states and success/error messages
- **Responsive Design**: Works on desktop, tablet, and mobile
- **Type Safety**: Full TypeScript implementation
- **Modern UI**: Clean, professional interface with Tailwind CSS

## 🔧 Configuration

### API Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017",
    "DatabaseName": "JobPilotDb",
    "JobsCollectionName": "JobListings",
    "UsersCollectionName": "Users",
    "ProxyApiUrl": "http://localhost:8000/proxy",
    "GoogleJobsSearchUrl": "https://www.google.com/search?q={0}+{1}+jobs",
    "OpenAIApiUrl": "https://api.deepseek.com/v1",
    "OpenAIToken": "your-deepseek-api-key-here"
  }
}
```

### Frontend Configuration (.env.local)
```env
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🚀 Future Enhancements

- [ ] Real Google Jobs scraper implementation with proxy rotation
- [ ] User authentication and authorization
- [ ] Resume optimization with AI
- [ ] Application tracking system
- [ ] Email notifications
- [ ] Advanced job filtering and search
- [ ] Job application automation
- [ ] Interview scheduling integration
- [ ] Salary negotiation insights

## 📞 Support

For support, email [your-email] or create an issue in the GitHub repository.

## 🎯 Demo

Visit the live demo at [demo-url] (when deployed)

---

Built with ❤️ using .NET 8, Next.js, and modern web technologies.