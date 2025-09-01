using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _service;
        public PropertiesController(IPropertyService service) => _service = service;

        /// <summary>Create Property Building</summary>
        [HttpPost]
        [Authorize] // protect mutations
        public async Task<IActionResult> Create([FromBody] CreatePropertyRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var created = await _service.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Add Image from property</summary>
        [HttpPost("{id:guid}/images")]
        [Authorize]
        public async Task<IActionResult> AddImage([FromRoute] Guid id, [FromBody] AddImageRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var img = await _service.AddImageAsync(id, req.Url, req.IsCover, ct);
            return Ok(img);
        }

        /// <summary>Change Price</summary>
        [HttpPut("{id:guid}/price")]
        [Authorize]
        public async Task<IActionResult> ChangePrice([FromRoute] Guid id, [FromBody] ChangePriceRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var updated = await _service.ChangePriceAsync(id, req.Price, ct);
            return Ok(updated);
        }

        /// <summary>Update property</summary>
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePropertyRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var updated = await _service.UpdateAsync(id, req, ct);
            return Ok(updated);
        }

        /// <summary>List property with filters</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] ListPropertiesQuery query, CancellationToken ct)
        {
            var result = await _service.ListAsync(query, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var prop = await _service.GetAsync(id, ct);
            if (prop is null) return NotFound();
            return Ok(prop);
        }
    }
}
