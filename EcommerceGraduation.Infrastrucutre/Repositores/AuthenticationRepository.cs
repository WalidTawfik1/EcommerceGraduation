using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Core.Sharing;
using EcommerceGraduation.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Infrastrucutre.Repositores
{
    public class AuthenticationRepository : IAuthentication
    {
        private readonly UserManager<Customer> userManager;
        private readonly IEmailService emailService;
        private readonly SignInManager<Customer> signInManager;
        private readonly IGenerateToken generateToken;
        private readonly EcommerceDbContext context;

        public AuthenticationRepository(UserManager<Customer> userManager, IEmailService emailService, SignInManager<Customer> signInManager, IGenerateToken generateToken, EcommerceDbContext context)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.generateToken = generateToken;
            this.context = context;
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            try
            {
                if (registerDTO == null)
                {
                    return null;
                }
                if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
                {
                    return "This email already exists";
                }
                var customer = new Customer
                {
                    Name = registerDTO.Name,
                    Email = registerDTO.Email,
                    UserName = registerDTO.Email.Split('@')[0],
                    Address = registerDTO.Address,
                    City = registerDTO.City,
                    Country = registerDTO.Country,
                    PostalCode = registerDTO.PostalCode,
                    PhoneNumber = registerDTO.PhoneNumber,
                    CreatedAt = DateTime.Now,
                    Gender = registerDTO.Gender,
                    DateOfBirth = registerDTO.DateOfBirth,
                    UserType = "Customer",
                };
                var result = await userManager.CreateAsync(customer, registerDTO.Password);
                if (!result.Succeeded)
                {
                    return string.Join(", ", result.Errors.Select(e => e.Description));
                }
                await userManager.AddToRoleAsync(customer, "Customer");

                // Generate OTP code instead of token
                string otpCode = GenerateCode.GenerateOtpCode(6);

                // Store OTP in database
                await StoreOtpForUser(customer.Email, otpCode);

                // Send OTP email
                await SendEmailWithOtp(customer.Email, otpCode, "Please verify your email by entering the code below in the app");

                return "User Created Successfully";
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? "No inner exception";
                // Log or return the innerMessage
                return $"Database error: {innerMessage}";
            }
            catch (Exception ex)
            {
                // Log or return ex.Message
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return null;
            }
            var customer = await userManager.FindByEmailAsync(loginDTO.Email);

            if (!customer.EmailConfirmed)
            {
                // Generate OTP code instead of token
                string otpCode = GenerateCode.GenerateOtpCode(6);

                // Store OTP in database
                await StoreOtpForUser(customer.Email, otpCode);

                // Send OTP email
                await SendEmailWithOtp(customer.Email, otpCode, "Please verify your email by entering the code below in the app");

                return "Please verify your email first. We have sent a verification code to your email";
            }

            var result = await signInManager.CheckPasswordSignInAsync(customer, loginDTO.Password, true);
            if (result.Succeeded)
            {
                return generateToken.GetAndCreateToken(customer);
            }
            return "Invalid Email or Password";
        }

        public async Task<bool> SendEmailForgetPassword(string email)
        {
            var customer = await userManager.FindByEmailAsync(email);
            if (customer == null)
            {
                return false;
            }

            // Generate OTP code instead of token
            string otpCode = GenerateCode.GenerateOtpCode(6);

            // Store OTP in database with purpose "ResetPassword"
            await StoreOtpForUser(customer.Email, otpCode, "ResetPassword");

            // Send OTP email
            await SendEmailWithOtp(customer.Email, otpCode, "Please use the code below to reset your password");

            return true;
        }

        public async Task<string> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var customer = await userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (customer == null)
            {
                return "Invalid Email";
            }

            // Verify OTP code
            var otpVerification = await context.OtpVerifications
                .Where(o => o.Email == resetPasswordDTO.Email && o.OtpCode == resetPasswordDTO.Code)
                .OrderByDescending(o => o.ExpirationTime)
                .FirstOrDefaultAsync();

            if (otpVerification == null)
            {
                return "Invalid verification code";
            }

            if (otpVerification.ExpirationTime < DateTime.UtcNow)
            {
                return "Verification code has expired";
            }

            // Generate token for password reset
            string token = await userManager.GeneratePasswordResetTokenAsync(customer);
            var result = await userManager.ResetPasswordAsync(customer, token, resetPasswordDTO.Password);

            if (result.Succeeded)
            {
                return "Password Reset Successfully";
            }
            return result.Errors.ToList()[0].Description;
        }

        public async Task<bool> ActiveAccount(ActiveAccountDTO activeAccountDTO)
        {
            var customer = await userManager.FindByEmailAsync(activeAccountDTO.Email);
            if (customer == null)
            {
                return false;
            }

            // Verify OTP code
            var otpVerification = await context.OtpVerifications
                .Where(o => o.Email == activeAccountDTO.Email && o.OtpCode == activeAccountDTO.Code && !o.IsUsed)
                .OrderByDescending(o => o.ExpirationTime)
                .FirstOrDefaultAsync();

            if (otpVerification == null)
            {
                return false;
            }

            if (otpVerification.ExpirationTime < DateTime.UtcNow)
            {
                // Generate new OTP if expired
                string otpCode = GenerateCode.GenerateOtpCode(6);
                await StoreOtpForUser(customer.Email, otpCode);
                await SendEmailWithOtp(customer.Email, otpCode, "Please verify your email by entering the code below in the app");
                return false;
            }

            // Mark OTP as used
            otpVerification.IsUsed = true;
            await context.SaveChangesAsync();

            // Confirm email
            string token = await userManager.GenerateEmailConfirmationTokenAsync(customer);
            var result = await userManager.ConfirmEmailAsync(customer, token);

            return result.Succeeded;
        }

        public async Task StoreOtpForUser(string email, string otpCode, string purpose = "VerifyEmail")
        {
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(10);

            var otpVerification = new OtpVerification
            {
                Email = email,
                OtpCode = otpCode,
                ExpirationTime = expirationTime,
                IsUsed = false,
                Purpose = purpose
            };

            context.OtpVerifications.Add(otpVerification);
            await context.SaveChangesAsync();
        }

        public async Task SendEmailWithOtp(string email, string otpCode, string message)
        {
            EmailDTO emailDTO = new EmailDTO
                (
                to: email,
                from: "smarket.ebusiness@gmail.com",
                subject: "SMarket Verification Code",
                content: EmailStringBody.send(email, otpCode, message)
                );

            await emailService.SendEmailAsync(emailDTO);
        }

        public async Task <bool> CheckOtpCode(string email, string otpCode)
        {
            // Verify OTP code
            var otpVerification = await context.OtpVerifications
                .Where(o => o.Email == email && o.OtpCode == otpCode && !o.IsUsed)
                .OrderByDescending(o => o.ExpirationTime)
                .FirstOrDefaultAsync();

            if (otpVerification == null)
            {
                return false;
            }

            if (otpVerification.ExpirationTime < DateTime.UtcNow)
            {
                return false;
            }

            otpVerification.IsUsed = true;
            await context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> ResendOtpCode(string email)
        {
            var customer = await userManager.FindByEmailAsync(email);
            if (customer == null)
            {
                return false;
            }

            // Determine purpose based on user status
            string purpose = "VerifyEmail";

            // Check if there's an existing OTP for password reset
            var existingPasswordResetOtp = await context.OtpVerifications
                .Where(o => o.Email == email && o.Purpose == "ResetPassword" && !o.IsUsed)
                .OrderByDescending(o => o.ExpirationTime)
                .FirstOrDefaultAsync();

            if (existingPasswordResetOtp != null && existingPasswordResetOtp.ExpirationTime > DateTime.UtcNow)
            {
                // If there's a valid password reset OTP, continue with that purpose
                purpose = "ResetPassword";
            }
            else if (customer.EmailConfirmed)
            {
                // If email is already confirmed, this is likely a password reset
                purpose = "ResetPassword";
            }

            // Generate new OTP
            string otpCode = GenerateCode.GenerateOtpCode(6);

            // Store OTP in database
            await StoreOtpForUser(email, otpCode, purpose);

            // Send appropriate message based on purpose
            string message = purpose == "ResetPassword"
                ? "Please use the code below to reset your password"
                : "Please verify your email by entering the code below in the app";

            // Send OTP email
            await SendEmailWithOtp(email, otpCode, message);

            return true;
        }

    }
}