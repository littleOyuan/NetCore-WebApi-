using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RestfulApi
{
    public class CustomizedExceptionFilterAttribute : ExceptionFilterAttribute
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public CustomizedExceptionFilterAttribute(IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public override void OnException(ExceptionContext context)
        {
            //ObjectResult objectResult = new ObjectResult(new ResponseMessage(false, context.Exception.Message))
            //{
            //    StatusCode = (int)HttpStatusCode.OK
            //};

            context.Result = new OkObjectResult(new ResponseMessage(false, context.Exception.Message));

            LogHelper.Error(context, context.Exception.Message, context.Exception);
        }
    }
}
