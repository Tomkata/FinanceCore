using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.ValueObjects
{
    public record class Address
    {
        private Address()
        {
            
        }
        public Address(string cityAddress, string city, int zip, string country)
        {
            this.CityAddress = cityAddress;
            this.City = city;
            this.Zip = zip;
            this.Country = country;
        }

        public string CityAddress { get; init; }
        public string City { get; init; }
        public int Zip { get; init; }
        public string Country { get; init; }
    }
}
