namespace MigrationHomework.Framework._02._02
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }
        public string Supply { get; set; }
        public decimal Num { get; set; }
        public int Selfprice { get; set; }
        public DateTime Date { get; set; }
    }

    public class AppContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();

        public AppContext() : base() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=migrations1Db.db");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using (var db = new AppContext())
            {
                db.Database.EnsureCreated();

                if (!db.Products.Any())
                {
                    db.Products.AddRange(new[]
                    {
                        new Product { Name = "Ручка", ProductType = "Канцелярия", Supply = "Поставщик1", Num = 50, Selfprice = 10, Date = DateTime.Now },
                        new Product { Name = "Тетрадь", ProductType = "Бумага", Supply = "Поставщик2", Num = 30, Selfprice = 25, Date = DateTime.Now },
                        new Product { Name = "Маркер", ProductType = "Канцелярия", Supply = "Поставщик3", Num = 15, Selfprice = 40, Date = DateTime.Now.AddDays(-5) }
                    });
                    db.SaveChanges();
                }

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Меню:");
                    Console.WriteLine("1. Показать все товары");
                    Console.WriteLine("2. Показать товары по категории");
                    Console.WriteLine("3. Показать товары по поставщику");
                    Console.WriteLine("4. Показать самый старый товар");
                    Console.WriteLine("5. Показать среднее количество товаров по типам");
                    Console.WriteLine("0. Выход");
                    Console.Write("Выберите действие: ");

                    string choice = Console.ReadLine();
                    Console.Clear();

                    switch (choice)
                    {
                        case "1":
                            Show(db);
                            break;
                        case "2":
                            ShowByCategory(db);
                            break;
                        case "3":
                            ShowBySupplier(db);
                            break;
                        case "4":
                            ShowOldest(db);
                            break;
                        case "5":
                            ShowAverage(db);
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Некорректный ввод.");
                            break;
                    }

                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void Show(AppContext db)
        {
            var products = db.Products.ToList();
            if (!products.Any())
            {
                Console.WriteLine("Склад пуст.");
                return;
            }

            Console.WriteLine("Все товары на складе:");
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}, Название: {product.Name}, Тип: {product.ProductType}, " +
                                  $"Поставщик: {product.Supply}, Количество: {product.Num}, " +
                                  $"Себестоимость: {product.Selfprice}, Дата поставки: {product.Date:d}");
            }
        }

        static void ShowByCategory(AppContext db)
        {
            Console.Write("Введите категорию товара: ");
            string category = Console.ReadLine();

            var products = db.Products.Where(p => p.ProductType.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!products.Any())
            {
                Console.WriteLine($"Товары категории \"{category}\" не найдены.");
                return;
            }

            Console.WriteLine($"Все товары категории \"{category}\":");
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}, Название: {product.Name}, Количество: {product.Num}, Себестоимость: {product.Selfprice}");
            }
        }

        static void ShowBySupplier(AppContext db)
        {
            Console.Write("Введите имя поставщика: ");
            string supplier = Console.ReadLine();

            var products = db.Products.Where(p => p.Supply.Equals(supplier, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!products.Any())
            {
                Console.WriteLine($"Товары поставщика \"{supplier}\" не найдены.");
                return;
            }

            Console.WriteLine($"Все товары поставщика \"{supplier}\":");
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}, Название: {product.Name}, Количество: {product.Num}, Себестоимость: {product.Selfprice}");
            }
        }

        static void ShowOldest(AppContext db)
        {
            var oldestProduct = db.Products.OrderBy(p => p.Date).FirstOrDefault();
            if (oldestProduct != null)
            {
                Console.WriteLine($"Самый старый товар: {oldestProduct.Name}, Дата поставки: {oldestProduct.Date:d}");
            }
            else
            {
                Console.WriteLine("Нет данных о товарах.");
            }
        }

        static void ShowAverage(AppContext db)
        {
            var averageQuantities = db.Products
                .GroupBy(p => p.ProductType)
                .Select(g => new
                {
                    ProductType = g.Key,
                    AverageQuantity = g.Average(p => p.Num)
                }).ToList();

            if (!averageQuantities.Any())
            {
                Console.WriteLine("Нет данных для вычисления.");
                return;
            }

            Console.WriteLine("Среднее количество товаров по типам:");
            foreach (var item in averageQuantities)
            {
                Console.WriteLine($"Тип товара: {item.ProductType}, Среднее количество: {item.AverageQuantity:F2}");
            }
        }
    }
}
