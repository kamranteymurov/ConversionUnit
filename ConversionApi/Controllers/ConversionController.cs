using Microsoft.AspNetCore.Mvc;
using ConversionLibrary;

namespace ConversionApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConversionController : ControllerBase
    {
        [HttpGet(Name = "GetConversion")]
        public IEnumerable<Conversion> Get(double value, string unitFrom, string unitTo)
        {
            ConversionUnit.ConversionResult conversionResult = ConversionUnit.Convertion(value, unitFrom, unitTo);
            return Enumerable.Range(1, 1).Select(index => new Conversion
            {
                result = conversionResult.value,
                detail = conversionResult.detail
            })
            .ToArray();
        }
    }
}