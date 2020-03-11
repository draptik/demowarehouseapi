using System.Collections.Generic;
using DemoWareHouseApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoWareHouseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        // GET: api/Product
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "test1", Price = 10 },
                new Product { Id = 2, Name = "test2", Price = 11 },
                new Product { Id = 3, Name = "test3", Price = 12 },
                new Product { Id = 4, Name = "test4", Price = 13 },
                new Product { Id = 5, Name = "test5", Price = 14 }
            };
        }

        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        public Product Get(int id)
        {
            return new Product { Id = id, Name = $"test{id}", Price = id * 10 };
        }

        // POST: api/Product
        [HttpPost]
        public void Post([FromBody] Product product)
        {
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Product product)
        {
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
