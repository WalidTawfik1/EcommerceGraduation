﻿using AutoMapper;
using EcommerceGraduation.API.Helper;
using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Infrastructure.Data;
using EcommerceGraduation.Infrastrucutre.Repositores;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceGraduation.API.Controllers
{
    /// <summary>
    /// Controller for managing user accounts.
    /// </summary>
    public class AccountController : BaseController
    {
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IGenerateToken _generateToken;
        private readonly EcommerceDbContext _context;
        public AccountController(IUnitofWork work, IMapper mapper, UserManager<Customer> userManager, SignInManager<Customer> signInManager, IConfiguration configuration, IGenerateToken generateToken, EcommerceDbContext context) : base(work, mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _generateToken = generateToken;
            _context = context;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">The registration details.</param>
        /// <returns>A response indicating the result of the registration.</returns>
        [AllowAnonymous]
        [HttpPost("register")]
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
        [AllowAnonymous]
        [HttpPost("login")]
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
        [AllowAnonymous]
        [HttpPost("active-account")]
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
        [AllowAnonymous]
        [HttpGet("send-email-forget-password")]
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
        [AllowAnonymous]
        [HttpPost("reset-password")]
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
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("token");
            return Ok(new APIResponse(200, "Logout Successfully"));
        }
        /// <summary>
        /// Gets the profile of the authenticated user.
        /// </summary>
        /// <returns>User profile</returns>
        [Authorize]
        [HttpGet("profile")]
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
        /// Deletes the account of the authenticated user unless he didn't make an order.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("delete-account")]
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

                return Ok(new APIResponse(200, result));
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
        [Authorize]
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] UpdateCustomerDTO customerDTO)
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
        /// <summary>
        /// Initiates Google login, Redirect automaticlly to google-response.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", null, Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }
        /// <summary>
        /// Handles the Google login response.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("Error loading external login information.");
            }

            // Look for the user in your application
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Google account did not provide an email.");
                }

                // Check if user already exists by email
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new Customer
                    {
                        UserName = info.Principal.Identity?.Name?.Replace(" ", "") ?? email,
                        Name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "Unknown",
                        Email = email,
                        EmailConfirmed = true,
                        UserType = "Customer",
                        CreatedAt = DateTime.Now,
                    };

                    var result = await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, "Customer");

                    if (!result.Succeeded)
                    {
                        return BadRequest(new { Message = "User creation failed.", Errors = result.Errors });
                    }
                }

                // Link Google login with user
                var loginResult = await _userManager.AddLoginAsync(user, info);
                if (!loginResult.Succeeded)
                {
                    return BadRequest(new { Message = "Failed to associate Google login with user.", Errors = loginResult.Errors });
                }
            }

            // Sign in the user using cookie authentication
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Generate JWT token
            var token = _generateToken.GetAndCreateToken(user);
            return Redirect($"https://graduation-project-smarket.vercel.app/google-bridge?token={token}");
        }

        /// <summary>
        /// Verifies the OTP code sent to the user's email.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OTPVerify request)
        {
            bool isValid = await work.Authentication.CheckOtpCode(request.Email, request.Code);

            if (isValid)
            {
                return Ok(new { success = true, message = "Verification code is valid" });
            }

            return BadRequest(new { success = false, message = "Invalid or expired verification code" });
        }

        /// <summary>
        /// Resends the OTP code to the user's email.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] OTPResend request)
        {
            bool result = await work.Authentication.ResendOtpCode(request.Email);

            if (result)
            {
                return Ok(new { success = true, message = "Verification code sent successfully" });
            }

            return BadRequest(new { success = false, message = "Failed to send verification code" });
        }

        [AllowAnonymous]
        [HttpPost("mobile-google-login")]
        public async Task<IActionResult> MobileGoogleLogin([FromBody] GoogleTokenDTO dto)
        {
            if (string.IsNullOrEmpty(dto.IdToken))
                return BadRequest("Invalid ID token.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string>
                {
                    _configuration["Authentication:Google:ClientId"]
                }
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
            }
            catch (InvalidJwtException ex)
            {
                return BadRequest("Invalid Google token: " + ex.Message);
            }


            var user = await _userManager.FindByLoginAsync("Google", payload.Subject);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new Customer
                    {
                        UserName = payload.Email.Split('@')[0],
                        Email = payload.Email,
                        Name = payload.Name,
                        EmailConfirmed = true,
                        UserType = "Customer",
                        CreatedAt = DateTime.Now
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return BadRequest(new { message = "User creation failed", errors = result.Errors });

                    await _userManager.AddToRoleAsync(user, "Customer");
                }

                var loginInfo = new UserLoginInfo("Google", payload.Subject, "Google");
                var loginResult = await _userManager.AddLoginAsync(user, loginInfo);
                if (!loginResult.Succeeded)
                    return BadRequest(new { message = "Google login association failed", errors = loginResult.Errors });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var token = _generateToken.GetAndCreateToken(user);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Email,
                    user.Name,
                    user.UserType,
                }
            });
        }


    }
}
