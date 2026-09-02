using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleProject.Data;
using SimpleProject.Data.Models;
using SimpleProject.Data.UnitOfWork;
using SimpleProject.DTOs;
using SimpleProject.Models;

namespace SimpleProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _db;

    public ItemsController(IUnitOfWork unitOfWork, AppDbContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Items
            .Include(x => x.Category)
            .ToListAsync();

        return Ok(items.Select(x => new
        {
            x.Id,
            x.Name,
            x.Description,
            x.Price,
            x.ImageUrl,
            x.CategoryId,
            CategoryName = x.Category.Name,
            x.CreatedAt
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _db.Items
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
            return NotFound();

        return Ok(new
        {
            item.Id,
            item.Name,
            item.Description,
            item.Price,
            item.ImageUrl,
            item.CategoryId,
            CategoryName = item.Category.Name,
            item.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateItemDto dtoItem)
    {
        string folderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "images"
        );

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string? imageUrl = null;

        if (dtoItem.Image != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dtoItem.Image.FileName);
            string filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dtoItem.Image.CopyToAsync(stream);

            imageUrl = "/images/" + fileName;
        }

        var item = new Item
        {
            Name = dtoItem.Name,
            Description = dtoItem.Description,
            Price = dtoItem.Price,
            CategoryId = dtoItem.CategoryId,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.Now
        };

        await _unitOfWork.Items.AddAsync(item);
        await _unitOfWork.SaveAsync();

        return Ok(item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateItemDto dtoItem)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);

        if (item == null)
            return NotFound();

        item.Name = dtoItem.Name;
        item.Description = dtoItem.Description;
        item.Price = dtoItem.Price;
        item.CategoryId = dtoItem.CategoryId;

        _unitOfWork.Items.Update(item);
        await _unitOfWork.SaveAsync();

        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id);

        if (item == null)
            return NotFound();

        _unitOfWork.Items.Delete(item);
        await _unitOfWork.SaveAsync();

        return Ok();
    }
}