using BankingSystem.Application.DTOs.Accounts;
using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Application.UseCases.Accounts.GetAccountById;
using BankingSystem.Application.UseCases.Customers.CreateCustomer;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.ValueObjects;
using Mapster;
using System.Xml.Serialization;

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
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.EGN, src => src.EGN.Value)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber.Value)
                .Map(dest => dest.City, src => src.Address.City)
                .Map(dest => dest.Street, src => src.Address.CityAddress)
                .Map(dest => dest.PostalCode, src => src.Address.Zip.ToString())
                .Map(dest => dest.Country, src => src.Address.Country);


            TypeAdapterConfig<Account, AccountDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.IBAN, src => src.IBAN.Value)
                .Map(dest => dest.Balance, src => src.Balance)
                .Map(dest => dest.AccountStatus, src => src.AccountStatus.ToString())
                .Map(dest => dest.AccountType, src => src.AccountType.ToString())
                .Map(dest => dest.WithdrawLimits, src => src.WithdrawLimits)
                .Map(dest => dest.CurrentMonthWithdrawals, src => src.CurrentMonthWithdrawals)
                .Map(dest => dest.MaturityDate, src => src.MaturityDate)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);













        }

    }
}
