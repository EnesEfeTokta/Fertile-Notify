using System;

namespace FertileNotify.API.Models.Requests;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}
