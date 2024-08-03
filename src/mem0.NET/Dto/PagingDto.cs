namespace mem0.Core;

public class PagingDto<T> 
{
    public long Total { get; set; }

    public List<T> Items { get; set; }
}