using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using ProductService;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

using var chanel = GrpcChannel.ForAddress(config.GetSection("Sources")["Grps"]);
var orderClient = new OrderService.OrderServiceClient(chanel);

while (true)
{
    Console.WriteLine("1. Добавить заказ\n2. Получить заказ\n3. Удалить\n4. Все заказы\n5. Фильтрация\n0. Выход");
    var input = Console.ReadLine();
    switch (input)
    {
        case "1":
            var order = new CreateOrderRequest()
            {
                Date = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            Console.Write("Введите название товара:");
            var name = Console.ReadLine();
            Console.Write("Введите цену товара:");
            var price = Console.ReadLine();

            var product = new Product
            {
                Name = name,
                Price = Convert.ToDouble(price)
            };

            order.Products.Add(product);

            var result = await orderClient.CreateOrderAsync(order);
            Console.WriteLine($"Создан заказ с ID: {result.Id}");
            break;

        case "2":
            //Console.Write("Введите ID: ");
            //var id = Console.ReadLine();
            //var orderInfo = client.GetOrder(new OrderIdRequest { Id = id });
            //Console.WriteLine($"Дата: {orderInfo.Date.ToDateTime()}, Товаров: {orderInfo.Items.Count}");
            break;

        case "3":
            //Console.Write("Введите ID для удаления: ");
            //id = Console.ReadLine();
            //client.DeleteOrder(new OrderIdRequest { Id = id });
            //Console.WriteLine("Удалено.");
            break;

        case "4":
            OrderList reply = await orderClient.ListOrderAsync(new Empty());
            foreach (var o in reply.Orders)
            {
                Console.WriteLine($"Заказ {o.Id}|{o.Date,20}|{o.Products.Count}");
                foreach (var p in o.Products)
                    Console.WriteLine($"# {p.Name,30}|{p.Price}");
            }

            break;

        case "5":
            //Console.Write("Мин. дата (YYYY-MM-DD): ");
            //var date = DateTime.Parse(Console.ReadLine());
            //Console.Write("Мин. сумма: ");
            //var sum = double.Parse(Console.ReadLine());
            //var filtered = client.FilterOrders(new FilterRequest
            //{
            //    Date = Timestamp.FromDateTime(date.ToUniversalTime()),
            //    MinPrice = sum
            //});
            //foreach (var o in filtered.Orders)
            //    Console.WriteLine($"ID: {o.Id}, Сумма: {o.Items.Sum(i => i.Price)}");
            break;

        case "0":
            return;
        default:
            return;
    }
    Console.ReadLine();
    Console.Clear();
}


