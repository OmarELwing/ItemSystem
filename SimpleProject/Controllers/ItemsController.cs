using Microsoft.AspNetCore.Mvc;
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

    public ItemsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _unitOfWork.Items.GetAllAsync(x => x.Category);

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
        var item = await _unitOfWork.Items.GetByIdAsync(id, x => x.Category);

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
        var category = await _unitOfWork.Categories.GetByIdAsync(dtoItem.CategoryId);

        if (category == null)
            return BadRequest("Category does not exist.");

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
            string fileName = Guid.NewGuid().ToString() +
                              Path.GetExtension(dtoItem.Image.FileName);

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

        var category = await _unitOfWork.Categories.GetByIdAsync(dtoItem.CategoryId);

        if (category == null)
            return BadRequest("Category does not exist.");

        item.Name = dtoItem.Name;
        item.Description = dtoItem.Description;
        item.Price = dtoItem.Price;
        item.CategoryId = dtoItem.CategoryId;

        if (dtoItem.Image != null)
        {
            string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images"
            );

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                string oldFilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    item.ImageUrl.TrimStart('/').Replace(
                        "/",
                        Path.DirectorySeparatorChar.ToString()
                    )
                );

                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            string fileName = Guid.NewGuid().ToString() +
                              Path.GetExtension(dtoItem.Image.FileName);

            string filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await dtoItem.Image.CopyToAsync(stream);

            item.ImageUrl = "/images/" + fileName;
        }

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

        if (!string.IsNullOrEmpty(item.ImageUrl))
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                item.ImageUrl.TrimStart('/').Replace(
                    "/",
                    Path.DirectorySeparatorChar.ToString()
                )
            );

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        _unitOfWork.Items.Delete(item);
        await _unitOfWork.SaveAsync();

        return Ok();
    }
}