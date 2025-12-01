using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Application.UseCases.Customers.CreateCustomer;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.ValueObjects;
using Mapster;

namespace BankingSystem.Application.Common.Mappings
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {

             TypeAdapterConfig<CreateCustomerCommand, Customer>
            .NewConfig()
            .ConstructUsing(src => new Customer(
                src.Data.UserName,
                src.Data.FirstName,
                src.Data.LastName,
                new PhoneNumber(src.Data.PhoneNumber),
                new Address(src.Data.Street, src.Data.City, int.Parse(src.Data.PostalCode), src.Data.Country),
                EGN.Create(src.Data.EGN)
            ));

            TypeAdapterConfig<CreateCustomerDto, Customer>
                .NewConfig()
                .ConstructUsing(src => new Customer(
                    src.UserName,
                    src.FirstName,
                    src.LastName,
                    new PhoneNumber(src.PhoneNumber),
                    new Address(src.Street, src.City, int.Parse(src.PostalCode), src.Country),
                    EGN.Create(src.EGN)
                ));

            // Domain → DTO
            TypeAdapterConfig<Customer, CustomerDto>
                .NewConfig()
                .Map(dest => dest.EGN, src => src.EGN.Value)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber.Value)
                .Map(dest => dest.City, src => src.Address.City)
                .Map(dest => dest.Street, src => src.Address.CityAddress)
                .Map(dest => dest.PostalCode, src => src.Address.Zip.ToString())
                .Map(dest => dest.Country, src => src.Address.Country);

        }

    }
}
