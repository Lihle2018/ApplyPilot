'use client';

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Search, Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { jobApi } from '@/lib/api';
import { ScrapingResponse } from '@/types';

const scrapingSchema = z.object({
  keyword: z.string().min(2, 'Keyword must be at least 2 characters'),
  location: z.string().min(2, 'Location must be at least 2 characters'),
});

type ScrapingFormData = z.infer<typeof scrapingSchema>;

interface JobScraperFormProps {
  onScrapingComplete: (response: ScrapingResponse) => void;
}

export function JobScraperForm({ onScrapingComplete }: JobScraperFormProps) {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ScrapingFormData>({
    resolver: zodResolver(scrapingSchema),
    defaultValues: {
      keyword: 'software developer',
      location: 'remote',
    },
  });

  const onSubmit = async (data: ScrapingFormData) => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await jobApi.scrapeJobs(data);
      onScrapingComplete(response);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to scrape jobs');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Card className="w-full max-w-2xl mx-auto">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Search className="h-6 w-6" />
          Job Scraper
        </CardTitle>
        <CardDescription>
          Enter a keyword and location to scrape job listings from Google Jobs
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <label htmlFor="keyword" className="text-sm font-medium">
                Keyword
              </label>
              <Input
                id="keyword"
                placeholder="e.g., software developer"
                {...register('keyword')}
                disabled={isLoading}
              />
              {errors.keyword && (
                <p className="text-sm text-red-600">{errors.keyword.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <label htmlFor="location" className="text-sm font-medium">
                Location
              </label>
              <Input
                id="location"
                placeholder="e.g., San Francisco or remote"
                {...register('location')}
                disabled={isLoading}
              />
              {errors.location && (
                <p className="text-sm text-red-600">{errors.location.message}</p>
              )}
            </div>
          </div>

          {error && (
            <div className="p-3 text-sm text-red-600 bg-red-50 border border-red-200 rounded-md">
              {error}
            </div>
          )}

          <Button type="submit" className="w-full" disabled={isLoading}>
            {isLoading ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Scraping jobs...
              </>
            ) : (
              <>
                <Search className="mr-2 h-4 w-4" />
                Scrape Jobs
              </>
            )}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}