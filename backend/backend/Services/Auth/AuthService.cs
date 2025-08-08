﻿using backend.Interface.Auth;
using backend.ModelDTO.Auth.AuthRespond;
using backend.ModelDTO.Auth.AuthRequest;
using backend.Model.Auth;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;
using System.Net.WebSockets;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using System.Reflection.PortableExecutable;
using backend.Enum;
using backend.Helper;
using backend.Model.Staff_Customer;
using backend.ModelDTO.GenericRespond;
using Microsoft.AspNetCore.Authorization;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using BCrypt.Net;

namespace backend.Services.Auth
{
    public class AuthService : IAuth
    {
        private readonly DataContext _dataContext;

        private readonly IConfiguration _configuration;
        
        private readonly HashHelper _hashHelper;

        public AuthService(DataContext dataContext, IConfiguration _configuration , HashHelper hashHelper)
        {
            _dataContext = dataContext;
            this._configuration = _configuration;
            _hashHelper = hashHelper;
        }

        [AllowAnonymous]
        public async Task<registerRespondDTO> Register(registerRequestDTO registerRequest)
        {
            var getCustomerRoleID = _dataContext.roleInformation.FirstOrDefault(x => x.roleName.Equals("Customer"));

            if (getCustomerRoleID != null)
            {
                var findExitsEmail = _dataContext.userInformation.FirstOrDefault(x => x.loginUserEmail.ToLower()
                    .Equals(registerRequest.loginEmail.ToLower()));
                if (findExitsEmail != null)
                {
                    return new registerRespondDTO()
                    {
                        message = "Lỗi Email Đã bị trùng",
                        statusCode = StatusCodes.Status400BadRequest,
                    };
                }
                var Transition =  await _dataContext.Database.BeginTransactionAsync();
                try
                {
                    Guid userID = Guid.NewGuid();
                    var BryptPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.loginUserPassword);
                    var HashIdentityCode = _hashHelper.Hash(registerRequest.IdentityCode);
                    var newUserInformarion = new userInformation()
                    {
                        userId = userID.ToString(),
                        loginUserEmail = registerRequest.loginEmail,
                        loginUserPassword = BryptPassword
                    };
                    string CustomerID = Guid.NewGuid().ToString();
                    var newCustomerInfo = new Customer()
                    {
                        dateOfBirth = registerRequest.dateOfBirth,
                        IdentityCode = HashIdentityCode,
                        Id = CustomerID,
                        Name = registerRequest.userName,
                        userID = userID.ToString(),
                        phoneNumber = registerRequest.phoneNumber,
                    };
                    await _dataContext.Customers.AddAsync(newCustomerInfo);
                    await _dataContext.userInformation.AddAsync(newUserInformarion);

                    var newUserRoleInformation = new userRoleInformation()
                    {
                        userId = userID.ToString(),
                        roleId = getCustomerRoleID.roleId
                    };
                    await _dataContext.userRoleInformation.AddAsync(newUserRoleInformation);
                    
                    await Transition.CommitAsync();
                    return new registerRespondDTO { statusCode = StatusCodes.Status201Created, message = "Đã tạo thành công" };
                }
                catch (Exception ex)
                {
                    await Transition.RollbackAsync();
                    return new registerRespondDTO { statusCode = StatusCodes.Status201Created, message = "Lỗi Database" };
                }
               
            }
            return new registerRespondDTO { statusCode = StatusCodes.Status400BadRequest, message = "Loi Nhap Thieu Truong Du Lieu" };
        }

        [AllowAnonymous]
        public loginRespondDTO Login(loginRequestDTO loginRequest)
        {
            try
            {
                var checkLoginRequest = checkLogin(loginRequest);
                if (checkLoginRequest != null)
                {
                    // Lấy ID
                    var getID = checkLoginRequest.userId;
                    // Lấy ID role trong bảng quan hệ n-n
                    var getRole = _dataContext.userRoleInformation.Where(x => x.userId.Equals(getID)).Select(x => x.roleId).ToList();
                    // Lấy RoleName
                    var getRoleList = _dataContext.roleInformation.Where(x => getRole.Contains(x.roleId)).Select(x => x.roleName).ToList();

                    // Tạo Claims để làm JWT
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name , loginRequest.loginUserName),
                    };

                    foreach (var roleName in getRoleList)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleName));
                    }

                    var getToken = generateToken(claims, checkLoginRequest.userId);

                    if (getToken != null)
                    {
                        return getToken;
                    }
                }
                return new loginRespondDTO()
                {
                    message = "Error"
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null!;
            }
        }

        private userInformation checkLogin(loginRequestDTO loginRequest)
        {
            try
            {
                var findUser = _dataContext.userInformation.FirstOrDefault
                (x => x.loginUserEmail.Equals(loginRequest.loginUserName));
                if (findUser != null)
                {
                    // Kiểm tra có đúng mk hay không
                    var checkPassword = BCrypt.Net.BCrypt.Verify(loginRequest.loginUserPassword, findUser.loginUserPassword);
                    if (checkPassword)
                    {
                        return findUser;
                    }
                    return null!;
                }
                return null!;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null!;
            }
        }

        private loginRespondDTO generateToken(List<Claim> claims , string Email)
        {
            var getJWTKey = _configuration["Jwt:Key"];

            /*
             * 
             */
            if (getJWTKey != null)
            {
                // Tạo header
                var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(getJWTKey));
                // Tạo header
                var SigningCreatical = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
                var Hour = DateTime.Now.AddHours(1);
                // Tạo JWT_Token
                var genrateTokenString = new JwtSecurityToken
                    (_configuration["Jwt:Iss"],
                    _configuration["Jwt:Aud"],
                    claims, 
                    DateTime.Now,
                    Hour, SigningCreatical
                    );

                var gettingToken = new JwtSecurityTokenHandler().WriteToken(genrateTokenString);

                var getToken = new JwtSecurityTokenHandler().ReadToken(gettingToken);

                var newAuthRepond = new loginRespondDTO()
                {
                    tokenID = gettingToken ,
                    userID = Email,
                    expDate = Hour.ToString(),
                    RoleName = String.Join("," , claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value)),
                    message = "Success"
                };
                return newAuthRepond;
            }
            return null! ;
        }

        public GenericRespondWithObjectDTO<Dictionary<string , string>> VerifyEmailCode(string EmailAddress ,string code)
        {
            if (String.IsNullOrEmpty(EmailAddress))
            {
                return new GenericRespondWithObjectDTO<Dictionary<string, string>>()
                {
                    Status = GenericStatusEnum.Failure.ToString(),
                    message = "Email Khong Duoc De Trong"
                };
            }
            if (String.IsNullOrEmpty(code))
            {
                return new GenericRespondWithObjectDTO<Dictionary<string , string>>()
                {
                    Status = GenericStatusEnum.Failure.ToString(),
                    message = "Ban Chua Nhap Ma Code"
                };
            }
            
            var checkEmailAddress = _dataContext.userInformation.FirstOrDefault
                (x => x.loginUserEmail.Equals(EmailAddress));
            if (checkEmailAddress != null)
            {
                var checkOTP = _dataContext.EmailList.FirstOrDefault(x =>
                    x.UserId.Equals(checkEmailAddress.userId) && !x.isUsed && DateTime.Now < x.ExpirationDate);
                if (checkOTP != null)
                {
                    // Verify OTP 
                    var VerifyOTP = _hashHelper.GetData(checkOTP.EmailCode);
                    if (String.IsNullOrEmpty(VerifyOTP))
                    {
                        return new GenericRespondWithObjectDTO<Dictionary<string , string>>()
                        {
                            Status = GenericStatusEnum.Failure.ToString(),
                            message = "Khong Tim Thay Ma OTP"
                        };
                    }

                    if (VerifyOTP == code)
                    {
                        try
                        {
                            checkOTP.ResetToken = Guid.NewGuid().ToString();
                            _dataContext.EmailList.Update(checkOTP);
                            _dataContext.SaveChanges();

                            return new GenericRespondWithObjectDTO<Dictionary<string , string>>()
                            {
                                Status = GenericStatusEnum.Success.ToString(),
                                message = "Email Code Verified",
                                data = new Dictionary<string, string>()
                                {
                                    {"Token" , checkOTP.ResetToken }
                                }
                            };
                        }
                        catch(Exception e)
                        {
                            return new GenericRespondWithObjectDTO<Dictionary<string, string>>()
                            {
                                Status = GenericStatusEnum.Failure.ToString(),
                                message = "Loi Database" 
                            };
                        }
                    }
                }
            }

            return new GenericRespondWithObjectDTO<Dictionary<string, string>>()
            {
                Status = GenericStatusEnum.Failure.ToString(),
                message = "Khong Tim Thay Nguoi Dung"
            };
        }


        public async Task SaveChanges()
        {
            await _dataContext.SaveChangesAsync();
        }
    }
}
