using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.DTOs.Accounts
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string IBAN { get; set; }
        public decimal Balance { get; set; }
        public string AccountType { get; set; }
        public string AccountStatus { get; set; }

        public int? WithdrawLimits { get; set; }    
        public int? CurrentMonthWithdrawals { get; set; }

        public int? DepositTermMonths { get; set; }
        public DateTime? MaturityDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
