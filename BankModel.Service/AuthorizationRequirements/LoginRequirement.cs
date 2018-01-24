using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BankModel.Service.AuthorizationRequirements
{
    public class LoginRequirement: AuthorizationHandler<LoginRequirement>,IAuthorizationRequirement
    {
        public DateTime PasswwordExpiryDate { get; private set; }
        public string UserStatus { get; set; }

        public LoginRequirement(DateTime passwordExpiryDate, string userStatus)
        {
            PasswwordExpiryDate = passwordExpiryDate;
            UserStatus = userStatus;

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LoginRequirement requirement)
        {
            if (requirement.UserStatus == "ACTIVE")
            {
                context.Succeed(requirement);
            }

            if (requirement.PasswwordExpiryDate > DateTime.UtcNow.Date)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
