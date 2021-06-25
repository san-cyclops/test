using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestStub.Models;
using TestStub.Repository;

namespace TestStub.Controllers
{
    [Route("test/v1/[controller]")]
    [ApiController]
    public class TestOfferController : ControllerBase
    {
        private readonly ILogger<TestOfferController> _logger;
        private readonly ITestOfferRepository<IOffersBaseEntity> _TestOfferRepositoryCaller;

        public TestOfferController(ILogger<TestOfferController> logger, ITestOfferRepository<IOffersBaseEntity> TestOfferRepositoryCaller)
        {
            _logger = logger;
            _TestOfferRepositoryCaller = TestOfferRepositoryCaller;
        }

        [HttpGet]
        public async Task<List<OfferCompareResponse>> AuthToken()
        {
            return await Task.FromResult(await _TestOfferRepositoryCaller.OfferCompareCaller());
        }

        [Authorize]
        [HttpPost]
        [Route("JsonResponseFileUpload")]
        public async Task<IActionResult> TestJsonResponse(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            // full path to file in temp location
            var filePath = @"C:\FirstClassFlyer\Offer\11613829190669-717324448.json";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return Ok(new { count = files.Count, size, filePath });
        }

        [Authorize]
        [HttpPost]
        [Route("OfferDataFileUpload")]
        public async Task<IActionResult> TestOfferData(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            // full path to file in temp location
            var filePath = @"C:\FirstClassFlyer\Offer\OfferRawData.csv";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return Ok(new { count = files.Count, size, filePath });
        }

    }
}
