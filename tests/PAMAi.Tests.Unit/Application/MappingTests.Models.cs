namespace PAMAi.Tests.Unit.Application;
internal partial class MappingTests
{
    internal class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    internal record EntityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
