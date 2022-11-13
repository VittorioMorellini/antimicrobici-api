using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Utils
{
    public class ServiceResult
    {
        public static ActionResult Execute(Action action)
        {
            try
            {
                action.Invoke();
                return new OkObjectResult("OK");
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException != null ? (" " + ex.InnerException.Message) : "";
                return new BadRequestObjectResult($"{ex.Message}{innerMsg}");
            }
        }

        public static ActionResult Execute<T>(Func<T> func)
        {
            try
            {
                return new OkObjectResult(func.Invoke());
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException != null ? (" " + ex.InnerException.Message) : "";
                return new BadRequestObjectResult($"{ex.Message}{innerMsg}");
            }
        }

        public static ActionResult ExecuteAsync<T>(Func<Task<T>> func)
        {
            try
            {
                var task = func.Invoke();
                task.Wait();

                if (task.IsCompletedSuccessfully)
                    return new OkObjectResult(task.Result);
                else
                    return new BadRequestObjectResult(task.Result);
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException != null ? (" " + ex.InnerException.Message) : "";
                return new BadRequestObjectResult($"{ex.Message}{innerMsg}");
            }
        }
    }
}
