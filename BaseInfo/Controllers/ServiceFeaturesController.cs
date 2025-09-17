using Application.Common;
using Application.CQRS;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BaseInfo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceFeaturesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServiceFeaturesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام ویژگی‌های خدمات
        /// </summary>
        /// <returns>لیست ویژگی‌های خدمات</returns>
        [HttpGet]
        public async Task<ActionResult<List<ServiceFeatureDto>>> GetAll()
        {
            var query = new GetAllServiceFeaturesQuery();
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<ServiceFeatureDto>> { IsSuccess = true , Data = result });
        }

        /// <summary>
        /// دریافت لیست ویژگی‌های خدمات فعال
        /// </summary>
        /// <returns>لیست ویژگی‌های خدمات فعال</returns>
        [HttpGet("active")]
        public async Task<ActionResult<List<ServiceFeatureSimpleDto>>> GetActive()
        {
            var query = new GetActiveServiceFeaturesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// دریافت ویژگی خدمات بر اساس ID
        /// </summary>
        /// <param name="id">شناسه ویژگی خدمات</param>
        /// <returns>اطلاعات ویژگی خدمات</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceFeatureDto>> GetById(int id)
        {
            var query = new GetServiceFeatureByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"ServiceFeature with ID {id} not found.");

            return Ok(new ApiResult<ServiceFeatureDto> { IsSuccess = true, Data = result });
        }

        /// <summary>
        /// دریافت ویژگی‌های خدمات مربوط به یک عنوان اصلی خاص
        /// </summary>
        /// <param name="mainTitleId">شناسه عنوان اصلی</param>
        /// <param name="activeOnly">فقط ویژگی‌های فعال (پیش‌فرض false)</param>
        /// <returns>لیست ویژگی‌های خدمات مربوط به عنوان اصلی</returns>
        [HttpGet("by-maintitle/{mainTitleId}")]
        public async Task<ActionResult<List<MainTitleServiceFeatureDto>>> GetByMainTitle(
            int mainTitleId,
            [FromQuery] bool activeOnly = false)
        {
            var query = new GetServiceFeaturesByMainTitleQuery(mainTitleId, activeOnly);
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<MainTitleServiceFeatureDto>> { IsSuccess = true, Data = result });
        }

        /// <summary>
        /// دریافت عناوین اصلی مربوط به یک ویژگی خدمات خاص
        /// </summary>
        /// <param name="serviceFeatureId">شناسه ویژگی خدمات</param>
        /// <param name="activeOnly">فقط روابط فعال (پیش‌فرض false)</param>
        /// <returns>لیست عناوین اصلی مربوط به ویژگی خدمات</returns>
        [HttpGet("{serviceFeatureId}/maintitles")]
        public async Task<ActionResult<List<MainTitleServiceFeatureDto>>> GetMainTitlesByServiceFeature(
            int serviceFeatureId,
            [FromQuery] bool activeOnly = false)
        {
            var query = new GetMainTitlesByServiceFeatureQuery(serviceFeatureId, activeOnly);
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<MainTitleServiceFeatureDto>> { IsSuccess = true, Data = result });
        }

        /// <summary>
        /// دریافت تمام روابط بین عناوین اصلی و ویژگی‌های خدمات
        /// </summary>
        /// <returns>لیست تمام روابط</returns>
        [HttpGet("relations")]
        public async Task<ActionResult<List<MainTitleServiceFeatureDto>>> GetAllRelations()
        {
            var query = new GetAllMainTitleServiceFeaturesQuery();
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<MainTitleServiceFeatureDto>> { IsSuccess = true, Data = result });
        }

        /// <summary>
        /// ایجاد ویژگی خدمات جدید
        /// </summary>
        /// <param name="request">اطلاعات ویژگی خدمات</param>
        /// <returns>شناسه ویژگی خدمات ایجاد شده</returns>
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateServiceFeatureRequest request)
        {
            var command = new CreateServiceFeatureCommand(
                request.Name,
                request.Description,
                request.Code,
                request.Icon,
                request.Color,
                request.DisplayOrder
            );

            var serviceFeatureId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = serviceFeatureId },
                new { Id = serviceFeatureId, Message = "ServiceFeature created successfully" }
            );
        }

        /// <summary>
        /// ویرایش ویژگی خدمات
        /// </summary>
        /// <param name="id">شناسه ویژگی خدمات</param>
        /// <param name="request">اطلاعات جدید</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateServiceFeatureRequest request)
        {
            var command = new UpdateServiceFeatureCommand(
                id,
                request.Name,
                request.Description,
                request.Code,
                request.Icon,
                request.Color,
                request.DisplayOrder,
                request.IsActive
            );

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"ServiceFeature with ID {id} not found.");

            return Ok(new { Message = "ServiceFeature updated successfully" });
        }

        /// <summary>
        /// حذف ویژگی خدمات (Soft Delete)
        /// </summary>
        /// <param name="id">شناسه ویژگی خدمات</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteServiceFeatureCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"ServiceFeature with ID {id} not found.");

            return Ok(new { Message = "ServiceFeature deleted successfully" });
        }

        /// <summary>
        /// اختصاص ویژگی خدمات به عنوان اصلی
        /// </summary>
        /// <param name="request">اطلاعات ارتباط</param>
        /// <returns>شناسه ارتباط ایجاد شده</returns>
        [HttpPost("assign")]
        public async Task<ActionResult<int>> AssignToMainTitle([FromBody] AssignServiceFeatureRequest request)
        {
            var command = new AssignServiceFeatureToMainTitleCommand(
                request.MainTitleId,
                request.ServiceFeatureId,
                request.IsActive,
                request.DisplayOrder,
                request.Notes
            );

            var relationId = await _mediator.Send(command);

            return Ok(new
            {
                Id = relationId,
                Message = "ServiceFeature assigned to MainTitle successfully"
            });
        }

        /// <summary>
        /// به‌روزرسانی ارتباط بین عنوان اصلی و ویژگی خدمات
        /// </summary>
        /// <param name="relationId">شناسه ارتباط</param>
        /// <param name="request">اطلاعات جدید</param>
        /// <returns></returns>
        [HttpPut("relations/{relationId}")]
        public async Task<ActionResult> UpdateRelation(int relationId, [FromBody] UpdateServiceFeatureRelationRequest request)
        {
            var command = new UpdateMainTitleServiceFeatureCommand(
                relationId,
                request.IsActive,
                request.DisplayOrder,
                request.Notes
            );

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"Relation with ID {relationId} not found.");

            return Ok(new { Message = "ServiceFeature relation updated successfully" });
        }

        /// <summary>
        /// حذف ارتباط بین عنوان اصلی و ویژگی خدمات
        /// </summary>
        /// <param name="relationId">شناسه ارتباط</param>
        /// <returns></returns>
        [HttpDelete("relations/{relationId}")]
        public async Task<ActionResult> RemoveRelation(int relationId)
        {
            var command = new RemoveServiceFeatureFromMainTitleCommand(relationId);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"Relation with ID {relationId} not found.");

            return Ok(new { Message = "ServiceFeature relation removed successfully" });
        }
    }

    // ===== REQUEST MODELS =====

    /// <summary>
    /// مدل درخواست ایجاد ویژگی خدمات
    /// </summary>
    public class CreateServiceFeatureRequest
    {
        /// <summary>
        /// نام ویژگی خدمات
        /// </summary>
        [Required(ErrorMessage = "نام ویژگی خدمات الزامی است")]
        [MaxLength(200, ErrorMessage = "نام ویژگی خدمات نباید بیشتر از 200 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام ویژگی خدمات باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات ویژگی خدمات
        /// </summary>
        [MaxLength(500, ErrorMessage = "توضیحات نباید بیشتر از 500 کاراکتر باشد")]
        public string? Description { get; set; }

        /// <summary>
        /// کد ویژگی (اختیاری)
        /// </summary>
        [MaxLength(50, ErrorMessage = "کد نباید بیشتر از 50 کاراکتر باشد")]
        public string? Code { get; set; }

        /// <summary>
        /// آیکون یا نماد ویژگی
        /// </summary>
        [MaxLength(100, ErrorMessage = "آیکون نباید بیشتر از 100 کاراکتر باشد")]
        public string? Icon { get; set; }

        /// <summary>
        /// رنگ نمایش ویژگی (برای UI)
        /// </summary>
        [MaxLength(20, ErrorMessage = "رنگ نباید بیشتر از 20 کاراکتر باشد")]
        public string? Color { get; set; }

        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
        public int DisplayOrder { get; set; } = 0;
    }

    /// <summary>
    /// مدل درخواست ویرایش ویژگی خدمات
    /// </summary>
    public class UpdateServiceFeatureRequest
    {
        /// <summary>
        /// نام ویژگی خدمات
        /// </summary>
        [Required(ErrorMessage = "نام ویژگی خدمات الزامی است")]
        [MaxLength(200, ErrorMessage = "نام ویژگی خدمات نباید بیشتر از 200 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام ویژگی خدمات باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات ویژگی خدمات
        /// </summary>
        [MaxLength(500, ErrorMessage = "توضیحات نباید بیشتر از 500 کاراکتر باشد")]
        public string? Description { get; set; }

        /// <summary>
        /// کد ویژگی (اختیاری)
        /// </summary>
        [MaxLength(50, ErrorMessage = "کد نباید بیشتر از 50 کاراکتر باشد")]
        public string? Code { get; set; }

        /// <summary>
        /// آیکون یا نماد ویژگی
        /// </summary>
        [MaxLength(100, ErrorMessage = "آیکون نباید بیشتر از 100 کاراکتر باشد")]
        public string? Icon { get; set; }

        /// <summary>
        /// رنگ نمایش ویژگی (برای UI)
        /// </summary>
        [MaxLength(20, ErrorMessage = "رنگ نباید بیشتر از 20 کاراکتر باشد")]
        public string? Color { get; set; }

        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// وضعیت فعال/غیرفعال
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// مدل درخواست اختصاص ویژگی خدمات به عنوان اصلی
    /// </summary>
    public class AssignServiceFeatureRequest
    {
        /// <summary>
        /// شناسه عنوان اصلی
        /// </summary>
        [Required(ErrorMessage = "شناسه عنوان اصلی الزامی است")]
        [Range(1, int.MaxValue, ErrorMessage = "شناسه عنوان اصلی باید عددی مثبت باشد")]
        public int MainTitleId { get; set; }

        /// <summary>
        /// شناسه ویژگی خدمات
        /// </summary>
        [Required(ErrorMessage = "شناسه ویژگی خدمات الزامی است")]
        [Range(1, int.MaxValue, ErrorMessage = "شناسه ویژگی خدمات باید عددی مثبت باشد")]
        public int ServiceFeatureId { get; set; }

        /// <summary>
        /// وضعیت فعال/غیرفعال این ارتباط
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// ترتیب نمایش این ویژگی برای این عنوان اصلی
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// یادداشت یا توضیح خاص برای این ارتباط
        /// </summary>
        [MaxLength(500, ErrorMessage = "یادداشت نباید بیشتر از 500 کاراکتر باشد")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// مدل درخواست به‌روزرسانی ارتباط ویژگی خدمات
    /// </summary>
    public class UpdateServiceFeatureRelationRequest
    {
        /// <summary>
        /// وضعیت فعال/غیرفعال این ارتباط
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// ترتیب نمایش این ویژگی برای این عنوان اصلی
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// یادداشت یا توضیح خاص برای این ارتباط
        /// </summary>
        [MaxLength(500, ErrorMessage = "یادداشت نباید بیشتر از 500 کاراکتر باشد")]
        public string? Notes { get; set; }
    }
}