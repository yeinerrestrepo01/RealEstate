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

        public PropertiesController(IPropertyService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePropertyRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var created = await _service.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.IdProperty }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePropertyRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var updated = await _service.UpdateAsync(id, req, ct);
            return Ok(updated);
        }

        [HttpPut("{id:int}/price")]
        [Authorize]
        public async Task<IActionResult> ChangePrice([FromRoute] int id, [FromBody] decimal price, CancellationToken ct)
        {
            var updated = await _service.ChangePriceAsync(id, price, ct);
            return Ok(updated);
        }

        [HttpPost("{id:int}/images")]
        [Authorize]
        public async Task<IActionResult> AddImage([FromRoute] int id, [FromBody] AddImageRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var img = await _service.AddImageAsync(id, req.File, req.Enabled, ct);
            return Ok(img);
        }

        [HttpPost("{id:int}/traces")]
        [Authorize]
        public async Task<IActionResult> AddTrace([FromRoute] int id, [FromBody] AddTraceRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var trace = await _service.AddTraceAsync(id, req, ct);
            return Ok(trace);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ListPropertiesQuery query, CancellationToken ct)
        {
            var result = await _service.ListAsync(query, ct);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var prop = await _service.GetAsync(id, ct);
            if (prop is null) return NotFound();
            return Ok(prop);
        }

        [HttpGet("{id:int}/traces")]
        public async Task<IActionResult> GetTraces([FromRoute] int id, CancellationToken ct)
        {
            var traces = await _service.GetTracesAsync(id);
            if (traces is null) return NotFound();
            return Ok(traces);
        }
    }
}