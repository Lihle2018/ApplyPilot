'use client';

import { JobListing, JobType, ExperienceLevel } from '@/types';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { CalendarDays, MapPin, Building, DollarSign, Trash2, ExternalLink } from 'lucide-react';

interface JobListingCardProps {
  jobListing: JobListing;
  onDelete?: (id: string) => void;
}

const getJobTypeLabel = (jobType: JobType): string => {
  switch (jobType) {
    case JobType.FullTime:
      return 'Full Time';
    case JobType.PartTime:
      return 'Part Time';
    case JobType.Contract:
      return 'Contract';
    case JobType.Internship:
      return 'Internship';
    case JobType.Remote:
      return 'Remote';
    default:
      return 'Unknown';
  }
};

const getExperienceLevelLabel = (level: ExperienceLevel): string => {
  switch (level) {
    case ExperienceLevel.EntryLevel:
      return 'Entry Level';
    case ExperienceLevel.MidLevel:
      return 'Mid Level';
    case ExperienceLevel.SeniorLevel:
      return 'Senior Level';
    default:
      return 'Unknown';
  }
};

const formatSalary = (minSalary: number, maxSalary: number, currency: string): string => {
  if (minSalary === 0 && maxSalary === 0) {
    return 'Salary not specified';
  }
  if (minSalary === maxSalary) {
    return `${currency} ${minSalary.toLocaleString()}`;
  }
  return `${currency} ${minSalary.toLocaleString()} - ${currency} ${maxSalary.toLocaleString()}`;
};

export function JobListingCard({ jobListing, onDelete }: JobListingCardProps) {
  const postedDate = new Date(jobListing.postedDate).toLocaleDateString();

  return (
    <Card className="w-full hover:shadow-lg transition-shadow">
      <CardHeader>
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <CardTitle className="text-xl mb-2">{jobListing.title}</CardTitle>
            <div className="flex flex-wrap gap-2 mb-2">
              <Badge variant="secondary">{getJobTypeLabel(jobListing.jobType)}</Badge>
              <Badge variant="outline">{getExperienceLevelLabel(jobListing.experienceLevel)}</Badge>
            </div>
          </div>
          {onDelete && (
            <Button
              variant="ghost"
              size="icon"
              onClick={() => onDelete(jobListing.id)}
              className="text-red-600 hover:text-red-700 hover:bg-red-50"
            >
              <Trash2 className="h-4 w-4" />
            </Button>
          )}
        </div>
      </CardHeader>
      
      <CardContent className="space-y-4">
        <div className="flex items-center gap-4 text-sm text-gray-600">
          <div className="flex items-center gap-1">
            <Building className="h-4 w-4" />
            <span>{jobListing.company.name}</span>
            {jobListing.company.website && (
              <Button variant="ghost" size="sm" asChild className="h-auto p-0 ml-1">
                <a 
                  href={jobListing.company.website} 
                  target="_blank" 
                  rel="noopener noreferrer"
                  className="text-blue-600 hover:text-blue-800"
                >
                  <ExternalLink className="h-3 w-3" />
                </a>
              </Button>
            )}
          </div>
          
          <div className="flex items-center gap-1">
            <MapPin className="h-4 w-4" />
            <span>{jobListing.location.city}, {jobListing.location.country}</span>
          </div>
          
          <div className="flex items-center gap-1">
            <CalendarDays className="h-4 w-4" />
            <span>Posted: {postedDate}</span>
          </div>
        </div>

        <div className="flex items-center gap-1 text-sm">
          <DollarSign className="h-4 w-4 text-green-600" />
          <span className="font-medium text-green-600">
            {formatSalary(
              jobListing.salaryRange.minSalary,
              jobListing.salaryRange.maxSalary,
              jobListing.salaryRange.currency
            )}
          </span>
        </div>

        {jobListing.description.summary && (
          <div>
            <h4 className="font-medium mb-2">Summary</h4>
            <CardDescription className="text-sm leading-relaxed">
              {jobListing.description.summary}
            </CardDescription>
          </div>
        )}

        {jobListing.description.requirements && (
          <div>
            <h4 className="font-medium mb-2">Requirements</h4>
            <CardDescription className="text-sm leading-relaxed">
              {jobListing.description.requirements}
            </CardDescription>
          </div>
        )}

        {jobListing.description.responsibilities && (
          <div>
            <h4 className="font-medium mb-2">Responsibilities</h4>
            <CardDescription className="text-sm leading-relaxed">
              {jobListing.description.responsibilities}
            </CardDescription>
          </div>
        )}
      </CardContent>
    </Card>
  );
}