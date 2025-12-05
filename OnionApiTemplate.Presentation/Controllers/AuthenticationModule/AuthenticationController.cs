using Khazen.Application.DOTs.Auth;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ChangePassword;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.FogetPassword;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.GetRefreshToken;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Login;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.LogOut;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResendEmailVerification;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResendOtp;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResetPassword;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.SendEmailVerification;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.SendVerificationOtp;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.VerifyEmail;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.VerifyOtp;
using Khazen.Application.UseCases.AuthenticationModule.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Khazen.Presentation.Controllers.AuthenticationModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(ISender _mediator)
        : ControllerBase
    {

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromForm] LoginRequest request)
        {
            var command = new LoginCommand(request.Email, request.Password);
            var response = await _mediator.Send(command);

            if (!string.IsNullOrEmpty(response.RefreshToken))
                SetRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiration);

            return Ok(response);
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<AuthResponse>> RefreshToken()
        {
            var command = new GetRefreshTokenCommand(Request.Cookies["refreshToken"]!);
            var response = await _mediator.Send(command);

            if (!string.IsNullOrEmpty(response.RefreshToken))
                SetRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiration);

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "User logged out successfully." });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "Password changed successfully." });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "Password reset email sent." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "Password reset successfully." });
        }

        [HttpPost("send-email-verification")]
        [AllowAnonymous]
        public async Task<IActionResult> SendEmailVerification([FromBody] SendEmailVerificationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "Verification email sent successfully." });
        }


        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "Email verified successfully." });
        }

        [HttpPost("resend-email-verification")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailVerification([FromBody] ResendEmailVerificationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "Verification email resent successfully." });
        }

        [HttpPost("send-otp")]
        [Authorize]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "OTP sent successfully." });
        }

        [HttpPost("verify-otp")]
        [Authorize]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "Phone number verified successfully." });
        }

        [HttpPost("resend-otp")]
        [Authorize]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "OTP resent successfully." });
        }

        private void SetRefreshTokenCookie(string refreshToken, DateTime expireDate)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expireDate.ToLocalTime(),
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}