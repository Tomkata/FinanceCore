using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.ValueObjects
{
    public record class Address
    {

        public Address(string cityAddress, string city, int zip, string country)
        {
            this.CityAddress = cityAddress;
            this.City = city;
            this.Zip = zip;
            this.Country = country;
        }

        public string CityAddress { get; }
        public string City { get;  }
        public int Zip { get; }
        public string Country { get; }
    }
}
