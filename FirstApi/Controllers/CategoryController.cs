﻿using AutoMapper;
using FirstApi.Data.DAL;
using FirstApi.Dtos.CategoryDto;
using FirstApi.Dtos.ProductDto;
using FirstApi.Extention;
using FirstApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FirstApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public CategoryController(AppDbContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll(int page, string search)
        {
            var query = _context.Categories
                .Where(p => !p.IsDelete);

            CategoryListDto categoryListDto = new();
            categoryListDto.TotalCount = query.Count();
            categoryListDto.CurrentPage = page;


            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            categoryListDto.Items = query.Skip((page - 1) * 2)
                .Take(2)
                .Select(p => new CategoryListItemDto
                {
                    Name = p.Name,
                    Description = p.Description,
                    CreateDate = p.CreateDate,
                    UpdateDate = p.UpdateDate,
                    ImageUrl = "https://localhost:7076/img/" + p.ImageUrl
                }).ToList();

            List<CategoryListItemDto> categoryListItemDtos = new();



            return StatusCode(200, categoryListDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            Category category = _context.Categories
                .Include(c=>c.Products)
                .Where(p => !p.IsDelete)
                .FirstOrDefault(x => x.Id == id);
            if (category == null) return StatusCode(StatusCodes.Status404NotFound);

            CategoryReturnDto categoryReturnDto = _mapper.Map<CategoryReturnDto>(category);

            //CategoryReturnDto categoryReturnDto = new()
            //{
            //    Name = category.Name,
            //    Description = category.Description,
            //    CreateDate = category.CreateDate,
            //    UpdateDate = category.UpdateDate,
            //    ImageUrl = "https://localhost:7076/img/" + category.ImageUrl
            //};

            return Ok(categoryReturnDto);
        }

        [HttpPost]
        public IActionResult AddCategory([FromForm] CategoryCreateDto categoryCreateDto)
        {

            if (categoryCreateDto.Photo == null) return StatusCode(409);
            if (!categoryCreateDto.Photo.IsImage()) return BadRequest("photo type deyil");
            if (!categoryCreateDto.Photo.CheckSize(50)) return BadRequest("olcu boyukdur");





            Category newcategory = new()
            {
                Name = categoryCreateDto.Name,
                Description = categoryCreateDto.Description,
                ImageUrl = categoryCreateDto.Photo.SaveImage(_env, "img", categoryCreateDto.Photo.FileName)

            };
            _context.Categories.Add(newcategory);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created, newcategory);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, CategoryUpdateDto categoryUpdateDto)
        {
            bool isExist = _context.Categories.Any(c => c.Name.ToLower() == categoryUpdateDto.Name.ToLower() && c.Id != id);
            if (isExist)
            {
                return BadRequest("bu adli categori movcuddur");
            }
            var existCategory = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (existCategory == null) return NotFound();
            existCategory.Name = categoryUpdateDto.Name;
            existCategory.Description = categoryUpdateDto.Description;
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPatch]
        public IActionResult ChangeStatus(int id, string Description)
        {
            var existCategory = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (existCategory == null) return NotFound();
            existCategory.Description = Description;
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);
        }

    }
}
