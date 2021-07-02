using MacDonalsApi.Data;
using MacDonalsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacDonalsApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ShoppingCartItemsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ShoppingCartItemsController(ApplicationContext Context)
        {
            _context = Context;
        }

        [HttpGet("{userid}")]
        public IActionResult Get(int userid)
        {
            var user = _context.shoppingCartItems.Where(x=>x.CustomerId == userid);
            if(user == null)
            {
                return NotFound("this user is not found");
            }
            else
            {
                var cartitems = from r in _context.shoppingCartItems.Where(x => x.CustomerId == userid)
                                join p in _context.products on r.ProductId equals p.Id
                                select new
                                {
                                    Id = r.Id,
                                    Price = r.Price,
                                    TotalAmount = r.TotalAmount,
                                    Qty = r.Qty,
                                    ProductName = p.Name 
                                };

                return Ok(cartitems);
            }
        
        }

        [HttpGet("{userid}")]
        public IActionResult SubTotal(int userid)
        {
            var subtotal = (from x in _context.shoppingCartItems
                            where x.CustomerId == userid
                            select x.TotalAmount).Sum();

            return Ok(new {SubTotal = subtotal });
        }

        [HttpGet("{userid}")]
        public IActionResult TotalItems(int userid)
        {
            var totalitem = (from x in _context.shoppingCartItems
                             where x.CustomerId == userid
                             select x.Qty).Sum();

            return Ok(new { TotalItems = totalitem});
        }

        [HttpPost]
        public IActionResult AddToCart([FromBody] ShoppingCartItem shoppingCartItem)
        {
            var cart = _context.shoppingCartItems.FirstOrDefault(x => x.CustomerId == shoppingCartItem.CustomerId && x.ProductId == shoppingCartItem.ProductId);
            if(cart != null)
            {
                cart.Qty += shoppingCartItem.Qty;
                cart.TotalAmount = shoppingCartItem.Price * shoppingCartItem.Qty;

            }
            else
            {
                var cartobject = new ShoppingCartItem
                {
                    CustomerId = shoppingCartItem.CustomerId,
                    ProductId = shoppingCartItem.ProductId,
                    Price = shoppingCartItem.Price,
                    Qty = shoppingCartItem.Qty,
                    TotalAmount = shoppingCartItem.TotalAmount
                };

                _context.shoppingCartItems.Add(cartobject);
                _context.SaveChanges();
            }
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{userid}")]
        public IActionResult DeleteFromCart(int userid)
        {
            var shoppingcart = _context.shoppingCartItems.Where(x=>x.CustomerId == userid);
            _context.shoppingCartItems.RemoveRange(shoppingcart);
            _context.SaveChanges();

            return Ok("Deleted Successufly");
        }
    }
}
