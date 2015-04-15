using Flurl;
using System.Linq;

namespace Paging
{
    public static class StringExtensions
    {
        public static string SetParam(this string url, string name, string value, bool condition = true)
        {
            return url != null && condition && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value)
                ? url.SetQueryParam(name, value).ToString()
                : url;
        }

        public static string RemoveParam(this string url, string name, bool condition = true)
        {
            return url != null && condition && !string.IsNullOrEmpty(name)
            ? url.RemoveQueryParam(name).ToString()
            : url;
        }

        public static string RemoveNullOrEmptyParam(this string url)
        {
            var urlBuilder = new Url(url);
            var names = urlBuilder.QueryParams.Where(o => o.Value == null
                || string.IsNullOrWhiteSpace(o.Value.ToString())).Select(o => o.Key)
                .ToList();
            foreach (var item in names)
            {
                urlBuilder.QueryParams.Remove(item);
            }
            return urlBuilder.ToString();
        }
    }
}