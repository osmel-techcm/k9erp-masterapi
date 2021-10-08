using masterCore.Entities;
using masterCore.Interfaces;
using masterInfrastructure.Data;
using masterInfrastructure.Helper;
using masterInfrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace masterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _contextApp;
        private readonly string _cn;

        private readonly IAspNetUsersGroupsService _aspNetUsersGroupsService;
        private readonly IConfigService _configService;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly ITwoFactorAuthService _twoFactorAuthService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, 
            AppDbContext contextApp, IAspNetUsersGroupsService aspNetUsersGroupsService, IConfigService configService, IAspNetUserService aspNetUserService, 
            ITwoFactorAuthService twoFactorAuthService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _contextApp = contextApp;
            _cn = _contextApp.Database.GetDbConnection().ConnectionString;
            _aspNetUsersGroupsService = aspNetUsersGroupsService;
            _configService = configService;
            _aspNetUserService = aspNetUserService;
            _twoFactorAuthService = twoFactorAuthService;
        }

        [HttpPost("Login")]
        public async Task<responseData> Login([FromBody] BasicUser user)
        {
            var messageResult = new responseData();

            var loginResults = new JObject();

            try
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.Users.SingleOrDefaultAsync(r => r.Email == user.Email || r.UserName == user.Email);
                    if (appUser == null)
                    {
                        messageResult.error = true;
                        messageResult.description = "User not found!";
                        return messageResult;
                    }

                    if (appUser.masterDealer == null || appUser.masterDealer == false)
                    {
                        messageResult.error = true;
                        messageResult.description = "User not found!";
                        return messageResult;
                    }

                    messageResult.data = await GenerateJwtToken(user.Email, appUser);

                    return messageResult;
                }
                else
                {
                    messageResult.error = true;
                    messageResult.description = "Invalid Credentials!";
                    return messageResult;
                }
            }
            catch (Exception e)
            {
                messageResult.error = true;
                messageResult.description = e.Message;
                messageResult.data = e;
                return messageResult;
            }
        }

        [Authorize]
        [HttpGet("RefreshToken")]
        public async Task<responseData> RefreshToken()
        {
            var messageResult = new responseData();
            var currentEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;
            if (currentEmail == null)
            {
                currentEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            }

            var appUser = await _userManager.Users.SingleOrDefaultAsync(r => r.Email == currentEmail || r.UserName == currentEmail && r.masterDealer == true);
            if (appUser == null)
            {
                messageResult.error = true;
                messageResult.description = "User not found!";
                return messageResult;
            }

            messageResult.data = await GenerateJwtToken(currentEmail, appUser);

            return messageResult;
        }

        [Authorize]
        [HttpGet("RefreshTokenTenant")]
        public async Task<responseData> RefreshTokenTenant()
        {
            var messageResult = new responseData();
            var currentEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;
            if (currentEmail == null)
            {
                currentEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            }

            var appUser = await _userManager.Users.SingleOrDefaultAsync(r => r.Email == currentEmail || r.UserName == currentEmail && r.masterDealer == false);
            if (appUser == null)
            {
                messageResult.error = true;
                messageResult.description = "User not found!";
                return messageResult;
            }

            var tenantKey = HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "x-tenant-id");
            if (tenantKey.Key == null)
            {
                messageResult.error = true;
                messageResult.description = "User not found!";
                return messageResult;
            }
            

            messageResult.data = await GenerateJwtTokenTenant(currentEmail, appUser, tenantKey.Value.ToString());

            return messageResult;
        }

        [Authorize]
        [HttpPost("Register")]
        public async Task<responseData> Register([FromBody] BasicUser model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                LastName = model.LastName,
                masterDealer = true,
                IdGroup = model.IdGroup
            };

            var messageResult = new responseData
            {
                data = user
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                messageResult.data = user;
                return messageResult;
            }
            else
            {
                messageResult.error = true;
                messageResult.description = "Bad Request";
                messageResult.othersValidations = result;
                return messageResult;
            }
        }

        private async Task<string> GenerateJwtToken(string email, IdentityUser user)
        {
            var userdb = await _contextApp.AspNetUsers.FirstOrDefaultAsync(x => x.Id == user.Id);

            var claims = new List<Claim>
            {
                new Claim("Email", email),
                new Claim("Id", user.Id),
                new Claim("Name", userdb.Name),
                new Claim("LastName", userdb.LastName),
                new Claim("IdGroup", userdb.IdGroup.ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("LoginTenant")]
        public async Task<responseData> LoginTenant([FromBody] BasicUser user)
        {
            var messageResult = new responseData();

            var loginResults = new JObject();

            try
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

                if (result.Succeeded)
                {
                    var appUser = _userManager.Users.SingleOrDefault(r => r.Email == user.Email || r.UserName == user.Email);
                    if (appUser == null)
                    {
                        messageResult.error = true;
                        messageResult.description = "User not found!";
                        return messageResult;
                    }

                    var defaultTenants = await _contextApp.UsersTenantsRelations.Where(x => x.UserId == appUser.Id).ToListAsync();
                    if (defaultTenants.Count() == 0)
                    {
                        messageResult.error = true;
                        messageResult.description = "There is no tenant associated to this user, contact an administrator!";
                        return messageResult;
                    }
                    else if (defaultTenants.Count() == 1)
                    {
                        var tenantSel = await _contextApp.Tenants.FirstOrDefaultAsync(x => x.Id == defaultTenants.FirstOrDefault().TenantId);
                        loginResults.Add("multiTenant", false);
                        loginResults.Add("twoFactor", appUser.TwoFactorEnabled);

                        var tenants = new JObject
                        {
                            { "tenantId", tenantSel.Id },
                            { "tenantName", tenantSel.TenantName },
                            { "ConnectionString", tenantSel.ConnectionString },
                            { "tenantToken", await GenerateJwtTokenTenant(user.Email, appUser, tenantSel.ConnectionString) }
                        };

                        var jResult = new JArray
                        {
                            tenants
                        };
                        loginResults.Add("tenants", jResult);
                        messageResult.data = loginResults;
                        return messageResult;
                    }
                    else
                    {
                        var jResult = new JArray();
                        loginResults.Add("multiTenant", true);
                        loginResults.Add("twoFactor", appUser.TwoFactorEnabled);

                        foreach (var tenant in defaultTenants)
                        {
                            var tenantSel = await _contextApp.Tenants.FirstOrDefaultAsync(x => x.Id == tenant.TenantId);
                            var tenants = new JObject
                            {
                                { "tenantId", tenantSel.Id },
                                { "tenantName", tenantSel.TenantName },
                                { "ConnectionString", tenantSel.ConnectionString },
                                { "tenantToken", await GenerateJwtTokenTenant(user.Email, appUser, tenantSel.ConnectionString) }
                            };
                            jResult.Add(tenants);
                        }
                        loginResults.Add("tenants", jResult);
                        messageResult.data = loginResults;
                        return messageResult;
                    }
                }
                else
                {
                    messageResult.error = true;
                    messageResult.description = "Invalid Credentials!";
                    return messageResult;
                }
            }
            catch (Exception e)
            {
                messageResult.error = true;
                messageResult.description = e.Message;
                messageResult.data = e;
                return messageResult;
            }
        }

        [Authorize]
        [HttpPost("RegisterTenant")]
        public async Task<responseData> RegisterTenant([FromBody] BasicUser model)
        {
            var messageResult = new responseData();

            var idCustomer = HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "x-customer-id");
            if (idCustomer.Key == null || string.IsNullOrEmpty(idCustomer.Value))
            {
                messageResult.error = true;
                messageResult.description = "Missing customer!";
                return messageResult;
            }

            int idCust = Convert.ToInt32(idCustomer.Value.ToString());

            var dbCustomer = _contextApp.Customers.FirstOrDefault(x => x.Id == idCust);
            if (dbCustomer == null)
            {
                messageResult.error = true;
                messageResult.description = "Customer not found!";
                return messageResult;
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                LastName = model.LastName,
                masterDealer = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                try
                {
                    var tenantsByCustomer = _contextApp.Tenants.Where(x => x.Customer == dbCustomer.Id);
                    foreach (var tenant in tenantsByCustomer)
                    {
                        var newTenantRel = new UsersTenantsRelation
                        {
                            UserId = user.Id,
                            TenantId = tenant.Id
                        };
                        _contextApp.UsersTenantsRelations.Add(newTenantRel);
                    }

                    await _contextApp.SaveChangesAsync();

                    return messageResult;
                }
                catch (Exception e)
                {
                    await _userManager.DeleteAsync(user);

                    messageResult.error = true;
                    messageResult.description = e.Message;
                    messageResult.data = e;
                    return messageResult;
                }
            }
            else
            {
                messageResult.error = true;
                messageResult.description = "Bad Request";
                messageResult.data = result;
                return messageResult;
            }

        }

        private async Task<string> GenerateJwtTokenTenant(string email, IdentityUser user, string idTenant)
        {

            var userdb = await _contextApp.AspNetUsers.FirstOrDefaultAsync(x => x.Id == user.Id);
            var tokenStr = string.Empty;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, userdb.Name),
                new Claim("LastName", userdb.LastName),
                new Claim("IdGroup", userdb.IdGroup.ToString()),
                new Claim(ClaimTypes.Role, "Tenant")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKeyTenant"] + "-" + idTenant));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudienceTenant"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            token.Header.Add("kid", idTenant);

            tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            if (user.TwoFactorEnabled)
            {
                tokenStr = await _twoFactorAuthService.EncriptToken(tokenStr, idTenant);
            }

            return tokenStr;
        }

        [HttpGet]
        [Route("serverStatusAlive")]
        public bool serverStatusAlive() {
            return true;
        }

        [HttpGet("getDateTimeServer")]
        public async Task<DateTime> getDateTimeServer()
        {
            var serverDate = new DateTime();

            var dateTime = await _contextApp.Database.AdoSqlQueryAsync(_cn, @"SELECT dbo.getGTMDateTimeFromUTC()[dateTime]");

            DateTime.TryParse(dateTime.FirstOrDefault()["dateTime"].ToString(), out serverDate);

            return serverDate;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("dealerIsRegistered")]
        public async Task<responseData> dealerIsRegisteredAsync()
        {
            var responseData = new responseData();

            try
            {
                var configRes = await _configService.GetConfigByName("IsRegistered");
                if (configRes.error)
                {
                    return configRes;
                }
                responseData.data = (Config)configRes.data;
            }
            catch (Exception e)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = e.Message;
                responseData.data = e.InnerException;
            }

            return responseData;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("registerDealer")]
        public async Task<responseData> registerDealerAsync(Register register)
        {
            var responseData = new responseData();
            var config = new Config();
            var userGroup = new AspNetUsersGroup();
            var applicationUser = new ApplicationUser();

            var isRegistered = await dealerIsRegisteredAsync();
            if (isRegistered.error)
            {
                return isRegistered;
            }

            if (((Config)isRegistered.data).propValue == "True")
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "The dealer is already registered!";
                return responseData;
            }

            if (!Utils.isValidEmailAddress(register.email))
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "Invalid Registered email!";
            }

            if (string.IsNullOrEmpty(register.group))
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "Administrator Group can not be empty!";
            }

            if (!Utils.isValidEmailAddress(register.email))
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "Invalid email!";
            }

            if (string.IsNullOrEmpty(register.password))
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "Password can not be empty!";
            }

            if (string.IsNullOrEmpty(register._password))
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "Repeat Password can not be empty!";
            }

            if (register.password != register._password)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "The Password does not match!";
            }

            try
            {
                userGroup = new AspNetUsersGroup
                {
                    name = register.group,
                    administrator = true,
                    inactive = false,
                    maximumDiscount = 0
                };

                var addUserGroup = await _aspNetUsersGroupsService.PostAspNetUsersGroup(userGroup);

                if (addUserGroup.error)
                {
                    await ReverseRegisterDealer(userGroup, applicationUser, config);
                    return addUserGroup;
                }

                var basicUser = new BasicUser
                {
                    Email = register.email,
                    Password = register.password,
                    Name = register.email,
                    LastName = register.email,
                    IdGroup = ((AspNetUsersGroup)addUserGroup.data).id
                };

                responseData = await Register(basicUser);
                applicationUser = (ApplicationUser)responseData.data;

                if (responseData.error)
                {                    
                    await ReverseRegisterDealer(userGroup, applicationUser, config);
                    return responseData;
                }

                var configResponse = await _configService.GetConfigByName("IsRegistered");
                if (configResponse.error)
                {
                    await ReverseRegisterDealer(userGroup, applicationUser, config);
                    return configResponse;
                }

                config = (Config)configResponse.data;
                config.propValue = "True";
                var configResult = await _configService.PutConfig(config.Id, config);

                if (configResult.error)
                {
                    await ReverseRegisterDealer(userGroup, applicationUser, config);
                    return configResult;
                }
            }
            catch (Exception e)
            {
                await ReverseRegisterDealer(userGroup, applicationUser, config);
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = e.Message;
                responseData.data = e.InnerException;
            }

            return responseData;
        }

        private async Task ReverseRegisterDealer(AspNetUsersGroup userGroup, ApplicationUser applicationUser, Config config) 
        {
            if (userGroup != null && userGroup.id != 0)
            {
                await _aspNetUsersGroupsService.DeleteAspNetUsersGroup(userGroup.id);
            }
            if (applicationUser != null && !string.IsNullOrEmpty(applicationUser.Id))
            {
                await _userManager.DeleteAsync(applicationUser);
            }
            if (config != null && config.Id != 0)
            {
                config.propValue = "False";
                await _configService.PutConfig(config.Id, config);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getUserProfile")]
        public async Task<responseData> getUserProfile() 
        {
            var currentUserId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUserService.GetUserByTenant(currentUserId, currentUserId);
        }

        [HttpPost]
        [Route("validateOTP")]
        public async Task<responseData> validateOTP(string code, dynamic tenantData)
        {
            var responseData = new responseData();
            ClaimsPrincipal resultValidateToken = new ClaimsPrincipal();

            try
            {
                var tenants = tenantData.tenants;
                foreach (var tenant in tenants)
                {
                    tenant.tenantToken = await _twoFactorAuthService.DecriptToken(tenant.tenantToken.ToString(), tenant.ConnectionString.ToString());
                    resultValidateToken = validateToken(tenant.tenantToken.ToString(), tenant.ConnectionString.ToString());
                    if (resultValidateToken == null)
                    {
                        responseData.error = true;
                        responseData.errorValue = 2;
                        responseData.description = "Invalid Token!";
                        return responseData;
                    }
                }

                responseData = await _twoFactorAuthService.ValidateTwoFactorAuth(resultValidateToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value, code);
                if (responseData.error)
                {
                    return responseData;
                }

                if (!(bool)responseData.data)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Invalid OTP!";
                    return responseData;
                }

                responseData.data = tenants;

                return responseData;
            }
            catch (Exception e)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = e.Message;
                responseData.data = e;
                return responseData;
            }
        }

        private ClaimsPrincipal validateToken(string token, string tenantId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKeyTenant"] + "-" + tenantId));

            var validationParameters = new TokenValidationParameters()
            {
                ValidIssuer = _configuration["JwtIssuer"],
                ValidAudiences = new List<string> { _configuration["JwtAudienceTenant"], _configuration["JwtAudience"] },
                IssuerSigningKey = securityKey
            };

            SecurityToken validatedToken;
            var handler = new JwtSecurityTokenHandler();
            try
            {                
                return handler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (SecurityTokenException e)
            {
                return null;
            }
        }
    }
}