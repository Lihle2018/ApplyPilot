'use client';

import { useState, useEffect } from 'react';
import { JobScraperForm } from '@/components/job-scraper-form';
import { JobListingCard } from '@/components/job-listing-card';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { jobApi } from '@/lib/api';
import { JobListing, ScrapingResponse } from '@/types';
import { RefreshCw, Database, AlertCircle, CheckCircle } from 'lucide-react';

export default function Home() {
  const [jobListings, setJobListings] = useState<JobListing[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [lastScrapeResult, setLastScrapeResult] = useState<ScrapingResponse | null>(null);

  const loadJobListings = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const jobs = await jobApi.getAllJobs();
      setJobListings(jobs);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load job listings');
    } finally {
      setIsLoading(false);
    }
  };

  const handleScrapingComplete = (response: ScrapingResponse) => {
    setLastScrapeResult(response);
    if (response.success) {
      setJobListings(response.jobListings);
    }
  };

  const handleDeleteJob = async (id: string) => {
    try {
      await jobApi.deleteJob(id);
      setJobListings(prevJobs => prevJobs.filter(job => job.id !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete job listing');
    }
  };

  useEffect(() => {
    loadJobListings();
  }, []);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">JobPilot</h1>
              <p className="text-gray-600 mt-1">AI-powered job application assistant</p>
            </div>
            <div className="flex items-center gap-4">
              <Button
                variant="outline"
                onClick={loadJobListings}
                disabled={isLoading}
                className="flex items-center gap-2"
              >
                <RefreshCw className={`h-4 w-4 ${isLoading ? 'animate-spin' : ''}`} />
                Refresh
              </Button>
            </div>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="space-y-8">
          {/* Scraper Form */}
          <JobScraperForm onScrapingComplete={handleScrapingComplete} />

          {/* Scraping Result */}
          {lastScrapeResult && (
            <Card className="w-full max-w-2xl mx-auto">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  {lastScrapeResult.success ? (
                    <CheckCircle className="h-5 w-5 text-green-600" />
                  ) : (
                    <AlertCircle className="h-5 w-5 text-red-600" />
                  )}
                  Scraping Result
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className={`text-sm ${lastScrapeResult.success ? 'text-green-600' : 'text-red-600'}`}>
                  {lastScrapeResult.message}
                </p>
                {lastScrapeResult.success && (
                  <p className="text-sm text-gray-600 mt-1">
                    Found {lastScrapeResult.totalCount} job listings • 
                    Scraped at {new Date(lastScrapeResult.scrapedAt).toLocaleString()}
                  </p>
                )}
              </CardContent>
            </Card>
          )}

          {/* Job Listings Section */}
          <div className="space-y-6">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <Database className="h-6 w-6" />
                <h2 className="text-2xl font-bold">Job Listings ({jobListings.length})</h2>
              </div>
            </div>

            {error && (
              <Card className="border-red-200 bg-red-50">
                <CardContent className="pt-6">
                  <div className="flex items-center gap-2 text-red-600">
                    <AlertCircle className="h-5 w-5" />
                    <span>{error}</span>
                  </div>
                </CardContent>
              </Card>
            )}

            {isLoading ? (
              <div className="flex justify-center py-12">
                <RefreshCw className="h-8 w-8 animate-spin text-gray-400" />
              </div>
            ) : jobListings.length === 0 ? (
              <Card className="w-full">
                <CardContent className="flex flex-col items-center justify-center py-12 text-center">
                  <Database className="h-12 w-12 text-gray-400 mb-4" />
                  <CardTitle className="text-xl mb-2">No job listings found</CardTitle>
                  <CardDescription className="max-w-md">
                    Use the scraper above to find job listings, or they might be loading from the database.
                  </CardDescription>
                </CardContent>
              </Card>
            ) : (
              <div className="grid gap-6 md:grid-cols-1 lg:grid-cols-2">
                {jobListings.map((job) => (
                  <JobListingCard
                    key={job.id}
                    jobListing={job}
                    onDelete={handleDeleteJob}
                  />
                ))}
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
