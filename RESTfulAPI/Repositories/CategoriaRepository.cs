using Microsoft.EntityFrameworkCore;

using RESTfulAPI.Context;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;
public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext dbContext;
    public CategoriaRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<IEnumerable<Categoria>> GetCategorias()
    {
        return await dbContext.Categorias.ToListAsync();
    }
}
