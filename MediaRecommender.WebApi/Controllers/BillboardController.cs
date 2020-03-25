using MediaRecommender.Application.Contracts;
using MediaRecommender.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MediaRecommender.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillboardController : ControllerBase
    {
        #region Fields

        private readonly ILogger<BillboardController> _logger;
        private readonly IBillboardService _billboardService;

        #endregion

        #region Constructor

        public BillboardController(ILogger<BillboardController> logger, IBillboardService billboardService)
        {
            _logger = logger;
            _billboardService = billboardService;
        }

        #endregion

        #region Public Methods

        [HttpGet]
        public IActionResult GetBillboard()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet, Route("intelligent")]
        public async Task<IActionResult> GetIntelligentBillboard([FromQuery] IntelligentBillboardParameters parameters)
        {
            try
            {
                var result = await _billboardService.GetIntelligentBillboardAsync(parameters.From, parameters.NumberOfWeeks, parameters.NumberOfBigRooms,
                    parameters.NumberOfSmallRooms, parameters.City);

                if(result != null)
                {
                    return Ok(result);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { Error = $"There has been an error getting the intelligent billboard.", Params = parameters });
            }
            catch (Exception ex)
            {
                string message = $"There has been an error getting the intelligent billboard for [{parameters}].";
                _logger.LogError(ex, message);

                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { Error = $"There has been an error getting the intelligent billboard.", Params = parameters });
            }
        }

        #endregion
    }
}
