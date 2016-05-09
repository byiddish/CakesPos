using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public void AddCustomer(string firstName, string lastName, string address, string city, string state, string zip, string phone, string cell, bool caterer)
        {
            Customer c = new Customer();
            c.FirstName = firstName;
            c.LastName = lastName;
            c.Address = address;
            c.City = city;
            c.State = state;
            c.Zip = zip;
            c.Phone = phone;
            c.Cell = cell;
            c.Caterer = caterer;

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Customers.InsertOnSubmit(c);
                context.SubmitChanges();
            }
        }

        public int AddOrder(int customerId, DateTime orderDate, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes)
        {
            int orderId = 0;

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
            o.Phone = phone;
            o.CreditCard = creditCard;
            o.Expiration = expiration;
            o.SecurityCode = securityCode;
            o.Discount = discount;
            o.DeliveryOption = deliveryOpt;
            o.PaymentMethod = paymentMethod;
            o.Paid = false;
            o.Notes = notes;
            //Payment p = new Payment();
            //p.CustomerId = customerId;
            //p.PaymentMethod = paymentMethod;

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Orders.InsertOnSubmit(o);
                context.SubmitChanges();

                orderId = o.Id;
            }

            return orderId;
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

        public IEnumerable<OrderHistoryViewModel> GetAllOrders()
        {
            List<OrderHistoryViewModel> orders = new List<OrderHistoryViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Orders";
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrderHistoryViewModel oh = new OrderHistoryViewModel();
                    oh.id = (int)reader["Id"];
                    oh.customerId = (int)reader["CustomerId"];
                    oh.orderDate = (DateTime)reader["OrderDate"];
                    oh.requiredDate = (DateTime)reader["RequiredDate"];
                    oh.paymentMethod = (string)reader["PaymentMethod"];
                    oh.paid = reader.GetBoolean(reader.GetOrdinal("Paid"));
                    oh.deliveryOpt = (string)reader["DeliveryOption"];
                    oh.discount = (decimal)reader["Discount"];
                    Customer c = GetCustomerById(oh.customerId);
                    oh.firstName = c.FirstName;
                    oh.lastName = c.LastName;

                    orders.Add(oh);
                }

                return orders;
            }
        }

        public IEnumerable<Customer> SearchCustomers(string search)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.Customers.Where(c => c.FirstName.Contains(search) || c.LastName.Contains(search) || c.Phone.Contains(search) || c.Cell.Contains(search)).ToList();
            }
        }

        public IEnumerable<OrderHistoryViewModel> SearchOrders(string search)
        {
            List<OrderHistoryViewModel> orders = new List<OrderHistoryViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"Select * from Customers
                                    join Orders
                                    on Orders.CustomerId=Customers.Id
                                    Where Customers.FirstName LIKE '%' + @query + '%'  OR Customers.LastName LIKE '%' +  @query + '%' OR Customers.Phone LIKE '%' +  @query + '%' OR Customers.Cell LIKE '%' +  @query + '%'";
                cmd.Parameters.AddWithValue("@query", search);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrderHistoryViewModel oh = new OrderHistoryViewModel();
                    oh.id = (int)reader["Id"];
                    oh.customerId = (int)reader["CustomerId"];
                    oh.orderDate = (DateTime)reader["OrderDate"];
                    oh.requiredDate = (DateTime)reader["RequiredDate"];
                    oh.paymentMethod = (string)reader["PaymentMethod"];
                    oh.paid = reader.GetBoolean(reader.GetOrdinal("Paid"));
                    oh.deliveryOpt = (string)reader["DeliveryOption"];
                    oh.discount = (decimal)reader["Discount"];
                    Customer c = GetCustomerById(oh.customerId);
                    oh.firstName = c.FirstName;
                    oh.lastName = c.LastName;

                    orders.Add(oh);
                }

                return orders;
            }
        }

        public Customer GetCustomerById(int id)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                return context.Customers.Where(c => c.Id == id).FirstOrDefault();
            }
        }

        public OrderDetailsViewModel GetOrderDetails(int customerId, int orderId)
        {
            List<OrderDetailsProductModel> products = new List<OrderDetailsProductModel>();
            OrderDetailsViewModel odvm = new OrderDetailsViewModel();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                odvm.customer = context.Customers.Where(c => c.Id == customerId).FirstOrDefault();
                odvm.order = context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
                odvm.orderDetails = context.OrderDetails.Where(od => od.OrderId == orderId).ToList();

                foreach (OrderDetail od in odvm.orderDetails)
                {
                    OrderDetailsProductModel pm = new OrderDetailsProductModel();
                    Product p = (GetProductById(od.ProductId));
                    pm.productId = p.Id;
                    pm.productName = p.ProductName;
                    pm.quantity = od.Quantity;
                    pm.unitPrice = p.Price;

                    products.Add(pm);
                }
            }
            odvm.orderedProducts = products;
            return odvm;
        }

        public Product GetProductById(int productId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                return context.Products.Where(p => p.Id == productId).FirstOrDefault();
            }
        }

        public decimal GetTotalByOrderId(int orderId)
        {
            decimal total = 0;
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                IEnumerable<OrderDetail> orderDetails = context.OrderDetails.Where(od => od.OrderId == orderId).ToList();
                foreach (OrderDetail od in orderDetails)
                {
                    total += od.UnitPrice * od.Quantity;
                }
            }
            return total;
        }

        public IEnumerable<InventoryViewModel> GetInventory(DateTime min, DateTime max)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            List<Order> orders = new List<Order>();
            List<InventoryViewModel> ivm=new List<InventoryViewModel>();
            List<Product> products = new List<Product>();
            using(var context=new CakesPosDataContext(_connectionString))
            {
                products = context.Products.ToList();
                orders = context.Orders.Where(o => o.RequiredDate > min && o.RequiredDate < max).ToList();

                foreach(Order o in orders)
                {
                    List<OrderDetail> oDetails = new List<OrderDetail>();
                    oDetails = context.OrderDetails.Where(od => od.OrderId == o.Id).ToList();
                    foreach(OrderDetail od in oDetails)
                    {
                        orderDetails.Add(od);
                    }
                }
            }

            foreach(Product p in products)
            {
                int quantity = 0;
                InventoryViewModel i = new InventoryViewModel();
                IEnumerable<OrderDetail> oDetail = orderDetails.FindAll(o => o.ProductId == p.Id).ToList();
                foreach(OrderDetail od in oDetail)
                {
                    quantity += od.Quantity;
                }
                i.product = p;
                i.requestedAmount = quantity;
                ivm.Add(i);
            }
            return ivm;
        }

        public void UpdateInventory(int id, int amount)
        {
            using(var context=new CakesPosDataContext(_connectionString))
            {
                var product = (from p in context.Products
                               where p.Id == id
                               select p).First();
                product.InStock += amount;
                context.SubmitChanges();
            }
        }
    }
}
