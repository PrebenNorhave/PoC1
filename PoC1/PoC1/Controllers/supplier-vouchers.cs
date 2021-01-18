using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

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
        public async Task<Rootobject> Get()
        {
            HttpContext.Response.Headers.Add("Content-type", "application/json");
            StringValues econToken = string.Empty;
            if (!HttpContext.Request.Headers.TryGetValue(EconTokenHeaderName, out econToken))
            {
                //return EconTokenHeaderName + " not found";
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
                        var content = await response.Content
                            .ReadAsStringAsync();

                        var deserializeObject = JsonConvert.DeserializeObject<Rootobject>(content);
                        return deserializeObject;
                    }
                }
            }
        }

        [Route("old")]
        [HttpGet]
        public async Task<Dictionary<string, string>> GetOLD()
        {
            HttpContext.Response.Headers.Add("Content-type", "application/json");
            StringValues econToken = string.Empty;
            if (!HttpContext.Request.Headers.TryGetValue(EconTokenHeaderName, out econToken))
            {
                //return EconTokenHeaderName + " not found";
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
                        var content = await response.Content
                            .ReadAsStringAsync();
                        //var deserializeObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                        var deserializeObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                        return deserializeObject;
                        //return JsonConvert.SerializeObject(s);
                        //return await response.Content
                        //    .ReadAsStringAsync(); // here we return the json response, you may parse it
                    }
                }
            }
        }
    }


    public class Rootobject
    {
        public Collection[] collection { get; set; }
        public Pagination pagination { get; set; }
        public string self { get; set; }
    }

    public class Pagination
    {
        public int pageSize { get; set; }
        public int skipPages { get; set; }
        public int results { get; set; }
        public string firstPage { get; set; }
        public object nextPage { get; set; }
        public string lastPage { get; set; }
        public object previousPage { get; set; }
    }

    public class Collection
    {
        public int setupId { get; set; }
        public int voucherId { get; set; }
        public string prefix { get; set; }
        public int voucherNumber { get; set; }
        public int supplierNumber { get; set; }
        public string invoiceNumber { get; set; }
        public string date { get; set; }
        public string latestDueDate { get; set; }
        public string vatDate { get; set; }
        public string bookingDate { get; set; }
        public string text { get; set; }
        public float totalAmount { get; set; }
        public float totalAmountInBaseCurrency { get; set; }
        public string currency { get; set; }
        public float remainder { get; set; }
        public bool isBooked { get; set; }
        public object attachmentPictureNumber { get; set; }
        public long? lastUpdatedInTicks { get; set; }
        public Line[] lines { get; set; }
        public string self { get; set; }
    }

    public class Line
    {
        public int setupId { get; set; }
        public int voucherId { get; set; }
        public int voucherLineNumber { get; set; }
        public string dueDate { get; set; }
        public string text { get; set; }
        public float amount { get; set; }
        public float amountInBaseCurrency { get; set; }
        public int? accountNumber { get; set; }
        public bool isVatLine { get; set; }
        public string vatCode { get; set; }
        public string self { get; set; }
    }

}
