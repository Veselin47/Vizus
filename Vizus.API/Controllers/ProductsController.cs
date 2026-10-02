using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vizus.Application.Products.Commands;
using Vizus.Application.Products.Queries;

namespace Vizus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var products = await _mediator.Send(new GetAllProductsQuery(), ct);
        return Ok(products);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAll), new { id }, null);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateProductRequest request, CancellationToken ct)
    {
        await _mediator.Send(new UpdateProductCommand(
            id, request.Name, request.Description, request.Price, request.StockQuantity, request.CategoryId, request.ImageUrl), ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }

    public record UpdateProductRequest(string Name, string Description, decimal Price, int StockQuantity, int CategoryId, string? ImageUrl);
}