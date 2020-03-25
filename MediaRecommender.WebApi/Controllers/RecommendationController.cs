using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MediaRecommender.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        #region Fields

        private readonly ILogger<RecommendationController> _logger;

        #endregion

        #region Constructor

        public RecommendationController(ILogger<RecommendationController> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public Methods

        [HttpGet, Route("movie")]
        public IActionResult GetMovieRecommendation()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet, Route("tvshow")]
        public IActionResult GetTVShowRecommendation()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet, Route("documentary")]
        public IActionResult GetDocumentaryRecommendation()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        #endregion
    }
}
