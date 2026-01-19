using FertileNotify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FertileNotify.Application.UseCases.RegisterUser
{
    public class RegisterUserCommand
    {
        public string Email { get; init; } = default!;
        public SubscriptionPlan Plan { get; init; }
    }
}
