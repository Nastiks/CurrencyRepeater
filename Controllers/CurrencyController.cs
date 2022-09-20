using CurrencyRepeater.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace CurrencyRepeater.Controllers
{
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly string bankUrl;
        public CurrencyController(IConfiguration configuration)
        {
            bankUrl = configuration.GetValue<string>("BankURL");
        }


        [HttpGet("/currencies")]
        public async Task<IActionResult> GetCurrencies([FromQuery] int count, [FromQuery] int page = 1)
        {
            HttpClient client = new();

            BankResponce responce = await client.GetFromJsonAsync<BankResponce>(bankUrl);

            var result = responce.Valute.AsEnumerable();

            if (page > 1 && count > 0)
            {
                int start = count * (page - 1);
                result = result
                    .Skip(start)
                    .Take(count);
            }

            return Ok(result);
        }

        [HttpGet("/currency/{valute}")]
        public async Task<IActionResult> GetCurrency([FromRoute][Required] string valute)
        {
            HttpClient client = new();

            BankResponce responce = await client.GetFromJsonAsync<BankResponce>(bankUrl);

            bool exists = responce.Valute.TryGetValue(valute, out var result);

            if (!exists)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }            
        }
    }
}