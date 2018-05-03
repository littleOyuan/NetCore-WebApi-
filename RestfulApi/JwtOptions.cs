using System.Collections.Generic;

namespace RestfulApi
{
    public class JwtOptions
    {
        public string IssUser { get; set; }

        public int TokenExpiryCycle { get; set; }

        public List<Application> Applications { get; set; }
    }

    public class Application
    {
        public string ApplicationName { get; set; }

        public string VerifyCode { get; set; }

        public string Secretkey { get; set; }

        public int SignatureExpiryCycle { get; set; }
    }
}
