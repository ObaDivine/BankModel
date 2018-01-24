using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BankModel.Models;
using BankModel.Data;
using BankModel.Data.Interfaces;
using BankModel.Data.Repositories;
using BankModel.Service.Interfaces;
using BankModel.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankModel.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DBContext>(option => option.UseSqlServer(Configuration.GetConnectionString("DBConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;

            })
                .AddEntityFrameworkStores<DBContext>()
                .AddDefaultTokenProviders();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddSingleton<IConfiguration>(Configuration);

            #region
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", policy => policy.RequireRole("ADMINISTRATOR"));
                options.AddPolicy("RoutineProcessing", policy => policy.RequireRole("ROUTINE PROCESSING"));
                options.AddPolicy("Enquiry", policy => policy.RequireRole("ENQUIRY"));
                options.AddPolicy("CustomerService", policy => policy.RequireRole("CUSTOMER SERVICE"));
                options.AddPolicy("CashTransaction", policy => policy.RequireRole("CASH TRANSACTION"));
                options.AddPolicy("Treasury", policy => policy.RequireRole("TREASURY"));
                options.AddPolicy("Loan", policy => policy.RequireRole("LOAN"));
                options.AddPolicy("FixedDeposit", policy => policy.RequireRole("FIXED DEPOSIT"));
                options.AddPolicy("GLTransaction", policy => policy.RequireRole("GL TRANSACTION"));
                options.AddPolicy("Salary", policy => policy.RequireRole("SALARY"));
                options.AddPolicy("Template", policy => policy.RequireRole("TEMPLATE"));
                options.AddPolicy("Report", policy => policy.RequireRole("REPORT"));
                options.AddPolicy("Setup", policy => policy.RequireRole("SETUP"));
                options.AddPolicy("Product", policy => policy.RequireRole("PRODUCT"));
                options.AddPolicy("MobileMoney", policy => policy.RequireRole("MOBILE MONEY"));
                options.AddPolicy("Approval", policy => policy.RequireRole("APPROVAL"));
                options.AddPolicy("HeadOffice", policy => policy.RequireClaim("HeadOfficeStaff"));

                //Add role claims
                #region

                //Approval permissions
                options.AddPolicy("ApproveSystemUsers", policy => policy.RequireClaim("APPROVE SYSTEM USERS"));
                options.AddPolicy("ApproveBranch", policy => policy.RequireClaim("APPROVE BRANCH"));
                options.AddPolicy("ApproveChart", policy => policy.RequireClaim("APPROVE CHART OF ACCOUNT"));
                options.AddPolicy("ApproveProfile", policy => policy.RequireClaim("APPROVE PROFILE"));
                options.AddPolicy("ApproveAccount", policy => policy.RequireClaim("APPROVE ACCOUNT"));
                options.AddPolicy("ApproveCashTransaction", policy => policy.RequireClaim("APPROVE CASH TRANSACTION"));
                options.AddPolicy("ApproveGLTransaction", policy => policy.RequireClaim("APPROVE GL TRANSACTION"));
                options.AddPolicy("ApproveLoan", policy => policy.RequireClaim("APPROVE LOAN"));
                options.AddPolicy("ApproveFixedDeposit", policy => policy.RequireClaim("APPROVE FIXED DEPOSIT"));
                options.AddPolicy("ApproveTreasury", policy => policy.RequireClaim("APPROVE TREASURY"));
                options.AddPolicy("ApproveSalary", policy => policy.RequireClaim("APPROVE SALARY"));
                options.AddPolicy("ApproveMobileMoney", policy => policy.RequireClaim("APPROVE MOBILE MONEY"));
                options.AddPolicy("ApproveTemplate", policy => policy.RequireClaim("APPROVE TEMPLATE"));

                //Administrator permissions
                options.AddPolicy("SystemUsers", policy => policy.RequireClaim("SYSTEM USERS"));
                options.AddPolicy("Parameter", policy => policy.RequireClaim("PARAMETER"));
                options.AddPolicy("UserRole", policy => policy.RequireClaim("USER ROLE"));
                options.AddPolicy("RemoteConnection", policy => policy.RequireClaim("REMOTE CONNECTION"));
                options.AddPolicy("AuditLog", policy => policy.RequireClaim("AUDIT LOG"));

                //Cash transaction permissions
                options.AddPolicy("Deposit", policy => policy.RequireClaim("DEPOSIT"));
                options.AddPolicy("Withdrawal", policy => policy.RequireClaim("WITHDRAWAL"));
                options.AddPolicy("FundTransfer", policy => policy.RequireClaim("FUND TRANSFER"));

                //Customer service permissions
                options.AddPolicy("CustomerAccount", policy => policy.RequireClaim("CUSTOMER ACCOUNT"));
                options.AddPolicy("IndividualProfile", policy => policy.RequireClaim("INDIVIDUAL PROFILE"));
                options.AddPolicy("CorporateProfile", policy => policy.RequireClaim("CORPORATE PROFILE"));

                //Routine processing permissions
                options.AddPolicy("EOD", policy => policy.RequireClaim("END OF DAY"));
                options.AddPolicy("EOM", policy => policy.RequireClaim("END OF MONTH"));
                options.AddPolicy("EOY", policy => policy.RequireClaim("END OF YEAR"));

                //Enquiry permissions
                options.AddPolicy("CustomerEnquiry", policy => policy.RequireClaim("CUSTOMER ENQUIRY"));
                options.AddPolicy("GLEnquiry", policy => policy.RequireClaim("GL ENQUIRY"));

                //Setup permissions
                options.AddPolicy("ChartOfAccount", policy => policy.RequireClaim("CHART OF ACCOUNT"));
                options.AddPolicy("AccountSubHead", policy => policy.RequireClaim("ACCOUNT SUBHEAD"));
                options.AddPolicy("Branch", policy => policy.RequireClaim("BRANCH"));

                //Template permissions
                options.AddPolicy("AccountTemplate", policy => policy.RequireClaim("ACCOUNT TEMPLATE"));
                options.AddPolicy("LoanTemplate", policy => policy.RequireClaim("LOAN TEMPLATE"));
                options.AddPolicy("FixedDepositTemplate", policy => policy.RequireClaim("FIXED DEPOSIT TEMPLATE"));
                options.AddPolicy("TransactionReport", policy => policy.RequireClaim("TRANSACTION"));
                options.AddPolicy("OperationReport", policy => policy.RequireClaim("OPERATION"));
                options.AddPolicy("ManagementReport", policy => policy.RequireClaim("MANAGEMENT"));
                options.AddPolicy("LoanReport", policy => policy.RequireClaim("LOAN"));
                options.AddPolicy("FixedDepositReport", policy => policy.RequireClaim("FIXED DEPOSIT"));
                #endregion
            });

            #endregion

            services.AddMvc(options => {
                options.RequireHttpsPermanent = true;
                options.SslPort = 1520;
                options.Filters.Add(new RequireHttpsAttribute());
            });

            #region
            //Add application services and repositories
            services.AddTransient<DBInitializer>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICashService, CashService>();
            services.AddScoped<ISetupService, SetupService>();
            services.AddScoped<ISystemAdminService, SystemAdminService>();
            services.AddScoped<IGeneralLedgerService, GeneralLedgerService>();
            services.AddScoped<IRoutineProcessingService, RoutineProcessingService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IValidationDictionary, ValidationDictionary>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICashRepository, CashRepository>();
            services.AddScoped<ISetupRepository, SetupRepository>();
            services.AddScoped<ISystemAdminRepository, SystemAdminRepository>();
            services.AddScoped<IApprovalRepository, ApprovalRepository>();
            services.AddScoped<IGeneralLedgerRepository, GeneralLedgerRepository>();
            services.AddScoped<IRoutineProcessingRepository, RoutineProcessingRepository>();
            services.AddScoped<IEnquiryRepository, EnquiryRepository>();
            services.AddScoped<ICustomerServiceRepository, CustomerServiceRepository>();
            services.AddScoped<ITemplateRepository, TemplateRepository>();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DBInitializer dbInitialize)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });

            //This seed the database 
            dbInitialize.Seed().Wait();
        }
    }
}
