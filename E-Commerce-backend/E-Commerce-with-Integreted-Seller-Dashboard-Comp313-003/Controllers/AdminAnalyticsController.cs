using E_Commerce_Backend.Models;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Only admins can access
    public class AdminAnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminAnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/analytics/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
            var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders
            });
        }

        // GET: api/admin/analytics/best-selling-products
        [HttpGet("best-selling-products")]
        public async Task<IActionResult> GetBestSellingProducts()
        {
            var bestSellingProducts = await _context.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    TotalQuantitySold = g.Sum(od => od.Quantity),
                    TotalRevenueGenerated = g.Sum(od => od.Price * od.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(5)
                .ToListAsync();

            return Ok(bestSellingProducts);
        }

        // GET: api/admin/analytics/sales-report?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
        [HttpGet("sales-report")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var salesData = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(g => g.Date)
                .ToListAsync();

            return Ok(salesData);
        }

        // GET: api/admin/analytics/user-statistics
        [HttpGet("user-statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var userStats = await _context.Users
                .GroupBy(u => u.FirstName)
                .Select(g => new
                {
                    Date = g.Key,
                    NewUsers = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToListAsync();

            return Ok(userStats);
        }

        // GET: api/admin/analytics/order-status
        [HttpGet("order-status")]
        public async Task<IActionResult> GetOrderStatusDistribution()
        {
            var orderStatusData = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(orderStatusData);
        }
    }
}
