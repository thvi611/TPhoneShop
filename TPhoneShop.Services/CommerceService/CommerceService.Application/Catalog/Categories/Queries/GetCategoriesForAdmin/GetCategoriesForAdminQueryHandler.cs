using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Infrastructure.Extensions;
using CommerceService.Application.Catalog.Categories.Queries.Dtos;
using CommerceService.Application.Common.Abstractions;

namespace CommerceService.Application.Catalog.Categories.Queries.GetCategoriesForAdmin
{
    internal class GetCategoriesForAdminQueryHandler(CommerceDbContext dbContext, IMediaService mediaService) : IRequestHandler<GetCategoriesForAdminQuery, PagingResponse<CategoryForAdminDto>>
    {
        public async Task<PagingResponse<CategoryForAdminDto>> Handle(GetCategoriesForAdminQuery request, CancellationToken cancellationToken)
        {
            var query = dbContext.Categories
                                 .AsNoTracking()
                                 .WhereIf(!string.IsNullOrEmpty(request.Search), e => EF.Functions.ILike(e.Name, $"%{request.Search}%"))
                                 .Where(e => e.IsActive == request.IsActive)
                                 .OrderBy(e => e.Name)
                                 .Select(e => new
                                 {
                                     e.Id,
                                     e.ParentId,
                                     e.Name,
                                     e.Slug,
                                     e.Description,
                                     e.IsActive
                                 });
            var totalCount = await query.CountAsync(cancellationToken);
            var categorys = await query.Paginate(request.PageNumber, request.PageSize)
                                    .ToListAsync(cancellationToken);

            var items = await Task.WhenAll(categorys.Select(async category => new CategoryForAdminDto
            {
                Id = category.Id,
                ParentId = category.ParentId,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                IsActive = category.IsActive
            }));

            return new PagingResponse<CategoryForAdminDto>
            {
                TotalCount = totalCount,
                Items = items
            };
        }
    }
}
