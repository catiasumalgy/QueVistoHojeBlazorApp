﻿using RESTfulAPI.Entities;
using RESTfulAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace RESTfulAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

//[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaRepository categoriaRepository;

    public CategoriasController(ICategoriaRepository categoriaRepository)
    {
        this.categoriaRepository = categoriaRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categorias = await categoriaRepository.GetCategorias();
        return Ok(categorias);
    }
}