﻿using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Domain;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;
using System.Reflection.Metadata.Ecma335;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Mappers;

internal class DataLayerMapper : IDataReadMapper, IDataWriteMapper
{
    public List<RegionDto> RegionsDalToDto(RegionDal[] regions)
        => regions.Select(RegionDalToDto).ToList();
    public RegionDto RegionDalToDto(RegionDal region)
        => new RegionDto(Id: region.Id, Name: region.Name, Latitude: region.Latitude, Longitude: region.Longitude);

    public OrderDal OrderDalFromDomain(Order order)
    {
        return new OrderDal(order.Id,
            order.CustomerId,
            (OrderSourceEnum)order.Source,
            (OrderStateEnum)order.State, 
            order.TimeCreate,
            order.TimeUpdate,
            order.CountGoods,
            order.CountGoods,
            order.TotalWeigth,
            order.TotalPrice,
            new AddressDal(order.AddressOrder.Region,
                order.AddressOrder.City,
                order.AddressOrder.Street,
                order.AddressOrder.Building,
                order.AddressOrder.Apartment,
                order.AddressOrder.Coordinates.Latitude,
                order.AddressOrder.Coordinates.Longitude)
            );
    }
    public OrderDto OrderDalToDto(OrderDal order)
    {
        return new OrderDto(order.id,
            order.customer_id,
            (OrderSourceEnumDomain)order.source,
            (OrderStateEnumDomain)order.state,
            order.timeCreate,
            order.timeUpdate,
            order.regionId,
            order.countGoods,
            order.totalWeigth,
            order.totalPrice,
            AddressDalToDto(order.address));
    }
    public CustomerDto CustomerDalToDto(CustomerDal customer)
    {
        return new CustomerDto()
        {
            Id = customer.Id,
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.Phone,
            DefaultAddress = AddressDalToDto(customer.DefaultAddress),
            Addressed = customer.Addressed.Select(AddressDalToDto).ToArray()
        };
    }
    public AddressDto AddressDalToDto(AddressDal address)
    {
        return new AddressDto(address.Region, address.City, address.Street, address.Building, address.Apartment, address.Latitude, address.Longitude);
    }

    //public RegionStatisticDto RegionStatisticDalToDto(RegionStatisticDal statisticDal)
    //{
    //    return new RegionStatisticDto(
    //        statisticDal.regionId,
    //        statisticDal.TotalCountOrders,
    //        statisticDal.TotalSumOrders,
    //        statisticDal.TotalWigthOrders,
    //        statisticDal.TotalCustomers);
    //}
}
