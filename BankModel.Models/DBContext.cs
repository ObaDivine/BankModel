using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankModel.Models
{
    public class DBContext: IdentityDbContext<ApplicationUser>
    {
        public DBContext(DbContextOptions<DBContext> options) :base(options){ }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Parameter> Parameter { get; set; }
        //public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<StandingOrder> StandingOrder { get; set; }
        public DbSet<ChartOfAccount> ChartOfAccount { get; set; }
        public DbSet<ChartOfAccountSubHead> ChartOfAccountSubHead { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<Account> Accounts{ get; set; }
        public DbSet<GLTransaction> GLTransaction { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<BalanceSheet> BalanceSheet { get; set; }
        public DbSet<ProfitAndLoss> ProfitAndLoss { get; set; }
        public DbSet<LoanFee> LoanFee { get; set; }
        public DbSet<Loan> Loan { get; set; }
        public DbSet<LoanRepayment> LoansRepayment { get; set; }
        public DbSet<SavingsInterest> SavingInterest { get; set; }
        public DbSet<OverdrawnAccount> OverdrawnAccount { get; set; }
        public DbSet<SMS> SMS { get; set; }
        public DbSet<FixedDeposit> FixedDeposit { get; set; }
        public DbSet<FixedDepositFee> FixedDepositFee { get; set; }
        public DbSet<FixedDepositInterestPayment> FixedDepositInterestPayment { get; set; }
        public DbSet<TemplateAccount> TemplateAccount { get; set; }
        public DbSet<TemplateSalary> TemplateSalary { get; set; }
        public DbSet<TemplateFixedDeposit> TemplateFixedDeposit { get; set; }
        public DbSet<TemplateLoan> TemplateLoan { get; set; }
        public DbSet<MonthlyFee> MonthlyFee { get; set; }
        public DbSet<TemplateLoanFee> TemplateLoanFee { get; set; }
        public DbSet<TemplateFixedDepositFee> TemplateFixedDepositFee { get; set; }
        //public DbSet<Product> Products { get; set; }
    }
}
