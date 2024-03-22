using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisinessLayer.CustomException;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;

namespace BuisinessLayer.Filter.ExceptionFilter
{
    public class UserExceptionHandlerFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            if (context.Exception is SqlException)
            {
                context.ModelState.AddModelError("EMAIL ID", context.Exception.Message);
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status409Conflict;
                context.Result = new ConflictObjectResult(problemdetail);
            }
           else if (context.Exception is PasswordMissmatchException)
            {
                context.ModelState.AddModelError("Password", context.Exception.Message);
                ValidationProblemDetails problemDetails=new ValidationProblemDetails(context.ModelState);
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                context.Result=new UnauthorizedObjectResult(problemDetails);
            }
            else if (context.Exception is UserNotFoundException)
            {
                context.ModelState.AddModelError("UserNotFound", context.Exception.Message);
                ValidationProblemDetails problemDetails = new ValidationProblemDetails(context.ModelState);
                problemDetails.Status = StatusCodes.Status404NotFound;
                context.Result = new UnauthorizedObjectResult(problemDetails);
            }



            else
            {
                context.ModelState.AddModelError("unknow exceptio contact developer", context.Exception.Message);
                ValidationProblemDetails problemDetails = new ValidationProblemDetails(context.ModelState);
                problemDetails.Status = StatusCodes.Status404NotFound;
                context.Result = new UnauthorizedObjectResult(problemDetails);
            }
        }
    }
}
