using Microsoft.AspNetCore.Mvc;
using System;

namespace BackendTestTask.Authorization
{
    public class UnauthorizedProblemDetails : ProblemDetails
    {
        // Setup response information for unauthorized user
        public UnauthorizedProblemDetails(string details = null)
        {
            Title = "Unauthorized";
            Detail = details;
            Status = 401;
            Type = "https://httpstatuses.com/401";
        }
    }
}
