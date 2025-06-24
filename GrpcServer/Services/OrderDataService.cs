using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ProductService;

namespace GrpcServer.Services
{
    public class OrderDataService : OrderService.OrderServiceBase
    {
        static int id = 0;
        static List<Order> orders = new()
        {
            new Order() { Id = ++id, Date = DateTime.UtcNow.ToTimestamp(), Products =
                {
                    new Product() { Name = "Резной столик для чая", Price =  10000.00 },
                    new Product() { Name = "IPhone 16 pro", Price =  10000.00 }
                }
            },
            new Order() { Id = ++id, Date = DateTime.UtcNow.ToTimestamp(), Products =
                {
                    new Product() { Name = "Гречка", Price = 113.69 },
                    new Product() { Name = "Маргарин", Price = 83.43 }
                } },
        };

        static List<Product> ConvertRepeatedToProduct(RepeatedField<Product> productReply)
            => productReply.Select(p => new Product() { Name = p.Name, Price = p.Price }).ToList();

        public override Task<Order> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            var order = new Order() { Id = ++id, Date = request.Date };
            order.Products.AddRange(request.Products);
            orders.Add(order);
            var reply = new Order() { Id = order.Id, Date = order.Date };
            reply.Products.AddRange(order.Products);
            return Task.FromResult(reply);
        }

        public override Task<Order> GetOrder(GetOrderRequest request, ServerCallContext context)
        {
            var order = orders.Find(o => o.Id == request.Id);
            if (order == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Order Not Found"));
            }
            var reply = new Order() { Id = order.Id, Date = order.Date };
            reply.Products.AddRange(order.Products);

            return Task.FromResult(reply);
        }

        public override Task<Order> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
        {
            var order = orders.Find(o => o.Id == request.Id);
            if (order == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Order Not Found"));
            }
            order.Products.AddRange(request.Products);

            var reply = new Order() { Id = order.Id, Date = order.Date };
            reply.Products.AddRange(order.Products);
            return Task.FromResult(reply);
        }

        public override Task<Order> DeleteOrder(DeleteOrderRequest request, ServerCallContext context)
        {
            var order = orders.Find(o => o.Id == request.Id);
            if (order == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Order Not Found"));
            }

            orders.Remove(order);


            var reply = new Order() { Id = order.Id, Date = order.Date };
            reply.Products.AddRange(order.Products);
            return Task.FromResult(reply);
        }

        public override Task<OrderList> ListOrder(Empty request, ServerCallContext context)
        {
            var reply = new OrderList();
            var orderList = orders.Select(o =>
            {
                var order = new Order() { Id = o.Id, Date = o.Date };
                order.Products.AddRange(o.Products);
                return order;
            });
            reply.Orders.AddRange(orderList);
            return Task.FromResult(reply);
        }

        public override Task<OrderList> FilterOrders(FilterRequest request, ServerCallContext context)
        {
            var result = orders
                .Where(o => o.Date.ToDateTime() >= request.Date.ToDateTime() &&
                            o.Products.Sum(i => i.Price) >= request.MinPrice)
                .ToList();

            var response = new OrderList();
            response.Orders.AddRange(result);
            return Task.FromResult(response);
        }
    }
}
