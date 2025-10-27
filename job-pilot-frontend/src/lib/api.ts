import axios from 'axios';
import { JobListing, ScrapingRequest, ScrapingResponse } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const jobApi = {
  // Scrape job listings (real implementation)
  scrapeJobs: async (request: ScrapingRequest): Promise<ScrapingResponse> => {
    const response = await apiClient.post('/api/joblistings/scrape', request);
    return response.data;
  },

  // Mock scrape job listings (for testing)
  scrapeJobsMock: async (request: ScrapingRequest): Promise<ScrapingResponse> => {
    const response = await apiClient.post('/api/test/mock-scrape', request);
    return response.data;
  },

  // Get all job listings
  getAllJobs: async (): Promise<JobListing[]> => {
    const response = await apiClient.get('/api/joblistings');
    return response.data;
  },

  // Get a specific job listing
  getJob: async (id: string): Promise<JobListing> => {
    const response = await apiClient.get(`/api/joblistings/${id}`);
    return response.data;
  },

  // Delete a job listing
  deleteJob: async (id: string): Promise<void> => {
    await apiClient.delete(`/api/joblistings/${id}`);
  },

  // Health check
  healthCheck: async (): Promise<{ status: string; timestamp: string }> => {
    const response = await apiClient.get('/api/test/health');
    return response.data;
  },
};