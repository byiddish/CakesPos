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

        public void AddCustomer(string firstName, string lastName, string address, string city, string state, string zip, string phone, string cell, bool caterer, string email)
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
            c.Email = email;

            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.Customers.InsertOnSubmit(c);
                context.SubmitChanges();
            }
        }

        public int AddOrder(int customerId, DateTime orderDate, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes, string greetings, string deliveryNote, bool paid)
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
            o.Paid = paid;
            o.Notes = notes;
            o.Greetings = greetings;
            o.DeliveryNote = deliveryNote;
            o.Statement = false;
            o.Invoice = false;
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
                return context.Customers.ToList().OrderBy(c => c.LastName).OrderBy(c => c.FirstName);
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
                    Customer c = GetCustomerById(oh.customerId);
                    oh.firstName = c.FirstName;
                    oh.lastName = c.LastName;
                    oh.caterer = c.Caterer;

                    orders.Add(oh);
                }

                return orders.OrderByDescending(o => o.requiredDate);
            }
        }

        public IEnumerable<Customer> SearchCustomers(string search)
        {
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                return context.Customers.Where(c => c.FirstName.Contains(search) || c.LastName.Contains(search) || c.Phone.Contains(search) || c.Cell.Contains(search)).ToList().OrderBy(c => c.LastName).OrderBy(c => c.FirstName);
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

        public IEnumerable<OrderHistoryViewModel> SearchOrders(string search, int x, string opt)
        {
            DateTime today = DateTime.Now.Date;
            string com = "";
            string option = "";
            if (x == 8)
            {
                com = "";
            }
            else if (x == -1)
            {
                com = "o.requiredDate >= " + "'" + today.AddDays(x).ToShortDateString() + "'" + " AND o.requiredDate < " + "'" + today.ToShortDateString() + "'" + "AND";
            }
            else if (x <= 0)
            {
                com = "o.requiredDate >= " + "'" + today.AddDays(x).ToShortDateString() + "'" + " AND o.requiredDate <= " + "'" + today.ToShortDateString() + "'" + "AND";
            }
            else if (x == 1)
            {
                com = "o.requiredDate =" + "'" + today.AddDays(x).ToShortDateString() + "'" + "AND";
            }
            else if (x == 0)
            {
                com = "o.requiredDate = " + "'" + today.ToShortDateString() + "'" + "AND";
            }
            else
            {
                com = "o.requiredDate <= " + "'" + today.AddDays(x).ToShortDateString() + "'" + " AND o.requiredDate >= " + "'" + today.ToShortDateString() + "'" + "AND";
            }
            if (opt == "open")
            {
                option = "((o.paid=0 AND o.Invoice=1) OR (o.paid=1 AND o.Invoice=0) OR (o.paid=0 AND o.Invoice=0))";
            }
            else if (opt == "delivered")
            {
                option = "o.Invoice=1";
            }
            else if (opt == "paid")
            {
                option = "o.paid=1";
            }
            else if (opt == "closed")
            {
                option = "o.Invoice=1 AND o.paid=1";
            }
            List<OrderHistoryViewModel> orders = new List<OrderHistoryViewModel>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"Select o.CreditCard,o.CustomerId,o.DeliveryAddress,o.DeliveryCity,o.DeliveryFirstName,o.DeliveryLastName,o.DeliveryNote,o.DeliveryOption,o.DeliveryState,o.DeliveryZip,o.Discount,o.Expiration,o.Greetings,o.Id AS orderId,o.Notes,o.OrderDate,o.Paid,o.PaymentMethod,o.Phone,o.RequiredDate,o.SecurityCode,o.[Statement],o.[Invoice],c.Address,c.Caterer,c.Cell,c.City,c.Email,c.FirstName,c.Id,c.LastName,c.Phone,c.State,c.Zip
                                    FROM Customers AS c
                                    JOIN Orders AS o
                                    ON  o.CustomerId= c.Id
                                    WHERE " + option + " AND " + com + @" (c.FirstName LIKE '%' + @query + '%'  OR c.LastName LIKE '%' + @query + '%' OR c.Phone LIKE '%' + @query + '%' OR c.Cell LIKE '%' + @query + '%' OR o.DeliveryOption LIKE '%' + @query + '%')
                                    ORDER BY c.LastName ASC, c.FirstName ASC, o.RequiredDate DESC";
                cmd.Parameters.AddWithValue("@query", search);
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
                    Customer c = GetCustomerById(oh.customerId);
                    oh.firstName = c.FirstName;
                    oh.lastName = c.LastName;
                    oh.caterer = c.Caterer;

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

        public double GetTotalByOrderId(int orderId, int customerId)
        {
            Customer c = GetCustomerById(customerId);
            double total = 0;
            using (var context = new CakesPosDataContext(_connectionString))
            {
                context.DeferredLoadingEnabled = false;
                IEnumerable<OrderDetail> orderDetails = context.OrderDetails.Where(od => od.OrderId == orderId).ToList();
                foreach (OrderDetail od in orderDetails)
                {
                    double catererDiscount = 0;
                    if (c.Caterer)
                    {
                        Product p = GetProductById(od.ProductId);
                        if (p.CategoryId == 1)
                        {
                            catererDiscount = 5;
                            total += (double)od.UnitPrice * (double)od.Quantity - (double)od.Quantity * catererDiscount;
                        }
                        else if (p.CategoryId == 2)
                        {
                            catererDiscount = (double)od.UnitPrice * (double)od.Quantity * 0.1;
                            total += ((double)od.Quantity * (double)od.UnitPrice - catererDiscount);
                        }
                        else if (p.CategoryId == 5)
                        {
                            catererDiscount = 2.5;
                            total += (double)od.UnitPrice * (double)od.Quantity - (double)od.Quantity * catererDiscount;
                        }
                    }
                    else
                    {
                        total += (double)od.UnitPrice * od.Quantity;
                    }
                }
            }
            return total;
        }

        public IEnumerable<InventoryViewModel> GetInventory(DateTime min, DateTime max)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            List<Order> orders = new List<Order>();
            List<InventoryViewModel> ivm = new List<InventoryViewModel>();
            List<Product> products = new List<Product>();
            using (var context = new CakesPosDataContext(_connectionString))
            {
                products = context.Products.ToList();
                orders = context.Orders.Where(o => o.RequiredDate > min && o.RequiredDate < max).ToList();

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


        public void UpdateOrderById(int orderId, int customerId, DateTime orderDate, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes, string greetings, string deliveryNote, bool paid)
        {
            ////int orderId = o;

            //Order o = new Order();
            //o.Id = orderId;
            //o.CustomerId = customerId;
            //o.OrderDate = orderDate;
            //o.RequiredDate = requiredDate;
            //o.DeliveryFirstName = deliveryFirstName;
            //o.DeliveryLastName = deliveryLastName;
            //o.DeliveryAddress = deliveryAddress;
            //o.DeliveryCity = deliveryCity;
            //o.DeliveryState = deliveryState;
            //o.DeliveryZip = deliveryZip;
            //o.Phone = phone;
            //o.CreditCard = creditCard;
            //o.Expiration = expiration;
            //o.SecurityCode = securityCode;
            //o.Discount = discount;
            //o.DeliveryOption = deliveryOpt;
            //o.PaymentMethod = paymentMethod;
            //o.Paid = paid;
            //o.Notes = notes;
            //o.Greetings = greetings;
            //o.DeliveryNote = deliveryNote;
            //Payment p = new Payment();
            //p.CustomerId = customerId;
            //p.PaymentMethod = paymentMethod;

            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Orders
                                  SET customerId=@customerId, orderDate=@orderDate, requiredDate=@requiredDate, deliveryFirstName=@deliveryFirstName, deliveryLastName=@deliveryLastName, deliveryAddress=@deliveryAddress, deliveryCity=@deliveryCity, deliveryState=@deliveryState, deliveryZip=@deliveryZip, phone=@phone, creditCard=@creditCard, expiration=@expiration, securityCode=@securityCode, discount=@discount, deliveryOption=@deliveryOpt, paymentMethod=@paymentMethod, paid=@paid, notes=@notes, greetings=@greetings, deliveryNote=@deliveryNote
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
                cmd.Parameters.AddWithValue("@phone", phone);
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


        public void MakePayment(int customerId, int orderId, decimal amount, string paymentNote)
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
            return c.Caterer;
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
                orders = context.Orders.Where(o => o.CustomerId == customerId && o.Statement == false).ToList();
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
            return sModel;
        }

        public IEnumerable<int> GetCustomerIdBySearch(string search)
        {
            List<int> customerIds = new List<int>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Customers 
                                WHERE FirstName LIKE '%' + @query + '%'  OR LastName LIKE '%' + @query + '%'";
                cmd.Parameters.AddWithValue("@query", search);
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
            return sModel;
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
                context.SubmitChanges();
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

        public void DeductFromAccount(int customerId, decimal amount)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Customers
                                  SET Account -= @amount
                                  WHERE Id=@customerid";
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("customerid", customerId);
                connection.Open();
                cmd.ExecuteScalar();
            }
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
                                  WHERE Customers.Caterer=1 AND Orders.[Statement]!=1";
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

                    oh.total = (decimal)GetTotalByOrderId(oh.id, oh.customerId);
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
                                  WHERE Customers.Id =@Id AND Orders.[Statement]!=1";
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

                    oh.total = (decimal)GetTotalByOrderId(oh.id, oh.customerId);
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
                    oh.caterer = c.Caterer;

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

