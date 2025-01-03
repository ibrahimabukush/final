using System.Linq.Expressions;
namespace eBook_Library_Service.Models
{
    public class QueryOptions<T> where T : class
    {
        public Expression<Func<T, object>> OrderBy { get; set; } = null;
        public Expression<Func<T, bool>> Where { get; set; }

        private string[] includes = Array.Empty<string>();
        public string Includes
        {
            set => includes = value.Replace(" ", "").Split(',');
        }
        public string[] GetIncludes() => includes;
        public bool HasWhere => Where != null;
        public bool HasOrderBy => OrderBy != null;
    }
}