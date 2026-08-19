using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PickleChic.API;
using PickleChic.API.DTOs;
using PickleChic.API.Utilities;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PickleChic.API.Controllers
{

    [Route("Access")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        CustomerRepository _customerRepository;
        StaffRepository _staffRepository;
        AddressRepository _addressRepository;

        private readonly IConfiguration _configuration;
        private readonly TimeZoneInfo _gmtPlus7 = TimeZoneInfo.CreateCustomTimeZone("GMT+7", TimeSpan.FromHours(7), "GMT+7", "GMT+7");

        public AccessController(
            IConfiguration configuration,
            CustomerRepository customerRepository,
            StaffRepository staffRepository,
            AddressRepository addressRepository)
        {
            _configuration = configuration;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
            _addressRepository = addressRepository;
        }

        [HttpPost("LoginCustomer")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            UtilityFunc gf = new UtilityFunc();
            string a = gf.HashPassword("Amin12345@");
            string username = loginModel.Username;
            string passwordHash = loginModel.PasswordHash;
            _customerRepository = new CustomerRepository();
            PickleChic.DAL.Models.Customer customer = _customerRepository.GetByKeyAndPassword(username, passwordHash).Result;
            if (customer != null)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginModel.Username),
                  new Claim(ClaimTypes.SerialNumber, customer.Id.ToString()),
                new Claim(ClaimTypes.Role, "Customer"),
                 new Claim(ClaimTypes.Email, customer.Email),
                  new Claim(ClaimTypes.Name, customer.FullName),
                     new Claim(ClaimTypes.MobilePhone, customer.PhoneNumber),
                      new Claim(ClaimTypes.Surname, customer.RankId.ToString()),
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var gmtPlus7Now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _gmtPlus7);
                var expirationGmt7 = gmtPlus7Now.AddMinutes(50);
                var expirationUtc = TimeZoneInfo.ConvertTimeToUtc(expirationGmt7, _gmtPlus7);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Issuer"],
                    claims: claims,
                    expires: expirationUtc,
                    signingCredentials: creds
                );

                return Ok(new LoginResponseDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expirationGmt7,
                    LoginSuccess = true,
                    FirstLogin = customer.LastLogin == null,
                });
            }
            else
            {
                return StatusCode(401, "Unauthorized");
            }
        }

        [HttpPost("LoginStaff")]
        public IActionResult LoginStaff([FromBody] LoginModel loginModel)
        {
            string username = loginModel.Username;
            string passwordHash = loginModel.PasswordHash;
            _staffRepository = new StaffRepository();
            Staff staff = _staffRepository.GetByKeyAndPassword(username, passwordHash).Result;
            if (staff != null)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginModel.Username),
                   new Claim(ClaimTypes.SerialNumber, staff.Id.ToString()),
                new Claim(ClaimTypes.Role, staff.RoleId.ToString()),
                 new Claim(ClaimTypes.Name, staff.UserName)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var gmtPlus7Now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _gmtPlus7);
                var expirationGmt7 = gmtPlus7Now.AddMinutes(180);
                var expirationUtc = TimeZoneInfo.ConvertTimeToUtc(expirationGmt7, _gmtPlus7);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Issuer"],
                    claims: claims,
                    expires: expirationUtc,
                    signingCredentials: creds
                );

                return Ok(new LoginResponseDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expirationGmt7,
                    LoginSuccess = true,
                    FirstLogin = staff.LastLogin == null
                });
            }
            else
            {
                return StatusCode(401, "Unauthorized");
            }
        }

        [HttpGet("Check")]
        [Authorize]
        public async Task<IActionResult> GetSecureData()
        {
            try
            {
                DateTime? expirationTime = null;
                bool isExpired = false;
                RankRepository rankRepository = new RankRepository();

                var authHeader = Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Replace("Bearer ", "");
                    if (!string.IsNullOrEmpty(token))
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jsonToken = handler.ReadJwtToken(token);
                        var expirationUtc = jsonToken.ValidTo;
                        expirationTime = TimeZoneInfo.ConvertTimeFromUtc(expirationUtc, _gmtPlus7);
                        isExpired = expirationUtc < DateTime.UtcNow;

                        if (isExpired)
                        {
                            return StatusCode(401, "TokenExpired");
                        }
                    }
                }
                int rankId = string.IsNullOrEmpty(User.FindFirst(ClaimTypes.Surname)?.Value) ? -1 : int.Parse(User.FindFirst(ClaimTypes.Surname)?.Value);
                string rankName = string.Empty;
                if (rankId != -1)
                {
                    rankName = rankRepository.GetByIdAsync(rankId).Result.RankName;
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
                var user = await _customerRepository.GetByIdAsync(userId);

                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                int targetRoleId = -1;
                if (roleClaim == "Customer")
                {
                    targetRoleId = 2;
                }
                else if (roleClaim != null && int.TryParse(roleClaim, out int roleId))
                {
                    targetRoleId = roleId;
                }

                List<PagePermissionDTO> pagePermissions = new List<PagePermissionDTO>();
                if (targetRoleId != -1)
                {
                    using (var context = new PickleChicDbContext())
                    {
                        var role = await context.Roles.FindAsync(targetRoleId);
                        if (role != null && !string.IsNullOrWhiteSpace(role.Permissions))
                        {
                            try
                            {
                                pagePermissions = System.Text.Json.JsonSerializer.Deserialize<List<PagePermissionDTO>>(
                                    role.Permissions,
                                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                                ) ?? new List<PagePermissionDTO>();
                            }
                            catch
                            {
                               
                            }
                        }
                    }
                }

                var userInfo = new
                {
                    id = userId,
                    username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    role = User.FindFirst(ClaimTypes.Role)?.Value,
                    email = User.FindFirst(ClaimTypes.Email)?.Value,
                    fullName = User.FindFirst(ClaimTypes.Name)?.Value,
                    phoneNumber = User.FindFirst(ClaimTypes.MobilePhone)?.Value,
                    rankId = User.FindFirst(ClaimTypes.Surname)?.Value,
                    rankName = rankName,
                    totalpoints = user != null ? user.TotalPoints : 0,
                    expirationTime = expirationTime,
                    isExpired = isExpired,
                    pagePermissions = pagePermissions
                };

                return Ok(userInfo);
            }
            catch (SecurityTokenExpiredException)
            {
                return StatusCode(403, "TokenExpired");
            }
            catch
            {
                return StatusCode(403, "InvalidToken");
            }
        }

        [HttpPost("customer-register")]
        public async Task<ActionResult<bool>> CustomerRegister([FromBody] RegisterModel registerModel)
        {
            var existingByEmail = await _customerRepository.FindUserExistByKeyWord(registerModel.Email);
            var existingByUsername = await _customerRepository.FindUserExistByKeyWord(registerModel.UserName);
            if (existingByEmail != null || existingByUsername != null)
            {
                return BadRequest("EmailOrUsernameAlreadyExit");
            }

            var newUser = new Customer
            {
                Username = registerModel.UserName,
                FullName = registerModel.FullName,
                Email = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber,
                PasswordHash = registerModel.PasswordHash,
                TotalPoints = 0,
                RankId = 1,
                Status = 1,
                LastLogin = DateTime.Now,
                DateOfBirth = registerModel.DateOfBirth,
                Gender = registerModel.Gender
            };

            var customerResult = await _customerRepository.AddAsync(newUser);

            if (customerResult == null)
            {
                return StatusCode(500, "DatabaseError");
            }

            await _addressRepository.EnsureSystemPickupAsync(customerResult.Id);

            return Ok(true);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordModel resetPasswordModel)
        {
            if (string.IsNullOrEmpty(resetPasswordModel.Email))
            {
                return BadRequest("EmailOrUsernameRequired");
            }

            var customer = await _customerRepository.FindUserByEmailAndPhoneAndUserName(resetPasswordModel.Email, string.Empty, string.Empty);

            if (customer == null)
            {
                return BadRequest("Notfound");
            }

            UtilityFunc utilityFunc = new UtilityFunc();
            string newPassword = utilityFunc.GenerateRandomString(6);
            //string newPassword = "Customer12345@";

            customer.PasswordHash = utilityFunc.HashPassword(newPassword);
            customer.LastLogin = null;

            var updatedCustomer = await _customerRepository.UpdateAsync(customer);

            if (updatedCustomer == null)
            {
                return StatusCode(500, "OtherError");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append($"Kính chào quý khách hàng <b>{customer.FullName}</b><br><br>Mật khẩu truy cập vào tài khoản PickleChic Store đã được thay đổi thành <b>{newPassword} </b> Vui lòng truy cập trang web và thay đổi mật khẩu, xin trân trọng cám ơn!<br><br>Đội ngũ PickleChic");
            if (updatedCustomer == null)
            {
                return StatusCode(500, "OtherError");
            }
            else
            {
                bool re = await utilityFunc.SendEmailToAddress(customer.Email, customer.FullName, "Khôi phục mật khẩu tài khoản PickleChic", "", sb.ToString());
            }

            return Ok(true);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<bool>> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO.CurrentPassword == changePasswordDTO.NewHashPassword)
            {
                return BadRequest("PasswordIsTheSame");
            }

            string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userName == null && email == null)
            {
                return BadRequest("Not found");
            }
            else
            {
                var customer = await _customerRepository.FindUserByEmailAndPhoneAndUserName(email, string.Empty, userName);

                if (customer == null)
                {
                    return NotFound("CustomerNotFound");
                }

                if (customer.PasswordHash != changePasswordDTO.CurrentPassword)
                {
                    return BadRequest("CurrentPasswordNotMatch");
                }

                customer.PasswordHash = changePasswordDTO.NewHashPassword;
                if (customer.LastLogin == null)
                {
                    customer.LastLogin = DateTime.Now;
                }
                var updatedCustomer = await _customerRepository.UpdateAsync(customer);

                if (updatedCustomer == null)
                {
                    return StatusCode(500, "OtherError");
                }

                return Ok(true);
            }
        }

        [HttpPost("staff-change-password")]
        public async Task<ActionResult<bool>> StaffChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO.CurrentPassword == changePasswordDTO.NewHashPassword)
            {
                return BadRequest("PasswordIsTheSame");
            }

            string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userName == null)
            {
                return BadRequest("StaffNotFound");
            }
            else
            {
                var staff = await _staffRepository.GetByKeyAndPassword(userName, changePasswordDTO.CurrentPassword);
                if (staff == null)
                {
                    return NotFound("CurrentPasswordNotMatch");
                }
                staff.PasswordHash = changePasswordDTO.NewHashPassword;
                if (staff.LastLogin == null)
                {
                    staff.LastLogin = DateTime.Now;
                }
                var updatedStaff = await _staffRepository.UpdateAsync(staff);
                if (updatedStaff == null)
                {
                    return StatusCode(500, "OtherError");
                }
                return Ok(true);
            }
        }




    }
}
