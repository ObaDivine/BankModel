using BankModel.Data.Interfaces;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BankModel.Web;

namespace BankModel.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DBContext _context;
        public AccountRepository(DBContext context)
        {
            _context = context;
        }

        public DateTime GetPasswordExpiryDate(string username)
        {
            return (from u in _context.ApplicationUser
                    where u.UserName == username
                    select u.PasswordExpiryDate.Date).FirstOrDefault();
        }

        public string GetProfileImage(string username)
        {
            return _context.ApplicationUser.Where(u => u.UserName == username)
                .Include(p => p.Profile)
                .Select(p => p.Profile.ProfileImage)
                .FirstOrDefault();
        }

        public string GetUserBranch(string username)
        {
            return _context.ApplicationUser.Where(u => u.UserName == username)
                .Include(p => p.Profile)
                .Select(u => u.Profile.Branch.BranchDesc).FirstOrDefault();
        }

        public string GetUserRole(string username)
        {
            return "coming soon";
        }

        public string GetUserStatus(string username)
        {
            return (from u in _context.ApplicationUser
                    where u.UserName == username
                    select u.Status).FirstOrDefault();
        }

        public string GetTransactionDate()
        {
            return _context.Parameter.Where(p => p.Name == "TRANSACTION_DATE").Select(p => p.Value).FirstOrDefault();
        }

        public bool UserIsLoggedIn(string username)
        {
            return _context.ApplicationUser.Where(u => u.UserName == username).Select(u => u.UserOnline).FirstOrDefault();
        }

        public void SetUserLogin(string username)
        {
            //This sets the user as logged in to prevent multiple login
            var user = _context.ApplicationUser.Where(u => u.UserName == username).FirstOrDefault();
            user.UserOnline = true;
            _context.SaveChanges();
        }

        public void ClearUserLogin(string username)
        {
            //This sets the user as logged in to prevent multiple login
            var user = _context.ApplicationUser.Where(u => u.UserName == username).FirstOrDefault();
            user.UserOnline = false;
            _context.SaveChanges();
        }

    }
}
