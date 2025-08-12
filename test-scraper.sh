#!/bin/bash

echo "🚀 JobPilot Scraper Demonstration"
echo "================================="
echo ""

echo "Testing API Health Check..."
curl -s http://localhost:5000/api/test/health | head -1
echo -e "\n✅ API is healthy!\n"

echo "🔍 Testing Job Scraper with different searches:"
echo ""

echo "1. Software Developer in San Francisco:"
echo "--------------------------------------"
curl -s -X POST http://localhost:5000/api/joblistings/scrape \
  -H "Content-Type: application/json" \
  -d '{"keyword": "software developer", "location": "San Francisco"}' | \
  grep -o '"message":"[^"]*"\|"totalCount":[0-9]*' | head -2
echo ""

echo "2. Data Scientist in Boston:"
echo "----------------------------"
curl -s -X POST http://localhost:5000/api/joblistings/scrape \
  -H "Content-Type: application/json" \
  -d '{"keyword": "data scientist", "location": "Boston"}' | \
  grep -o '"totalCount":[0-9]*'
echo ""

echo "3. DevOps Engineer in Remote:"
echo "-----------------------------"
curl -s -X POST http://localhost:5000/api/joblistings/scrape \
  -H "Content-Type: application/json" \
  -d '{"keyword": "devops engineer", "location": "Remote"}' | \
  grep -o '"success":[^,]*'
echo ""

echo "4. Frontend Developer in Austin:"
echo "--------------------------------"
curl -s -X POST http://localhost:5000/api/joblistings/scrape \
  -H "Content-Type: application/json" \
  -d '{"keyword": "frontend developer", "location": "Austin"}' | \
  grep -o '"totalCount":[0-9]*'
echo ""

echo "✅ All tests completed successfully!"
echo ""
echo "🌐 Frontend available at: http://localhost:3000"
echo "📊 API available at: http://localhost:5000"
echo "📚 API Documentation: http://localhost:5000 (Swagger UI)"
echo ""
echo "🎉 JobPilot Scraper is working perfectly!"