using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RestfulApi
{
    public class JsonWebTokenHelper
    {
        private readonly JwtOptions _jwtOptions;

        public JsonWebTokenHelper(IOptions<JwtOptions> options)
        {
            _jwtOptions = options.Value;
        }

        public string GetJwt(string applicationName, long timeStamp, string signature)
        {
            ParametersVerification(applicationName, timeStamp, signature);

            DateTime dateTimeNow = DateTime.UtcNow;

            Claim[] claims = {
                new Claim(JwtRegisteredClaimNames.Iat, dateTimeNow.ToUniversalTime().ToString(),ClaimValueTypes.Integer64)
            };

            string symmetricKeyWithBase64 = _jwtOptions.Applications.First(system => system.ApplicationName.Equals(applicationName, StringComparison.OrdinalIgnoreCase)).Secretkey;

            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(symmetricKeyWithBase64));

            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(_jwtOptions.IssUser, applicationName, claims, dateTimeNow, dateTimeNow.AddMinutes(_jwtOptions.TokenExpiryCycle), signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private void ParametersVerification(string applicationName, long timeStamp, string signature)
        {
            if (!_jwtOptions.Applications.Any(app => app.ApplicationName.Equals(applicationName, StringComparison.OrdinalIgnoreCase)))
                throw new Exception($"应用编号：{applicationName}不存在");

            Application application = _jwtOptions.Applications.First(system => system.ApplicationName.Equals(applicationName, StringComparison.OrdinalIgnoreCase));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(applicationName);
            stringBuilder.Append(application.VerifyCode);
            stringBuilder.Append(timeStamp);

            string md5EncryptStr = GetMd5(stringBuilder.ToString(), Encoding.UTF8);

            bool isSignaturelegal = Convert.ToBase64String(Encoding.UTF8.GetBytes(md5EncryptStr)).Equals(signature, StringComparison.Ordinal);

            if (!isSignaturelegal)
                throw new Exception($"签名{signature}无效");

            DateTime signatureTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local).AddSeconds(timeStamp);

            if (DateTime.Now.Subtract(signatureTime).TotalMinutes > application.SignatureExpiryCycle)
                throw new Exception($"签名{signature}过期");
        }

        private string GetMd5(string str, Encoding encoding)
        {
            string md5 = string.Empty;
            byte[] bytes = encoding.GetBytes(str);

            bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);

            for (int i = 0; i < bytes.Length; i++)
            {
                md5 += bytes[i].ToString("x").PadLeft(2, '0');
            }
            return md5;
        }
    }
}
