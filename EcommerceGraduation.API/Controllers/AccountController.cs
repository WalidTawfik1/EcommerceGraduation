using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Infrastrucutre.Repositores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceGraduation.API.Controllers
{
    /// <summary>
    /// Controller for managing user accounts.
    /// </summary>
    public class AccountController : BaseController
    {
        public AccountController(IUnitofWork work, IMapper mapper) : base(work, mapper)
        {
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">The registration details.</param>
        /// <returns>A response indicating the result of the registration.</returns>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var result = await work.Authentication.RegisterAsync(registerDTO);
            if (result != "User Created Successfully")
            {
                return BadRequest(new APIResponse(400, result));
            }
            return Ok(new APIResponse(200, result));
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginDTO">The login details.</param>
        /// <returns>A response indicating the result of the login.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var result = await work.Authentication.LoginAsync(loginDTO);
            if (result.StartsWith("Please") || result.StartsWith("Invalid"))
            {
                return BadRequest(new APIResponse(400, result));
            }
            Response.Cookies.Append("token", result, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                IsEssential = true,
                Expires = DateTime.Now.AddDays(1)
            });
            return Ok(new APIResponse(200, result));
        }

        /// <summary>
        /// Activates a user account.
        /// </summary>
        /// <param name="activeAccountDTO">The activation details.</param>
        /// <returns>A response indicating the result of the activation.</returns>
        [HttpPost("ActiveAccount")]
        public async Task<IActionResult> ActiveAccount(ActiveAccountDTO activeAccountDTO)
        {
            var result = await work.Authentication.ActiveAccount(activeAccountDTO);
            return result ? Ok(new APIResponse(200, "Account Activated Successfully")) : BadRequest(new APIResponse(200, "Please activate your account"));
        }

        /// <summary>
        /// Sends an email for password reset.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>A response indicating the result of the email sending.</returns>
        [HttpGet("SendEmailForgetPassword")]
        public async Task<IActionResult> SendEmailForgetPassword(string email)
        {
            var result = await work.Authentication.SendEmailForgetPassword(email);
            return result ? Ok(new APIResponse(200, "Email Sent Successfully")) : BadRequest(new APIResponse(200, "Email Not Sent"));
        }

        /// <summary>
        /// Resets the user password.
        /// </summary>
        /// <param name="resetPasswordDTO">The password reset details.</param>
        /// <returns>A response indicating the result of the password reset.</returns>
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var result = await work.Authentication.ResetPassword(resetPasswordDTO);
            if (result != "Password Reset Successfully")
            {
                return BadRequest(new APIResponse(400, result));
            }
            return Ok(new APIResponse(200, result));
        }
        /// <summary>
        /// Logs out a user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("token");
            return Ok(new APIResponse(200, "Logout Successfully"));
        }
        /// <summary>
        /// Gets the profile of the authenticated user.
        /// </summary>
        /// <returns>User profile</returns>
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // The repository method will get the ID from the authenticated user
                var customerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("User not authenticated, Please login or register.");
                }

                var customer = await work.CustomerRepository.GetCustomerByIdAsync(customerId);
                if (customer == null)
                {
                    return NotFound($"Customer profile not found.");
                }

                return Ok(customer);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the account of the authenticated user.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var customerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("User not authenticated, Please login or register.");
                }

                var result = await work.CustomerRepository.DeleteCustomerAsync(customerId);
                if (!result)
                {
                    return NotFound($"Customer with ID {customerId} not found.");
                }

                return Ok(new APIResponse(200, "Account deleted successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Updates the profile of the authenticated user.
        /// </summary>
        /// <param name="customerDTO"></param>
        /// <returns></returns>
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] CustomerDTO customerDTO)
        {
            try
            {
                var customerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("User not authenticated, Please login or register.");
                }

                var updatedCustomer = await work.CustomerRepository.UpdateCustomerAsync(customerId, customerDTO);
                return Ok(updatedCustomer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
