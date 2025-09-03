using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerService _owners;
        public OwnersController(IOwnerService owners) => _owners = owners;

        /// <summary>Create a new Owner</summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOwnerRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var owner = await _owners.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = owner.IdOwner }, owner);
        }

        /// <summary>Get owner by id</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var owner = await _owners.GetAsync(id, ct);
            return owner is null ? NotFound() : Ok(owner);
        }

        /// <summary>List owners (optional filter by name)</summary>
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ListOwnersQuery query, CancellationToken ct)
        {
            var owners = await _owners.ListAsync(query, ct);
            return Ok(owners);
        }

        /// <summary>Update owner</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateOwnerRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var updated = await _owners.UpdateAsync(id, req, ct);
            return Ok(updated);
        }

        /// <summary>Delete owner (must not have properties)</summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            var ok = await _owners.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
