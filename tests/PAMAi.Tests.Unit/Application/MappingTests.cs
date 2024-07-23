using Mapster;
using PAMAi.Application.Dto;

namespace PAMAi.Tests.Unit.Application;

internal partial class MappingTests
{
    [Test]
    public void PagedList_To_PagedListResponse()
    {
        List<Entity> entities =
        [
            new() { Id = 1, Name = "Socks", },
            new() { Id = 2, Name = "Shoes", },
        ];
        PagedList<Entity> records = new(entities, 10, 1, 2);
        var config = PagedListResponse<EntityDto>.FromPagedList<Entity>();

        PagedListResponse<EntityDto> result = records.Adapt<PagedListResponse<EntityDto>>(config);
        result.Data = records.Adapt<List<EntityDto>>();

        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Has.Count.EqualTo(records.Count));
            Assert.That(result.Metadata.CurrentPage, Is.EqualTo(records.CurrentPage));
            Assert.That(result.Metadata.TotalPages, Is.EqualTo(records.TotalPages));
            Assert.That(result.Metadata.PageSize, Is.EqualTo(records.PageSize));
            Assert.That(result.Metadata.TotalCount, Is.EqualTo(records.TotalCount));
            Assert.That(result.Metadata.HasPrevious, Is.EqualTo(records.HasPrevious));
            Assert.That(result.Metadata.HasNext, Is.EqualTo(records.HasNext));
        });
    }
}
