﻿using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interface;
using Polly.Registry;
using System.Net.Http.Json;
namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface, HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {

        //get product
        public async Task<ProductDTO> GetProduct(int productId)
        {
            //call product api sing httpclient
            //redirect this call to the api gateway since product api is not response to outsiders
            var getProduct = await httpClient.GetAsync($"/api/Product/{productId}");
            if (!getProduct.IsSuccessStatusCode)
            {
                return null!;
            }
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;


        }

        public async Task<AppUserDTO> GetUser(int userID)
        {
            //call user api sing httpclient
            //redirect this call to the api gateway since product api is not response to outsiders
            var getUser = await httpClient.GetAsync($"/api/product/{userID}");
            if (!getUser.IsSuccessStatusCode)
            {
                return null!;
            }
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        //get order client by id
        public async Task<IEnumerable<OrderDTO>> GetOrderByClientId(int clientId)
        {
            //get all client's orders
            var orders = await orderInterface.GetOrderAsync(o => o.ClientId == clientId);

            if (!orders.Any())
            {
                return null!;
            }
            //convert from entity to DTO
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;

        }

        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            //Prepare order
            var order = await orderInterface.FindByIdAsync(orderId);
            if (order is null || order!.Id <= 0)
            {
                return null!;
            }
            //get retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");


            //prepare product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));


            //prepare client
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            //populate order details
            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                (productDTO.Quantity * order.PurchaseQuantity),
                order.OrderedDate
                );
        }
    }
}
