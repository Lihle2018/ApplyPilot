export interface JobListing {
  id: string;
  title: string;
  description: JobDescription;
  company: Company;
  location: Location;
  postedDate: string;
  jobType: JobType;
  experienceLevel: ExperienceLevel;
  salaryRange: SalaryRange;
}

export interface JobDescription {
  summary: string;
  responsibilities: string;
  requirements: string;
}

export interface Company {
  name: string;
  website: string;
}

export interface Location {
  city: string;
  country: string;
}

export interface SalaryRange {
  minSalary: number;
  maxSalary: number;
  currency: string;
}

export enum JobType {
  FullTime = 0,
  PartTime = 1,
  Contract = 2,
  Internship = 3,
  Remote = 4,
}

export enum ExperienceLevel {
  EntryLevel = 0,
  MidLevel = 1,
  SeniorLevel = 2,
}

export interface ScrapingRequest {
  keyword: string;
  location: string;
}

export interface ScrapingResponse {
  success: boolean;
  message: string;
  jobListings: JobListing[];
  totalCount: number;
  scrapedAt: string;
}