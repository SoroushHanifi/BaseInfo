using Application.CQRS;
using Application.Models;
using Domain.Entities.Daroo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseInfo.Controllers
{

    // ===== BpmType CONTROLLER =====
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BpmTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BpmTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ResultApi<List<BpmType>>>> GetAll()
        {
            try
            {
                var query = new GetAllBpmTypeQuery();
                var result = await _mediator.Send(query);
                return Ok(new ResultApi<List<BpmType>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست نوع فراند ها با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت لیست نوع فرایند ها: {ex.Message}"
                });
            }
        }
    }
}
