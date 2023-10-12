using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace Chat_Authentication
{
    public class Authentication
    {
        // Generate token
        public static string GenerateJwtToken(string username)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, username)

        };

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CA1DFDASCF4DSAJHASDKFHADF661ADFAA55CA4"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var expires = DateTime.Now.AddDays(Convert.ToDouble(Convert.ToString(ConfigurationManager.AppSettings["config:JwtExpireDays"])));
            var expires = DateTime.Now.AddDays(Convert.ToDouble("30"));

            var token = new JwtSecurityToken(
                //Convert.ToString(ConfigurationManager.AppSettings["config:JwtIssuer"]),
                "https://localhost:44307",
               //Convert.ToString(ConfigurationManager.AppSettings["config:JwtAudience"]),
               "SecureApiUser",
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Validate the token
        public static string ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"]));
            var key = Encoding.ASCII.GetBytes("CA1DFDASCF4DSAJHASDKFHADF661ADFAA55CA4");
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var jti = jwtToken.Claims.First(claim => claim.Type == "jti").Value;
                var userName = jwtToken.Claims.First(sub => sub.Type == "sub").Value;

                // return user id from JWT token if validation successful
                return userName;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}