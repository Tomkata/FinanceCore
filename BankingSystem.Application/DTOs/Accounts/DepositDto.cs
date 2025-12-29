using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.DTOs.Accounts
{
    public class DepositDto
    {
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
