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

            // Command -> Domain
            TypeAdapterConfig<CreateCustomerCommand, Customer>
                   .NewConfig()
                   .Map(dest => dest.EGN, src => EGN.Create(src.Data.EGN))
                   .Map(dest => dest.PhoneNumber, src => new PhoneNumber(src.Data.PhoneNumber))
                   .Map(dest => dest.Address, src => new Address(src.Data.Street, src.Data.City,
                   int.Parse(src.Data.PostalCode), src.Data.Country));

            //Domain -> Dto

            TypeAdapterConfig<Customer, CreateCustomerDto>
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
