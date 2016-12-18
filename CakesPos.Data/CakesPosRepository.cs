using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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

        public void AddProduct(string productName, decimal price, decimal catererDiscount, int restockAmount, int inStock, string image, int categoryId)
        {
            Product p = new Product();
            p.CategoryId = categoryId;
            p.ProductName = productName;
            p.Price = price;
            p.CatererDiscount = catererDiscount;
            p.RestockAmount = restockAmount;
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

        public void AddCharge(string description, decimal price, int orderId)
        {
            Charge c = new Charge();
            c.Description = description;
            c.Price = price;
            c.OrderId = orderId;
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Charges.InsertOnSubmit(c);
                context.SubmitChanges();
            }
        }

        public IEnumerable<Charge> GetCharges(int orderId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                return context.Charges.Where(c => c.OrderId == orderId).ToList();
            }
        }

        //public void EditProduct()

        public int AddCustomer(string firstName, string lastName, string address, string city, string state, string zip, string phone1, string phone2, string cell1, string cell2, bool caterer, string email)
        {
            Customer c = new Customer();
            c.FirstName = firstName;
            c.LastName = lastName;
            c.Address = address;
            c.City = city;
            c.State = state;
            c.Zip = zip;
            c.Phone1 = phone1;
            c.Cell1 = cell1;
            c.Phone2 = phone2;
            c.Cell2 = cell2;
            c.Caterer = caterer;
            c.Email = email;
            c.Account = 0;

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Customers.InsertOnSubmit(c);
                context.SubmitChanges();
                int id = c.Id;
                return id;
            }
        }

        public void UpdateStock(int orderId)
        {
            IEnumerable<OrderDetail> od = GetOrderDetailsById(orderId);
            using (var context = new CakesPosDataContext(_connectionString))
            {
                foreach (OrderDetail o in od)
                {
                    var p = context.Products.Where(product => product.Id == o.ProductId).FirstOrDefault();
                    p.InStock -= o.Quantity;
                    context.SubmitChanges();
                }
            }
        }

        public void openOrder(int orderId)
        {
            if (File.Exists(@"C:\inetpub\sites\CakesPos\InvoicesPdf\" + orderId + ".pdf"))
            {
                File.Delete(@"C:\inetpub\sites\CakesPos\InvoicesPdf\" + orderId + ".pdf");
            }
            using (var context = new CakesPosDataContext(_connectionString))
            {
                var order = context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
                order.Invoice = false;
                var status = context.Status.Where(s => s.OrderId == orderId);
                context.SubmitChanges();
            }
        }

        //public Order GetOrderById(int orderId)
        //{
        //    using (var context=new CakesPosDataContext(_connectionString))
        //    {
        //        return context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
        //    }
        //}

        public int AddOrder(int customerId, DateTime orderDate, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone1, string phone2, string cell1, string cell2, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes, string greetings, string deliveryNote, bool paid, decimal deliveryCharge)
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
            o.Phone1 = phone1;
            o.Phone2 = phone2;
            o.Cell1 = cell1;
            o.Cell2 = cell2;
            o.CreditCard = creditCard;
            o.Expiration = expiration;
            o.SecurityCode = securityCode;
            o.Discount = discount;
            o.DeliveryOption = deliveryOpt;
            o.PaymentMethod = paymentMethod;
            o.Paid = paid;
            o.Notes = notes;
            o.Greetings = greetings;
            o.DeliveryNote = deliveryNote;
            o.Statement = false;
            o.Invoice = false;
            o.DeliveryCharge = deliveryCharge;
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
                return context.Products.OrderBy(p => p.ProductName).ToList();
            }
        }

        public IEnumerable<Category> GetAllCategories()
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.Categories.ToList();
            }
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.Products.Where(p => p.CategoryId == categoryId).Where(product => product.Discontinued == false || product.Discontinued ==null).OrderBy(p => p.ProductName).ToList();
            }
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                return context.Customers.OrderBy(c => c.LastName).ToList();
            }
        }

        public IEnumerable<OrderHistoryViewModel> GetOrders()
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
                    oh.payments = GetPaymentsByOrderId(oh.id);
                    oh.status = GetLatestStatusById(oh.id);
                    oh.invoice = reader.GetBoolean(reader.GetOrdinal("Invoice"));
                    Customer c = GetCustomerById(oh.customerId);
                    oh.firstName = c.FirstName;
                    oh.lastName = c.LastName;
                    oh.caterer = (bool)c.Caterer;

                    orders.Add(oh);
                }

                return orders.OrderByDescending(o => o.requiredDate).OrderBy(o => o.lastName).OrderBy(o => o.firstName);
            }
        }

        public IEnumerable<Customer> SearchCustomers(string search)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.Customers.Where(c => c.FirstName.StartsWith(search) || c.LastName.StartsWith(search) || c.Phone1.StartsWith(search) || c.Phone2.StartsWith(search) || c.Cell1.StartsWith(search) || c.Cell2.StartsWith(search)).ToList().OrderBy(c => c.LastName).ToList();
            }
        }

        //public IEnumerable<OrderHistoryViewModel> SearchOrders(string search, int x, string opt)
        //{
        //    List<OrderHistoryViewModel> orders = new List<OrderHistoryViewModel>();
        //    using (var connection = new SqlConnection(_connectionString))
        //    using (var cmd = connection.CreateCommand())
        //    {

        //    }
        //}

        public IEnumerable<OrderHistoryViewModel> SearchOrders(string search, string opt, DateTime date, bool all)
        {
            string com = "o.requiredDate = '" + date.ToShortDateString() + "' AND";
            if (all)
            {
                com = "";
            }
            string option = "";

            if (opt == "open")
            {
                option = "((o.paid=0 AND o.Invoice=1 AND o.Statement=0) OR (o.paid=1 AND o.Invoice=0) OR (o.paid=0 AND o.Invoice=0)) AND";
            }
            else if (opt == "delivered")
            {
                option = "o.Invoice=1 AND";
            }
            else if (opt == "paid")
            {
                option = "o.paid=1 AND";
            }
            else if (opt == "closed")
            {
                option = "o.Invoice=1 AND (o.paid=1 OR o.statement=1) AND";
            }
            else
            {
                option = "";
            }
            string q = "'" + search + "%'";
            List<OrderHistoryViewModel> orders = new List<OrderHistoryViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"Select o.CreditCard,o.CustomerId,o.DeliveryAddress,o.DeliveryCity,o.DeliveryFirstName,o.DeliveryLastName,o.DeliveryNote,o.DeliveryOption,o.DeliveryState,o.DeliveryZip,o.Discount,o.Expiration,o.Greetings,o.Id AS orderId,o.Notes,o.OrderDate,o.Paid,o.PaymentMethod,o.Phone1,o.Phone2,o.Cell1,o.Cell2,o.RequiredDate,o.SecurityCode,o.[Statement],o.[Invoice],c.Address,c.Caterer,c.City,c.Email,c.FirstName,c.Id,c.LastName,c.State,c.Zip
                                    FROM Customers AS c
                                    JOIN Orders AS o
                                    ON  o.CustomerId= c.Id
                                    WHERE " + option + " " + com + @" (c.FirstName LIKE '" + search + "%' OR c.LastName LIKE '" + search + "%' OR c.Phone1 LIKE '" + search + "%' OR c.Phone2 LIKE '" + search + "%' OR c.Cell1 LIKE '" + search + "%' OR c.Cell2 LIKE '" + search + "%')ORDER BY c.LastName ASC, c.FirstName ASC, o.RequiredDate DESC";
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrderHistoryViewModel oh = new OrderHistoryViewModel();
                    oh.id = (int)reader["orderId"];
                    oh.customerId = (int)reader["CustomerId"];
                    oh.orderDate = (DateTime)reader["OrderDate"];
                    oh.requiredDate = (DateTime)reader["RequiredDate"];
                    oh.paymentMethod = (string)reader["PaymentMethod"];
                    oh.paid = reader.GetBoolean(reader.GetOrdinal("Paid"));
                    oh.deliveryOpt = (string)reader["DeliveryOption"];
                    oh.discount = (decimal)reader["Discount"];
                    oh.payments = GetPaymentsByOrderId(oh.id);
                    oh.status = GetLatestStatusById(oh.id);
                    oh.invoice = reader.GetBoolean(reader.GetOrdinal("Invoice"));
                    oh.statement = reader.GetBoolean(reader.GetOrdinal("Statement"));
                    Customer c = GetCustomerById(oh.customerId);
                    oh.firstName = c.FirstName;
                    oh.lastName = c.LastName;
                    oh.caterer = (bool)c.Caterer;

                    orders.Add(oh);
                }

                return orders;
            }
        }

        public IEnumerable<Payment> GetPaymentsByOrderId(int id)
        {
            List<Payment> payments = new List<Payment>();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                payments = context.Payments.Where(p => p.OrderId == id).ToList();
                return payments;
            }
        }

        public Customer GetCustomerById(int id)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                Customer customer = context.Customers.Where(c => c.Id == id).FirstOrDefault();
                return customer;
            }
        }

        public Order GetOrderById(int orderId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                Order order = context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
                return order;
            }
        }

        public IEnumerable<OrderDetail> GetOrderDetailsById(int orderId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                IEnumerable<OrderDetail> orderDetails = context.OrderDetails.Where(od => od.OrderId == orderId).ToList();
                return orderDetails;
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
                odvm.payments = context.Payments.Where(p => p.OrderId == orderId).ToList();
                foreach (OrderDetail od in odvm.orderDetails)
                {
                    OrderDetailsProductModel pm = new OrderDetailsProductModel();
                    Product p = (GetProductById(od.ProductId));
                    pm.categoryId = p.CategoryId;
                    pm.productId = p.Id;
                    pm.productName = p.ProductName;
                    pm.quantity = od.Quantity;
                    pm.unitPrice = p.Price;
                    pm.catererDiscount = (decimal)p.CatererDiscount;

                    products.Add(pm);
                }
            }
            odvm.total = GetTotalByOrderId(orderId, customerId);
            odvm.orderedProducts = products;
            return odvm;
        }

        public Product GetProductById(int productId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.Products.Where(p => p.Id == productId).FirstOrDefault();
            }
        }

        public void EditProduct(Product edited, int productId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                Product p = context.Products.Where(product => product.Id == productId).FirstOrDefault();
                p.CategoryId = edited.CategoryId;
                p.CatererDiscount = edited.CatererDiscount;
                if (edited.Image != "")
                {
                    p.Image = edited.Image;
                }
                p.Price = edited.Price;
                p.ProductName = edited.ProductName;
                p.RestockAmount = edited.RestockAmount;
                p.Discontinued = edited.Discontinued;
                context.SubmitChanges();
            }
        }

        public decimal GetTotalByOrderId(int orderId, int customerId)
        {
            Customer c = GetCustomerById(customerId);
            decimal total = 0;
            decimal discount = 0;
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                IEnumerable<OrderDetail> orderDetails = context.OrderDetails.Where(od => od.OrderId == orderId).ToList();
                Order o = context.Orders.Where(order => order.Id == orderId).FirstOrDefault();
                discount = (decimal)o.Discount;
                if (o.DeliveryCharge != null)
                {
                    total += (decimal)o.DeliveryCharge;
                }
                foreach (OrderDetail od in orderDetails)
                {
                    decimal catererDiscount = 0;
                    if ((bool)c.Caterer)
                    {
                        Product p = GetProductById(od.ProductId);
                        if (p.CatererDiscount < 1)
                        {
                            catererDiscount = od.UnitPrice * od.Quantity * (decimal)p.CatererDiscount;
                            total += (od.Quantity * od.UnitPrice - catererDiscount);
                        }
                        else
                        {
                            catererDiscount = (decimal)p.CatererDiscount;
                            total += (od.UnitPrice * od.Quantity) - (od.Quantity * catererDiscount);
                        }
                        //else if (p.CategoryId == 2 || p.CategoryId == 3)
                        //{
                        //    catererDiscount = (double)od.UnitPrice * (double)od.Quantity * 0.1;
                        //    total += ((double)od.Quantity * (double)od.UnitPrice - catererDiscount);
                        //}
                        //else if (p.CategoryId == 4)
                        //{
                        //    catererDiscount = 2.5;
                        //    total += (double)od.UnitPrice * (double)od.Quantity - (double)od.Quantity * catererDiscount;
                        //}
                        //else
                        //{
                        //    total += (double)od.UnitPrice * (double)od.Quantity;
                        //}
                    }
                    else
                    {
                        total += od.UnitPrice * od.Quantity;
                    }
                }
            }
            if (discount < 1)
            {
                discount = discount * total;
            }
            return total - discount;
        }

        public IEnumerable<InventoryViewModel> GetInventory(DateTime min, DateTime max)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            List<Order> orders = new List<Order>();
            List<InventoryViewModel> ivm = new List<InventoryViewModel>();
            List<Product> products = new List<Product>();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                products = context.Products.OrderBy(p => p.ProductName).Where(product => product.Discontinued == false || product.Discontinued == null).ToList();
                orders = context.Orders.Where(o => o.RequiredDate >= min && o.RequiredDate <= max && o.Invoice == false).ToList();

                foreach (Order o in orders)
                {
                    List<OrderDetail> oDetails = new List<OrderDetail>();
                    oDetails = context.OrderDetails.Where(od => od.OrderId == o.Id).ToList();
                    foreach (OrderDetail od in oDetails)
                    {
                        orderDetails.Add(od);
                    }
                }
            }

            foreach (Product p in products)
            {
                int quantity = 0;
                InventoryViewModel i = new InventoryViewModel();
                IEnumerable<OrderDetail> oDetail = orderDetails.FindAll(o => o.ProductId == p.Id).ToList();
                foreach (OrderDetail od in oDetail)
                {
                    quantity += od.Quantity;
                }
                i.product = p;
                i.requestedAmount = quantity;
                ivm.Add(i);
            }
            return ivm;
        }

        public IEnumerable<InventoryViewModel> GetProductAvailability(DateTime min, DateTime max, int category)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            List<Order> orders = new List<Order>();
            List<InventoryViewModel> ivm = new List<InventoryViewModel>();
            List<Product> products = new List<Product>();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                products = context.Products.Where(p => p.CategoryId == category).Where(product => product.Discontinued == false || product.Discontinued == null).OrderBy(p => p.ProductName).ToList();
                orders = context.Orders.Where(o => o.RequiredDate >= min && o.RequiredDate <= max && o.Invoice == false).ToList();

                foreach (Order o in orders)
                {
                    List<OrderDetail> oDetails = new List<OrderDetail>();
                    oDetails = context.OrderDetails.Where(od => od.OrderId == o.Id).ToList();
                    foreach (OrderDetail od in oDetails)
                    {
                        orderDetails.Add(od);
                    }
                }
            }

            foreach (Product p in products)
            {
                int quantity = 0;
                InventoryViewModel i = new InventoryViewModel();
                IEnumerable<OrderDetail> oDetail = orderDetails.FindAll(o => o.ProductId == p.Id).ToList();
                foreach (OrderDetail od in oDetail)
                {
                    quantity += od.Quantity;
                }
                i.product = p;
                i.requestedAmount = quantity;
                ivm.Add(i);
            }
            return ivm;
        }

        public int GetProductAvailabilityByProductId(DateTime min, DateTime max, int productId)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            List<Order> orders = new List<Order>();
            Product product = new Product();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                product = context.Products.Where(p => p.Id == productId).FirstOrDefault();
                orders = context.Orders.Where(o => o.RequiredDate >= min && o.RequiredDate <= max && o.Invoice == false).ToList();

                foreach (Order o in orders)
                {
                    List<OrderDetail> oDetails = new List<OrderDetail>();
                    oDetails = context.OrderDetails.Where(od => od.OrderId == o.Id).ToList();
                    foreach (OrderDetail od in oDetails)
                    {
                        orderDetails.Add(od);
                    }
                }
            }

            int quantity = 0;
            IEnumerable<OrderDetail> oDetail = orderDetails.FindAll(o => o.ProductId == productId).ToList();
            foreach (OrderDetail od in oDetail)
            {
                quantity += od.Quantity;
            }
            return quantity;
        }

        public void UpdateInventory(int id, int amount)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                var product = (from p in context.Products
                               where p.Id == id
                               select p).First();
                product.InStock += amount;
                context.SubmitChanges();
            }
        }

        public void DeleteOrderDetailsById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"DELETE from OrderDetails
                                    Where OrderId=@orderId";
                cmd.Parameters.AddWithValue("@orderId", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void UpdateOrderById(int orderId, int customerId, DateTime orderDate, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone1, string phone2, string cell1, string cell2, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes, string greetings, string deliveryNote, bool paid, decimal deliveryCharge)
        {

            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Orders
                                  SET customerId=@customerId, orderDate=@orderDate, requiredDate=@requiredDate, deliveryFirstName=@deliveryFirstName, deliveryLastName=@deliveryLastName, deliveryAddress=@deliveryAddress, deliveryCity=@deliveryCity, deliveryState=@deliveryState, deliveryZip=@deliveryZip, phone1=@phone1, phone2=@phone2, cell1=@cell1, cell2=@cell2, creditCard=@creditCard, expiration=@expiration, securityCode=@securityCode, discount=@discount, deliveryOption=@deliveryOpt, paymentMethod=@paymentMethod, paid=@paid, notes=@notes, greetings=@greetings, deliveryNote=@deliveryNote, deliveryCharge=@deliveryCharge
                                  WHERE Id=@id";
                cmd.Parameters.AddWithValue("@id", orderId);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.Parameters.AddWithValue("@orderDate", orderDate);
                cmd.Parameters.AddWithValue("@requiredDate", requiredDate);
                cmd.Parameters.AddWithValue("@deliveryFirstName", deliveryFirstName);
                cmd.Parameters.AddWithValue("@deliveryLastName", deliveryLastName);
                cmd.Parameters.AddWithValue("@deliveryAddress", deliveryAddress);
                cmd.Parameters.AddWithValue("@deliveryCity", deliveryCity);
                cmd.Parameters.AddWithValue("@deliveryState", deliveryState);
                cmd.Parameters.AddWithValue("@deliveryZip", deliveryZip);
                cmd.Parameters.AddWithValue("@phone1", phone1);
                cmd.Parameters.AddWithValue("@phone2", phone2);
                cmd.Parameters.AddWithValue("@cell1", cell1);
                cmd.Parameters.AddWithValue("@cell2", cell2);
                cmd.Parameters.AddWithValue("@creditCard", creditCard);
                cmd.Parameters.AddWithValue("@expiration", expiration);
                cmd.Parameters.AddWithValue("@securityCode", securityCode);
                cmd.Parameters.AddWithValue("@discount", discount);
                cmd.Parameters.AddWithValue("@deliveryOpt", deliveryOpt);
                cmd.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                cmd.Parameters.AddWithValue("@paid", paid);
                cmd.Parameters.AddWithValue("@notes", notes);
                cmd.Parameters.AddWithValue("@greetings", greetings);
                cmd.Parameters.AddWithValue("@deliveryNote", deliveryNote);
                cmd.Parameters.AddWithValue("@deliveryCharge", deliveryCharge);
                connection.Open();
                cmd.ExecuteNonQuery();
            }

            //return orderId;
        }

        //public void AddOrderDetails(int orderId, int productId, decimal unitPrice, int quantity)
        //{
        //    OrderDetail od = new OrderDetail();
        //    od.OrderId = orderId;
        //    od.ProductId = productId;
        //    od.UnitPrice = unitPrice;
        //    od.Quantity = quantity;

        //    using (var context = new CakesPosDataContext(_connectionString))
        //    {
        //        context.OrderDetails.InsertOnSubmit(od);
        //        context.SubmitChanges();
        //    }
        //}


        public void MakePayment(int customerId, int orderId, decimal amount, string paymentNote, string paymentMethod)
        {
            var total = (decimal)GetTotalByOrderId(orderId, customerId);
            var totalPayments = GetPaymentsByOrderId(orderId).Sum(p => p.Payment1);
            using (var context = new CakesPosDataContext(_connectionString))
            {
                Payment p = new Payment();
                p.CustomerId = customerId;
                p.OrderId = orderId;
                p.Payment1 = amount;
                p.PaymentNote = paymentNote;
                p.Date = DateTime.Now;
                p.PaymentMethod = paymentMethod;
                context.Payments.InsertOnSubmit(p);
                if (totalPayments + amount >= total)
                {
                    var order = context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
                    order.Paid = true;
                }
                context.SubmitChanges();
            }
        }

        public void DeleteOrderById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"DELETE from Orders
                                    Where Id=@Id";
                cmd.Parameters.AddWithValue("@Id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeletePaymentsById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"DELETE from Payments
                                    Where OrderId=@Id";
                cmd.Parameters.AddWithValue("@Id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteStatusById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"DELETE from Status
                                    Where OrderId=@Id";
                cmd.Parameters.AddWithValue("@Id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool CheckIfCaterer(int customerId)
        {
            Customer c = new Customer();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                c = context.Customers.Where(cust => cust.Id == customerId).FirstOrDefault();
            }
            return (bool)c.Caterer;
        }

        public void SetOrderInvoiced(int orderId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                var order = context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
                order.Invoice = true;
                context.SubmitChanges();
            }
        }

        public void AddStatus(int orderId, string status)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                Status s = new Status();
                s.OrderId = orderId;
                s.Status1 = status;
                context.Status.InsertOnSubmit(s);
                context.SubmitChanges();
            }
            SetOrderInvoiced(orderId);
            UpdateStock(orderId);
        }

        public Status GetLatestStatusById(int orderId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.Status.Where(s => s.OrderId == orderId).OrderByDescending(s => s.Id).FirstOrDefault();
            }
        }

        public int GenerateStatement(int customerId)
        {
            decimal total = 0;
            decimal payments = 0;
            List<Order> orders = new List<Order>();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                orders = context.Orders.Where(o => o.CustomerId == customerId && o.PaymentMethod == "Bill Monthly" && o.Statement == false).ToList();
            }
            foreach (Order o in orders)
            {
                total += (decimal)GetTotalByOrderId(o.Id, customerId);
                IEnumerable<Payment> p = GetPaymentsByOrderId(o.Id);
                payments += (decimal)p.Sum(x => x.Payment1);
            }
            Statement s = new Statement();
            s.CustomerId = customerId;
            s.Balance = total - payments;
            s.Date = DateTime.Now.Date;
            s.Open = true;
            int statementId = 0;
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Statements.InsertOnSubmit(s);
                context.SubmitChanges();
                statementId = s.Id;
            }
            foreach (Order o in orders)
            {
                OrdersStatement os = new OrdersStatement();
                os.OrderId = o.Id;
                os.StatementId = statementId;
                using (var context = new CakesPosDataContext(_connectionString))
                {
                    context.OrdersStatements.InsertOnSubmit(os);
                    var order = context.Orders.Where(or => or.Id == o.Id).FirstOrDefault();
                    order.Statement = true;
                    context.SubmitChanges();
                }
            }
            return statementId;
        }

        public void AddStatementFilePath(int id, string filePath)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                var statement = context.Statements.Where(s => s.Id == id).FirstOrDefault();
                statement.FilePath = filePath;
                context.SubmitChanges();
            }
        }

        public Statement GetStatementById(int statementId)
        {
            Statement Statement = new Statement();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                Statement = context.Statements.Where(s => s.Id == statementId).FirstOrDefault();
            }
            return Statement;
        }

        public IEnumerable<OrderDetailsViewModel> GetStatementOrders(int statementId, int customerId)
        {
            List<OrderDetailsViewModel> orders = new List<OrderDetailsViewModel>();
            List<OrdersStatement> ordersStatements = new List<OrdersStatement>();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                ordersStatements = context.OrdersStatements.Where(o => o.StatementId == statementId).ToList();
            }
            foreach (OrdersStatement o in ordersStatements)
            {
                orders.Add(GetOrderDetails(customerId, o.OrderId));
            }
            return orders;
        }

        public StatementsModel GetStatementsForPdf(int statementId, int customerId)
        {
            StatementsModel s = new StatementsModel();
            s.Statement = GetStatementById(statementId);
            s.Orders = GetStatementOrders(statementId, customerId);
            return s;
        }

        public IEnumerable<StatementsModel> GetAllOpenStatements()
        {
            List<StatementsModel> sModel = new List<StatementsModel>();
            List<Statement> statements = new List<Statement>();
            List<OrdersStatement> ordersStatements = new List<OrdersStatement>();

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                statements = context.Statements.Where(s => s.Open != false).ToList();
            }
            foreach (Statement s in statements)
            {
                List<OrderDetailsViewModel> orders = new List<OrderDetailsViewModel>();
                List<StatementPayment> payments = new List<StatementPayment>();
                StatementsModel sm = new StatementsModel();
                sm.Statement = s;
                using (var context = new CakesPosDataContext(_connectionString))
                {
                    context.DeferredLoadingEnabled = false;
                    payments = context.StatementPayments.Where(p => p.StatementId == s.Id).ToList();
                    ordersStatements = context.OrdersStatements.Where(o => o.StatementId == s.Id).ToList();
                }
                foreach (OrdersStatement o in ordersStatements)
                {
                    orders.Add(GetOrderDetails((int)s.CustomerId, o.OrderId));
                }
                sm.Payments = payments;
                sm.Orders = orders;
                sModel.Add(sm);
            }
            return sModel.OrderBy(o => o.Orders.First().customer.FirstName).OrderBy(o => o.Orders.First().customer.LastName).OrderByDescending(o => o.Statement.Date);
        }

        public IEnumerable<int> GetCustomerIdBySearch(string search)
        {
            List<int> customerIds = new List<int>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Customers 
                                WHERE FirstName LIKE '" + search + "%'  OR LastName LIKE '" + search + "%'";
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    customerIds.Add((int)reader["Id"]);
                }
            }
            return customerIds;
        }

        public IEnumerable<StatementsModel> GetStatementsFiltered(string search, string filter)
        {
            bool open = true;
            IEnumerable<int> ids = GetCustomerIdBySearch(search);
            List<StatementsModel> sModel = new List<StatementsModel>();
            List<Statement> statements = new List<Statement>();
            List<OrdersStatement> ordersStatements = new List<OrdersStatement>();

            if (filter == "closed")
            {
                open = false;
            }
            if (filter == "closed" || filter == "open")
            {
                foreach (int id in ids)
                {
                    using (var context = new CakesPosDataContext(_connectionString))
                    {
                        context.DeferredLoadingEnabled = false;
                        statements.AddRange(context.Statements.Where(s => s.CustomerId == id && s.Open == open).ToList());
                    }
                }
            }
            else
            {
                foreach (int id in ids)
                {
                    using (var context = new CakesPosDataContext(_connectionString))
                    {
                        context.DeferredLoadingEnabled = false;
                        statements.AddRange(context.Statements.Where(s => s.CustomerId == id).ToList());
                    }
                }
            }

            foreach (Statement s in statements)
            {
                List<OrderDetailsViewModel> orders = new List<OrderDetailsViewModel>();
                List<StatementPayment> payments = new List<StatementPayment>();
                StatementsModel sm = new StatementsModel();
                sm.Statement = s;
                using (var context = new CakesPosDataContext(_connectionString))
                {
                    context.DeferredLoadingEnabled = false;
                    payments = context.StatementPayments.Where(p => p.StatementId == s.Id).ToList();
                    ordersStatements = context.OrdersStatements.Where(o => o.StatementId == s.Id).ToList();
                }
                foreach (OrdersStatement o in ordersStatements)
                {
                    orders.Add(GetOrderDetails((int)s.CustomerId, o.OrderId));
                }
                sm.Payments = payments;
                sm.Orders = orders;
                sModel.Add(sm);
            }
            return sModel.OrderBy(o => o.Orders.First().customer.FirstName).OrderBy(o => o.Orders.First().customer.LastName).OrderByDescending(o => o.Statement.Date);
        }



        //public Statement GetStatementByCustomerId(int customerId)
        //{
        //    Statement s = new Statement();
        //    List<OrderDetailsViewModel> od = new List<OrderDetailsViewModel>();
        //    IEnumerable<OrderHistoryViewModel> oh = GetOrdersForStatementById(customerId);
        //    foreach (OrderHistoryViewModel o in oh)
        //    {
        //        od.Add(GetOrderDetails(customerId, o.id));
        //        UpdateOrderAsStatement(o.id);
        //    }
        //    s.Orders = od;
        //    s.StatementDate = DateTime.Now.Date;
        //    s.StatementNumber = AddStatement(s);
        //    return s;
        //}

        public void AddStatementsFilePath(int id, string path)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Statements
                                  SET FilePath = @filePath
                                  WHERE Id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@filePath", path);
                connection.Open();
                cmd.ExecuteScalar();
            }
        }

        //        public int AddStatement(Statement s)
        //        {
        //            int id = 0;
        //            using (var connection = new SqlConnection(_connectionString))
        //            using (var cmd = connection.CreateCommand())
        //            {
        //                cmd.CommandText = @"INSERT INTO Statement
        //                                  VALUES (@balance, @date)";
        //                cmd.Parameters.AddWithValue("@balance", s.Total);
        //                cmd.Parameters.AddWithValue("@date", s.StatementDate);
        //                //cmd.Parameters.AddWithValue("@filePath", filePath);
        //                connection.Open();
        //                id = (int)cmd.ExecuteScalar();
        //            }
        //            return id;
        //        }

        //        public void UpdateOrderAsStatement(int orderId)
        //        {
        //            using (var connection = new SqlConnection(_connectionString))
        //            using (var cmd = connection.CreateCommand())
        //            {
        //                cmd.CommandText = @"UPDATE Orders
        //                                  SET Statement = 1
        //                                  WHERE Id=@orderId";
        //                cmd.Parameters.AddWithValue("@orderId", orderId);
        //                connection.Open();
        //                cmd.ExecuteScalar();
        //            }
        //        }

        //public decimal GenerateStatmentTotal(List<Order> orders)
        //{
        //    decimal total = 0;
        //    foreach(Order o in orders)
        //    {
        //        IEnumerable<OrderDetail> od= GetTotalByOrderId(orders)
        //        foreach(OrderDetail d in od)
        //        {
        //            if(d.)
        //        }
        //    }
        //}

        public void AddStatementPayment(int customerId, int statementId, decimal amount, string paymentNote)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {

                StatementPayment p = new StatementPayment();
                p.CustomerId = customerId;
                p.StatementId = statementId;
                p.Payment = amount;
                p.PaymentNote = paymentNote;
                p.Date = DateTime.Now.Date;
                context.StatementPayments.InsertOnSubmit(p);
                var s = context.Statements.Where(x => x.Id == statementId).FirstOrDefault();
                if ((GetStatementPayments(statementId).Sum(y => y.Payment) + amount) >= (s.Balance) || amount >= s.Balance)
                {
                    s.Open = false;
                }
                context.SubmitChanges();
            }
        }

        public Order GetOrdersById(int orderId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                return context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
            }
        }

        public IEnumerable<OrdersStatement> GetStatementOrderIds(int statementId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                return context.OrdersStatements.Where(o => o.StatementId == statementId).ToList();
            }
        }

        public void UnCompleteOrder(int orderId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                var order = context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
                order.Invoice = false;
                context.SubmitChanges();
            }
            if (File.Exists(@"C:\inetpub\sites\CakesPos\InvoicesPdf\" + orderId + ".pdf"))
            {
                File.Delete(@"C:\inetpub\sites\CakesPos\InvoicesPdf\" + orderId + ".pdf");
            }
        }

        public void UpdateInvoicesPaid(int statementId)
        {
            IEnumerable<OrdersStatement> statementOrders = GetStatementOrderIds(statementId);
            List<Order> orders = new List<Order>();
            foreach (OrdersStatement os in statementOrders)
            {
                orders.Add(GetOrdersById(os.OrderId));
            }

            foreach (Order o in orders)
            {
                using (var context = new CakesPosDataContext(_connectionString))
                {
                    var order = context.Orders.Where(x => x.Id == o.Id).FirstOrDefault();
                    order.Paid = true;
                    context.SubmitChanges();
                }
            }
        }

        public IEnumerable<StatementPayment> GetStatementPayments(int statementId)
        {
            List<StatementPayment> payments = new List<StatementPayment>();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                payments = context.StatementPayments.Where(p => p.StatementId == statementId).ToList();
            }
            return payments;
        }

        public void SaveEmail(int customerId, string email)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                var customer = context.Customers.Where(c => c.Id == customerId).FirstOrDefault();
                customer.Email = email;
                context.SubmitChanges();
            }
        }

        public void DeductFromAccount(int customerId, int orderId, decimal amount)
        {
            //            using (var connection = new SqlConnection(_connectionString))
            //            using (var cmd = connection.CreateCommand())
            //            {
            //                cmd.CommandText = @"UPDATE Customers
            //                                  SET Account -= @amount
            //                                  WHERE Id=@customerid";
            //                cmd.Parameters.AddWithValue("@amount", amount);
            //                cmd.Parameters.AddWithValue("customerid", customerId);
            //                connection.Open();
            //                cmd.ExecuteScalar();
            //            }
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.MakeAccountTrans(customerId, -amount, "Order #" + orderId + " payment...");
            cpr.MakePayment(customerId, orderId, amount, "Account charged...", "");
        }

        public void sDeductFromAccount(int customerId, int statementId, decimal amount, string note)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.MakeAccountTrans(customerId, -amount, "Statement #" + statementId + " payment...");
            cpr.AddStatementPayment(customerId, statementId, amount, note);
        }



        public IEnumerable<OrderHistoryViewModel> GetCatererOrdersForStatements()
        {
            List<OrderHistoryViewModel> orders = new List<OrderHistoryViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * From Orders
                                  JOIN Customers
                                  ON Orders.CustomerId=Customers.Id
                                  WHERE Orders.PaymentMethod='Bill Monthly' AND Orders.Invoice=1 AND Orders.[Statement]!=1";
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
                    oh.payments = GetPaymentsByOrderId(oh.id);
                    //oh.status = GetLatestStatusById(oh.id);
                    //Customer c = GetCustomerById(oh.customerId);
                    oh.firstName = (string)reader["FirstName"];
                    oh.lastName = (string)reader["LastName"];
                    oh.caterer = (bool)reader["Caterer"];

                    oh.total = GetTotalDiscount((decimal)GetTotalByOrderId(oh.id, oh.customerId), oh.discount);
                    oh.balance = oh.total - (decimal)oh.payments.Sum(p => p.Payment1);

                    orders.Add(oh);
                }
                IEnumerable<OrderHistoryViewModel> filteredOrders = orders.GroupBy(o => o.customerId).Select(group => group.First());
                foreach (OrderHistoryViewModel o in filteredOrders)
                {
                    o.total = orders.FindAll(od => od.customerId == o.customerId).Sum(result => result.total);
                    o.balance = orders.FindAll(od => od.customerId == o.customerId).Sum(result => result.balance);
                }
                return filteredOrders;
            }
        }

        public IEnumerable<OrderHistoryViewModel> GetOrdersForStatementById(int customerId)
        {
            List<OrderHistoryViewModel> orders = new List<OrderHistoryViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * From Orders
                                  JOIN Customers
                                  ON Orders.CustomerId=Customers.Id
                                  WHERE Customers.Id =@Id AND Orders.PaymentMethod='Bill Monthly' AND Orders.[Statement]!=1";
                cmd.Parameters.AddWithValue("@Id", customerId);
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
                    oh.payments = GetPaymentsByOrderId(oh.id);
                    //oh.status = GetLatestStatusById(oh.id);
                    //Customer c = GetCustomerById(oh.customerId);
                    oh.firstName = (string)reader["FirstName"];
                    oh.lastName = (string)reader["LastName"];
                    oh.caterer = (bool)reader["Caterer"];

                    oh.total = GetTotalDiscount((decimal)GetTotalByOrderId(oh.id, oh.customerId), oh.discount);
                    oh.balance = oh.total - (decimal)oh.payments.Sum(p => p.Payment1);

                    orders.Add(oh);
                }
                IEnumerable<OrderHistoryViewModel> filteredOrders = orders.GroupBy(o => o.customerId).Select(group => group.First());
                foreach (OrderHistoryViewModel o in filteredOrders)
                {
                    o.total = orders.FindAll(od => od.customerId == o.customerId).Sum(result => result.total);
                    o.balance = orders.FindAll(od => od.customerId == o.customerId).Sum(result => result.balance);
                }
                return filteredOrders;
            }
        }

        public decimal GetTotalDiscount(decimal total, decimal discount)
        {
            if (discount < 0)
            {
                return total - (total * discount);
            }
            else
            {
                return total - discount;
            }
        }

        public void MakeAccountTrans(int customerId, decimal amount, string note)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                AccountTran a = new AccountTran();
                a.Date = DateTime.Today.Date;
                a.CustomerId = customerId;
                a.Amount = amount;
                a.Note = note;

                var c = context.Customers.Where(x => x.Id == customerId).FirstOrDefault();
                c.Account += amount;
                context.AccountTrans.InsertOnSubmit(a);
                context.SubmitChanges();
            }
        }

        public IEnumerable<AccountTran> GetAccountTrans(int customerId)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.AccountTrans.Where(a => a.CustomerId == customerId).OrderByDescending(a => a.Date).ToList();
            }
        }

        public void EditCustomer(int customerId, string firstName, string lastName, string address, string city, string state, string zip, string phone1, string phone2, string cell1, string cell2, bool caterer, string email)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                var c = context.Customers.Where(x => x.Id == customerId).FirstOrDefault();
                c.FirstName = firstName;
                c.LastName = lastName;
                c.Address = address;
                c.City = city;
                c.State = state;
                c.Zip = zip;
                c.Phone1 = phone1;
                c.Cell1 = cell1;
                c.Phone2 = phone2;
                c.Cell2 = cell2;
                c.Email = email;
                c.Caterer = caterer;
                context.SubmitChanges();
            }
        }


        public IEnumerable<OrderHistoryViewModel> GetCatererOrdersByDate(DateTime min, DateTime max)
        {
            List<OrderHistoryViewModel> orders = new List<OrderHistoryViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Orders WHERE OrderDate >= @min AND OrderDate<=@max";
                cmd.Parameters.AddWithValue("@min", min);
                cmd.Parameters.AddWithValue("@max", max);
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
                    oh.payments = GetPaymentsByOrderId(oh.id);
                    //oh.status = GetLatestStatusById(oh.id);
                    Customer c = GetCustomerById(oh.customerId);
                    oh.firstName = c.FirstName;
                    oh.lastName = c.LastName;
                    oh.caterer = (bool)c.Caterer;

                    oh.total = (decimal)GetTotalByOrderId(oh.id, oh.customerId);
                    oh.balance = oh.total - (decimal)oh.payments.Sum(p => p.Payment1);

                    orders.Add(oh);
                }
                IEnumerable<OrderHistoryViewModel> filteredOrders = orders.Where(o => o.caterer == true).GroupBy(o => o.customerId).Select(group => group.First());
                foreach (OrderHistoryViewModel o in filteredOrders)
                {
                    o.total = orders.FindAll(od => od.customerId == o.customerId).Sum(result => result.total);
                    o.balance = orders.FindAll(od => od.customerId == o.customerId).Sum(result => result.balance);
                }
                return filteredOrders;
            }
        }
    }
}

