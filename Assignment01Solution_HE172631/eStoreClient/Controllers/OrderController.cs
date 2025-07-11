﻿using BusinessObject.DTO;
using eStoreClient.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using eStoreClient.Utils;
using BusinessObject.Models;
using Newtonsoft.Json;

namespace eStoreClient.Controllers
{
    public class OrderController : Controller
    {
        private const string ORDER_ITEMS_KEY = "ORDER_ITEMS";

        private readonly HttpClient client = null;
        private string OrderApiUrl = "";
        private string OrderDetailApiUrl = "";
        private string MemberApiUrl = "";
        private string ProductApiUrl = "";
        public OrderController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "https://localhost:7139/api/Order";
            OrderDetailApiUrl = "https://localhost:7139/api/OrderDetail";
            MemberApiUrl = "https://localhost:7139/api/Member";
            ProductApiUrl = "https://localhost:7139/api/Product";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string role = HttpContext.Session.GetString("USERNAME");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to view your order history.";
                return RedirectToAction("Index", "Home", TempData);
            }
            else if (role != "admin@estore.com")
            {
                TempData["ErrorMessage"] = "You must login as a admin to view orders.";
                return RedirectToAction("Profile", "Member", TempData);
            }

            List<Order> listOrders = await ApiHandler.DeserializeApiResponse<List<Order>>(OrderApiUrl, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(listOrders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string role = HttpContext.Session.GetString("USERNAME");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to view your order history.";
                return RedirectToAction("Index", "Home", TempData);
            }


            List<Order> listOrders = await ApiHandler.DeserializeApiResponse<List<Order>>(OrderApiUrl + $"/Member/{userId.Value}", HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View("Index", listOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            string role = HttpContext.Session.GetString("USERNAME");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to view order details.";
                return RedirectToAction("Index", "Home", TempData);
            }

            Order order = await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl + "/" + id, HttpMethod.Get);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";

                if (role == "admin@estore.com")
                    return RedirectToAction("Index", "Order", TempData);
                else
                    return RedirectToAction("OrderHistory", "Order", TempData);
            }
            if (order.MemberId != userId.Value)
            {
                TempData["ErrorMessage"] = "You don't have permission to view this order.";
                return RedirectToAction("OrderHistory", "Order", TempData);
            }

            List<OrderDetail> listOrderDetails = await ApiHandler.DeserializeApiResponse<List<OrderDetail>>(OrderDetailApiUrl + $"/order/{id}", HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            ViewData["Order"] = order;
            ViewData["OrderDetails"] = listOrderDetails;

            return View("OrderDetail");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string role = HttpContext.Session.GetString("USERNAME");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to delete order";
                return RedirectToAction("Index", "Home", TempData);
            }
            //if (role == "Customer")
            else
            {
                TempData["ErrorMessage"] = "You don't have permission to delete order";
                return RedirectToAction("OrderHistory", "Order", TempData);
            }

            Order order = await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl + "/" + id, HttpMethod.Get);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index", "Order", TempData);
            }


            List<OrderDetail> listOrderDetails = await ApiHandler.DeserializeApiResponse<List<OrderDetail>>(OrderDetailApiUrl + $"/order/{id}", HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            ViewData["Order"] = order;
            ViewData["OrderDetails"] = listOrderDetails;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string orderIdStr)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string role = HttpContext.Session.GetString("ROLE");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to delete order";
                return RedirectToAction("Index", "Home", TempData);
            }
            else
            {
                TempData["ErrorMessage"] = "You don't have permission to delete order";
                return RedirectToAction("OrderHistory", "Order", TempData);
            }

            int orderId = int.Parse(orderIdStr);
            Order order = await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl + "/" + orderId, HttpMethod.Get);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index", "Order", TempData);
            }


            List<OrderDetail> listOrderDetails = await ApiHandler.DeserializeApiResponse<List<OrderDetail>>(OrderDetailApiUrl + $"/order/{order.OrderId}", HttpMethod.Get);

            foreach (OrderDetail orderDetail in listOrderDetails)
            {
                await ApiHandler.DeserializeApiResponse<OrderDetail>(OrderDetailApiUrl + "/" + orderDetail.OrderId + "/" + orderDetail.ProductId, HttpMethod.Delete);
            }

            await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl + "/" + order.OrderId, HttpMethod.Delete);

            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction("Index", "Order", TempData);
        }

        public async Task<IActionResult> Report(string startDate, string endDate)
        {
            List<Order> listOrders = await ApiHandler.DeserializeApiResponse<List<Order>>(OrderApiUrl, HttpMethod.Get);

            if (startDate != null && endDate == null)
            {
                DateOnly start = DateOnly.FromDateTime(DateTime.Parse(startDate));
                listOrders = listOrders.Where(o => o.OrderDate >= start).ToList();
            }
            else if (startDate == null && endDate != null)
            {
                DateOnly end = DateOnly.FromDateTime(DateTime.Parse(endDate));
                listOrders = listOrders.Where(o => o.OrderDate <= end).ToList();
            }
            else if (startDate != null && endDate != null)
            {
                 DateOnly start = DateOnly.FromDateTime(DateTime.Parse(startDate));
                DateOnly end = DateOnly.FromDateTime(DateTime.Parse(endDate));

                if (start > end)
                {
                    TempData["ErrorMessage"] = "Start date must be before end date.";
                    return RedirectToAction("Index", "Order", TempData);
                }

              

                listOrders = listOrders
                    .Where(o => o.OrderDate.HasValue &&
                                o.OrderDate.Value >= start &&
                                o.OrderDate.Value <= end)
                    .ToList();

            }
            else
            {
                TempData["ErrorMessage"] = "Please select a date range.";
                return RedirectToAction("Index", "Order", TempData);
            }

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;
            ViewData["Orders"] = listOrders;

            return View();
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to create order.";
                return RedirectToAction("Index", "Home", TempData);
            }

            List<Member> listMembers = await ApiHandler.DeserializeApiResponse<List<Member>>(MemberApiUrl, HttpMethod.Get);
            List<Product> listProducts = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            ViewData["OrderItems"] = GetOrderItems();
            ViewData["Member"] = listMembers;
            ViewData["Products"] = listProducts;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderRequest orderRequest)
        {
            List<OrderItemRequest> listItemsRequest = GetOrderItems();
            if (listItemsRequest.Count == 0)
            {
                TempData["ErrorMessage"] = "Order items are empty.";
                return RedirectToAction("Create", TempData);
            }
            int? userId = HttpContext.Session.GetInt32("USERID");
            string role = HttpContext.Session.GetString("USERNAME");
            int memberId = orderRequest.MemberId;
            if (role != "admin@estore.com")
            {
                memberId = HttpContext.Session.GetInt32("USERID").Value;
            }

            Order order = new Order()
            {
                MemberId = memberId,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                Freight = orderRequest.Freight,
            };
            Order orderSaved = await ApiHandler.DeserializeApiResponse<Order>(OrderApiUrl, HttpMethod.Post, order);

            foreach (OrderItemRequest itemRequest in listItemsRequest)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = orderSaved.OrderId,
                    ProductId = itemRequest.Product.ProductId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = itemRequest.Product.UnitPrice,
                    Discount = 0
                };
                await client.PostAsJsonAsync(OrderDetailApiUrl, orderDetail);
            }

            // Clear order items in Session
            ClearOrderItemsSession();

            if (role != "admin@estore.com")
            {
                TempData["SuccessMessage"] = "Create order successfully.";
                return RedirectToAction("OrderHistory", TempData);
            }
            else
            {
                TempData["SuccessMessage"] = "Create order successfully.";
                return RedirectToAction("Index", TempData);
            }
        }

        public async Task<IActionResult> AddOrderItem(OrderRequest orderRequest)
        {
            List<Product> listFlowerBouquets = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl, HttpMethod.Get);

            Product product = listFlowerBouquets.Where(p => p.ProductId == orderRequest.ProductId).FirstOrDefault();
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product doesn't exist.";
                return RedirectToAction("Create", TempData);
            }

            List<OrderItemRequest> listItemsRequest = GetOrderItems();
            OrderItemRequest itemRequest = listItemsRequest.Find(p => p.Product.ProductId == orderRequest.ProductId);
            if (itemRequest != null)
            {
                if (itemRequest.Quantity + orderRequest.Quantity > product.UnitsInStock)
                {
                    TempData["ErrorMessage"] = "Quantity exceeds the number of products in stock.";
                    return RedirectToAction("Create", TempData);
                }
                itemRequest.Quantity += orderRequest.Quantity;
            }
            else
            {
                if (orderRequest.Quantity > product.UnitsInStock)
                {
                    TempData["ErrorMessage"] = "Quantity exceeds the number of products in stock.";
                    return RedirectToAction("Create", TempData);
                }
                listItemsRequest.Add(new OrderItemRequest() { Quantity = orderRequest.Quantity, Product = product });
            }

            SaveOrderItemsSession(listItemsRequest);
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> RemoveOrderItem(OrderRequest orderRequest)
        {
            List<OrderItemRequest> listItemsRequest = GetOrderItems();
            listItemsRequest.RemoveAll(p => p.Product.ProductId == orderRequest.ProductId);
            SaveOrderItemsSession(listItemsRequest);
            return RedirectToAction("Create");
        }

        // Get list order items in Session
        private List<OrderItemRequest> GetOrderItems()
        {
            var session = HttpContext.Session;
            string jsonOrderItems = session.GetString(ORDER_ITEMS_KEY);
            if (jsonOrderItems != null)
            {
                return JsonConvert.DeserializeObject<List<OrderItemRequest>>(jsonOrderItems);
            }
            return new List<OrderItemRequest>();
        }

        private void SaveOrderItemsSession(List<OrderItemRequest> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(ORDER_ITEMS_KEY, jsoncart);
        }

        private void ClearOrderItemsSession()
        {
            var session = HttpContext.Session;
            session.Remove(ORDER_ITEMS_KEY);
        }
    }
}
