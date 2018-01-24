using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BankModel.Service.AuthorizationRequirements
{
    public class ApprovalRequirement : AuthorizationHandler<ApprovalRequirement>, IAuthorizationRequirement
    {
        public ApprovalRequirement()
        {

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApprovalRequirement requirement)
        {
            return Task.CompletedTask;
        }
    }
}
