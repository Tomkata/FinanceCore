using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.DTOs.Customer
{
    public class UpdateAddressDto
    {
        public Guid AccountId { get; set; }
        public string Address { get; init; }
        public string City { get; init; }
        public int Zip { get; init; }
        public string Country { get; init; }
    }
}
