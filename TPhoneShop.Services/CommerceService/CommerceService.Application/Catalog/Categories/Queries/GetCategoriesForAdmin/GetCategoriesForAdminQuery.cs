using BuildingBlocks.Application.Pagination;
using CommerceService.Application.Catalog.Categories.Queries.Dtos;

namespace CommerceService.Application.Catalog.Categories.Queries.GetCategoriesForAdmin
{
    public class GetCategoriesForAdminQuery : PagingQuery, IRequest<PagingResponse<CategoryForAdminDto>>
    {
        public string? Search { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
