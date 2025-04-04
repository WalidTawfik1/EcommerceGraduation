using EcommerceGraduation.Core.DTO;
using EcommerceGraduation.Core.Entities;
using EcommerceGraduation.Core.Interfaces;
using EcommerceGraduation.Core.Services;
using EcommerceGraduation.Core.Sharing;
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

        public AuthenticationRepository(UserManager<Customer> userManager, IEmailService emailService, SignInManager<Customer> signInManager, IGenerateToken generateToken)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.generateToken = generateToken;
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

                // send email activiation
                string token = await userManager.GenerateEmailConfirmationTokenAsync(customer);
                await SendEmail(customer.Email, token, "Active", "Email Activation", "Please Active your email, click on button to active");
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

        public async Task SendEmail(string email, string code, string component, string subject, string message)
        {
            var result = new EmailDTO (email,
                "maximlev643@gmail.com",
                subject,
                EmailStringBody.send(email,code, component, message));
            await emailService.SendEmailAsync(result);
        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return null;
            }
            var customer = await userManager.FindByEmailAsync(loginDTO.Email);
            
            if(!customer.EmailConfirmed)
            {
                string token = await userManager.GenerateEmailConfirmationTokenAsync(customer);
                await SendEmail(customer.Email, token, "Active", "Email Activation", "Please Active your email, click on button to active");
                return "Please Active your email first, we have sent the acivation link to your email";

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
            string token = await userManager.GeneratePasswordResetTokenAsync(customer);
            await SendEmail(customer.Email, token, "ResetPassword", "Reset Password", "Please click on the button to reset your password");
            return true;
        }

        public async Task<string> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var customer = await userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (customer == null)
            {
                return "Invalid Email";
            }
            var result = await userManager.ResetPasswordAsync(customer, resetPasswordDTO.Token, resetPasswordDTO.Password);
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
            var result = await userManager.ConfirmEmailAsync(customer, activeAccountDTO.Token);
            if (result.Succeeded)
            {
                return true;
            }
            var token = await userManager.GenerateEmailConfirmationTokenAsync(customer);
            await SendEmail(customer.Email, token, "Active", "Email Activation", "Please Active your email, click on button to active");
            return false;
        }
    }
}