using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace PoC1.Controllers
{
    [ApiController]
    [Route("supplier-vouchers")]
    public class SupplierVouchersController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
    
        private const string EconTokenHeaderName = "Econ-Token";

        public readonly string VoucherPersistedCacheUrl;
        public const string VoucherPersistedCacheUrlKey = "App:VoucherPersistedCache";

        public SupplierVouchersController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            VoucherPersistedCacheUrl = configuration.GetValue<string>(VoucherPersistedCacheUrlKey);
        }

        [HttpGet]
        public async Task<string> Get()
        {
            StringValues econToken = string.Empty;
            if (!HttpContext.Request.Headers.TryGetValue(EconTokenHeaderName, out econToken))
            {
                return EconTokenHeaderName+" not found";
            }
            Uri uri = new Uri(VoucherPersistedCacheUrl + HttpContext.Request.QueryString);
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add(EconTokenHeaderName, econToken.ToString());
                    using (var response = await client.GetAsync(uri))
                    {
                        response.EnsureSuccessStatusCode();

                        return await response.Content
                            .ReadAsStringAsync(); // here we return the json response, you may parse it
                    }
                }
            }

            return string.Empty;
        }

        public Dictionary<string, string> ParseQueryString(string requestQueryString)
        {
            Dictionary<string, string> rc = new Dictionary<string, string>();
            string[] ar1 = requestQueryString.Split(new char[] { '&', '?' });
            foreach (string row in ar1)
            {
                if (string.IsNullOrEmpty(row)) continue;
                int index = row.IndexOf('=');
                if (index < 0) continue;
                rc[Uri.UnescapeDataString(row.Substring(0, index))] = Uri.UnescapeDataString(row.Substring(index + 1)); // use Unescape only parts          
            }
            return rc;
        }
    }
}
