using Ecommerce.SharedLibrary.Interface;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;

namespace OrderApi.Application.Interface
{
    public interface IOrder : IGenericInterface<Order>
    {
        Task<IEnumerable<Order>> GetOrderAsync(Expression<Func<Order, bool>> predicate);

    }


}
