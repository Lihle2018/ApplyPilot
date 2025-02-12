
using MediatR;
using Serilog;

namespace JobScraper.Application.Common.Behaviors
{

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Log the start of the request
            Log.Information("Handling {RequestName} with data: {@RequestData}", typeof(TRequest).Name, request);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Invoke the next step in the pipeline
                var response = await next();

                // Stop the stopwatch and log the duration
                stopwatch.Stop();
                Log.Information("Handled {RequestName} - Response: {@Response} - Duration: {ElapsedMilliseconds} ms",
                    typeof(TRequest).Name, response, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                // Stop the stopwatch in case of an exception
                stopwatch.Stop();
                Log.Error(ex, "An error occurred while handling {RequestName} - Duration: {ElapsedMilliseconds} ms",
                    typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

                throw; // Re-throw the exception to let the ExceptionHandlingBehavior handle it
            }
        }

    }
}
