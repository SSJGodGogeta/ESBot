using ESBot.Domain.Entities;

namespace ESBot.API.Filter.Entities;

public class EvaluationResultFilter : IEntityFilter<EvaluationResult>
{
    /*
    public int? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? NameLike { get; set; }
    public int? ProductId { get; set; }
    public int? ShelfId { get; set; }
    public bool IncludeProducts { get; set; } = false;
    public bool IncludeShelfs { get; set; } = false;
    */

    public async Task<List<EvaluationResult>> Apply(IQueryable<EvaluationResult> query)
    {
        // Example for filter
        /*
        if(CategoryId.HasValue) query = query.Where(c => c.CategoryId == CategoryId);
        if(ProductId.HasValue) query = query.Where(c => c.Products.Any(p => p.ProductId == ProductId));
        if (ShelfId.HasValue) query = query.Where(c => c.Shelfs.Any(s => s.ShelfId == ShelfId));

        if (!string.IsNullOrWhiteSpace(Name))
            query = query.Where(c => c.Name == Name);
        if (!string.IsNullOrWhiteSpace(NameLike))
            query = query.Where(c => EF.Functions.Like(c.Name.ToLower(), $"%{NameLike.ToLower()}%"));

        List<EvaluationResult> categories =  await query.ToListAsync();
        if (ProductId.HasValue)
        {
            categories.ForEach(i => i.Products = i.Products
                .Where(p => p.ProductId == ProductId.Value)
                .ToList());
        }
        else if (!IncludeProducts)
        {
            categories.ForEach(i => i.Products = []);
        }

        if (ShelfId.HasValue)
        {
            categories.ForEach(i => i.Shelfs = i.Shelfs
                .Where(s => s.ShelfId == ShelfId.Value)
                .ToList());
        }
        else if (!IncludeShelfs)
        {
            categories.ForEach(i => i.Shelfs = []);
        }
        return categories;
    }
    */
        return new List<EvaluationResult>();
    }
}