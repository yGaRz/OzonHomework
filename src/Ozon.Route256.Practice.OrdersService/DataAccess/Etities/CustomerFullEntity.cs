﻿using System;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities
{
    //int32 id = 1;
    //string first_name = 2;
    //string last_name = 3;
    //string mobile_number = 4;
    //string email = 5;
    //Address default_address = 6;
    //repeated Address addressed = 7;
    public record CustomerFullEntity
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
        public string Phone { get; init; } = "";
        public string Email { get; init; } = "";
        public AddressEntity DefaultAddress { get; init; }
        public AddressEntity[] Addressed { get; init; }= new AddressEntity[0];

        public CustomerFullEntity Convert(Customer customer)
        {
            return new CustomerFullEntity()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.MobileNumber,
                Email = customer.Email,
                DefaultAddress = AddressEntity.Convert(customer.DefaultAddress),
                Addressed = customer.Addressed.Select(AddressEntity.Convert).ToArray()
            };
        }

    }

    
}
