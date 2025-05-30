﻿using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interface;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrder()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
            {
                return NotFound("No order detected in the database");
            }
            var (_, list) = OrderConversion.FromEntity(null, orders);

            return list!.Any() ? Ok(list) : NotFound("No order detected in the database");
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);
            if (order is null)
            {
                return NotFound("No order detected in the database");
            }
            var (_order, _) = OrderConversion.FromEntity(order, null);
            return Ok(_order);
        }


        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
        {
            if (clientId <= 0) return BadRequest("Invalid data provied");
           
            var orders = await orderService.GetOrderByClientId(clientId);

            return orders.Any() ? Ok(orders) : NotFound(null);
        }


        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0) return BadRequest("Invalid data provied");
            
            var orderDetail = await orderService.GetOrderDetails(orderId);

            return orderDetail is not null ? Ok(orderDetail) : NotFound("No order found");
        }


        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Incomplete data submitted");
            }

            var getEntity = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.CreateAsync(getEntity);

            return response.Flag ? Ok(response) : BadRequest(response);
        }


        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
        {
            var order = OrderConversion.ToEntity(orderDTO);

            var response = await orderInterface.UpdateAsync(order);
            return response.Flag ? Ok(response) : BadRequest(response);
        }


        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO orderDTO)
        {
            var order = OrderConversion.ToEntity(orderDTO);

            var response = await orderInterface.DeleteAsync(order);
            return response.Flag ? Ok(response) : BadRequest(response);
        }



    }
}