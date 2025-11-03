
namespace Backend.Models
{
    public class DeleteRequest
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Code { get; set; } // 1=Deleted, 0=NotFound, -1=FK conflict

    }
}
