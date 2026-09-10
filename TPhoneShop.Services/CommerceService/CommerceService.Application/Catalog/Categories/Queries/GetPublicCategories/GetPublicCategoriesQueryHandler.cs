using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Infrastructure.Extensions;
using CommerceService.Application.Catalog.Categories.Queries.Dtos;
using CommerceService.Application.Common.Abstractions;

namespace CommerceService.Application.Catalog.Categories.Queries.GetPublicCategories
{
    internal class GetPublicCategoriesQueryHandler(CommerceDbContext dbContext, IMediaService mediaService) : IRequestHandler<GetPublicCategoriesQuery, PagingResponse<CategoryDto>>
    {
        public async Task<PagingResponse<CategoryDto>> Handle(GetPublicCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = dbContext.Categories
                                 .AsNoTracking()
                                 .Where(e => e.IsActive)
                                 .OrderBy(e => e.Name)
                                 .Select(e => new
                                 {
                                     e.Id,
                                     e.ParentId,
                                     e.Name,
                                     e.Slug,
                                     e.Description
                                 });
            var totalCount = await query.CountAsync(cancellationToken);
            var categories = await query.Paginate(request.PageNumber, request.PageSize)
                                    .ToListAsync(cancellationToken);

            var items = await Task.WhenAll(categories.Select(async category => new CategoryDto
            {
                Id = category.Id,
                ParentId = category.ParentId,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description
            }));

            return new PagingResponse<CategoryDto>
            {
                TotalCount = totalCount,
                Items = items
            };

        }
    }
}
