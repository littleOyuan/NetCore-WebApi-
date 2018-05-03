using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace RestfulApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Token")]
    public class TokenController : Controller
    {
        private readonly IOptions<JwtOptions> _jwtOptions;

        public TokenController(IOptions<JwtOptions> options)
        {
            _jwtOptions = options;
        }

        public ResponseMessage Get([FromQuery]string applicationName, long timeStamp, string signature)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new Exception("applicationName不能为空");

            if (timeStamp <= 0)
                throw new Exception("timeStamp不能为空");

            if (string.IsNullOrWhiteSpace(signature))
                throw new Exception("signature不能为空");

            string jwt = new JsonWebTokenHelper(_jwtOptions).GetJwt(applicationName, timeStamp, signature);

            return new ResponseMessage(jwt);
        }
    }
}