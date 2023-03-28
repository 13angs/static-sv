namespace static_sv.DTOs
{
    public class StaticfileQuery
    {
        public virtual string? Is { get; set; }
        public virtual string? Type { get; set; }
        public virtual long FolderId { get; set; }
        public virtual int Limit { get; set; }
        public virtual long StaticfileId { get; set; }
        public virtual long Timestamp { get; set; }
        public virtual string? Name { get; set; }
    }
}