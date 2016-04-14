using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakesPos.Data
{
    public class CakesPosRepository
    {
        private string _connectionString;

        public CakesPosRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddProduct(string productName, decimal price, int inStock, string image, int categoryId)
        {
            Product p = new Product();
            p.CategoryId = categoryId;
            p.ProductName = productName;
            p.Price = price;
            p.InStock = inStock;
            p.Image = image;

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Products.InsertOnSubmit(p);
                context.SubmitChanges();
            }
        }

        public void AddCategory(string categoryName)
        {
            Category c = new Category();
            c.CategoryName = categoryName;

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Categories.InsertOnSubmit(c);
                context.SubmitChanges();
            }
        }

        public void AddCustomer(string firstName, string lastName, string address, string city, string state, string zip, string phone, bool caterer)
        {
            Customer c = new Customer();
            c.FirstName = firstName;
            c.LastName = lastName;
            c.Address = address;
            c.City = city;
            c.State = state;
            c.Zip = zip;
            c.Phone = phone;
            c.Caterer = caterer;

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Customers.InsertOnSubmit(c);
                context.SubmitChanges();
            }
        }

        public void AddOrder(int customerId, DateTime orderDate, DateTime requiredDate, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone, string creditCard, string expiration, string securityCode, bool paid, string paymentMethod, decimal discount)
        {
            Order o = new Order();
            o.CustomerId = customerId;
            o.OrderDate = orderDate;
            o.RequiredDate = requiredDate;
            o.DeliveryFirstName = deliveryFirstName;
            o.DeliveryLastName = deliveryLastName;
            o.DeliveryAddress = deliveryAddress;
            o.DeliveryCity = deliveryCity;
            o.DeliveryState = deliveryState;
            o.DeliveryZip = deliveryZip;
            o.CreditCard = creditCard;
            o.Expiration = expiration;
            o.SecurityCode = securityCode;
            o.Discount = discount;

            Payment p = new Payment();
            p.CustomerId = customerId;
            p.Paid = paid;
            p.PaymentMethod = paymentMethod;

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Orders.InsertOnSubmit(o);
                context.SubmitChanges();

                p.OrderId = o.Id;

                context.Payments.InsertOnSubmit(p);
                context.SubmitChanges();
            }
        }

        public void AddOrderDetails(int orderId, int productId, decimal unitPrice, int quantity)
        {
            OrderDetail od = new OrderDetail();
            od.OrderId = orderId;
            od.ProductId = productId;
            od.UnitPrice = unitPrice;
            od.Quantity = quantity;

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.OrderDetails.InsertOnSubmit(od);
                context.SubmitChanges();
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                return context.Products.ToList();
            }
        }

        public IEnumerable<Category> GetAllCategories()
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                return context.Categories.ToList();
            }
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.Products.Where(p => p.CategoryId == categoryId).ToList();
            }
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                return context.Customers.ToList();
            }
        }

        public IEnumerable<Customer> SearchCustomers(string search)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.Customers.Where(c => c.FirstName.Contains(search) || c.LastName.Contains(search) || c.Phone.Contains(search)).ToList();
            }
        }
    }
}
