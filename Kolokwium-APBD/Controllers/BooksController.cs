using Kolokwium_APBD.Models;
using Kolokwium_APBD.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium_APBD.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBooksAuthorsService _booksService;
    
    public BooksController(IBooksAuthorsService booksService)
    {
        _booksService = booksService;
    }
    
    
    [HttpGet("{id}/authors")]
    public async Task<ActionResult<BooksAuthorsService>> ShowAuthors(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var authors = await _booksService.GetInfo(id);
            if (authors == null)
            {
                return NotFound();
            }
            return Ok(authors);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost]
    public async Task<ActionResult<BooksAuthorsService>> CreateNewBookAndAddAuthors([FromBody] BooksAuthorsNoId bookAuthors)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var addedProduct = await _booksService.CreateNewBookAndAuthors(bookAuthors);
            if (addedProduct == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(CreateNewBookAndAddAuthors), new {tittle = addedProduct.IdBook}, addedProduct);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
   
}