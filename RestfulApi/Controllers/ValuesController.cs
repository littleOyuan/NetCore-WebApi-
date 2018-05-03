using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RestfulApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public ValuesController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }


        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //throw new Exception("Test Message");
            LogHelper.Debug("Test", "Log4net Test");

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {

            var key = "myKey";
            var valueString = await _distributedCache.GetStringAsync(key);
            //if (valueByte == null)
            //{
            //    await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes("world22222"), new DistributedCacheEntryOptions().SetSlidingExpiration(DateTimeOffset.Now.AddSeconds(3000)));
            //    valueByte = await _distributedCache.GetAsync(key);
            //}
            if (string.IsNullOrWhiteSpace(valueString))
            {
                await _distributedCache.SetStringAsync(key, "abc");

                valueString = await _distributedCache.GetStringAsync(key);
            }

            return valueString;

            //return  _distributedCache.GetAsync("myKey");
            //return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]RequestModel requestModel)
        {

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
