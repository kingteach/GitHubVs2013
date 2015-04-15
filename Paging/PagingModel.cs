using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Paging
{
    public class PagingModel : IValidatableObject, IHtmlString
    {
        public PagingModel(
            int? pageIndex = null,
            int? pageSize = null,
            int? totalCount = null,
            int linkCount = 11,
            string prevDisplay = "<<",
            string nextDisplay = ">>")
        {
            PageSizeCollection = new int[] { 10, 20, 50 };
            DefaultPageSize = 10;
            PageIndex = pageIndex ?? 1;
            PageSize = pageSize ?? DefaultPageSize;
            DefaultLinkCount = linkCount;
            PrevDisplay = prevDisplay;
            NextDisplay = nextDisplay;
            TotalCount = totalCount ?? 0;
        }

        #region Properties

        [ScaffoldColumn(false)]
        public int[] PageSizeCollection { get; private set; }

        [ScaffoldColumn(false)]
        public int DefaultPageSize { get; private set; }

        [ScaffoldColumn(false)]
        public int DefaultLinkCount { get; set; }

        [ScaffoldColumn(false)]
        public int PageIndex { get; set; }

        [ScaffoldColumn(false)]
        public int PageSize { get; set; }

        [ScaffoldColumn(false)]
        public int TotalCount { get; set; }

        [ScaffoldColumn(false)]
        public int PageCount { get { return (int)Math.Ceiling(this.TotalCount / (double)this.PageSize); } }

        [ScaffoldColumn(false)]
        public string NextDisplay { get; set; }

        [ScaffoldColumn(false)]
        public string PrevDisplay { get; set; }

        [ScaffoldColumn(false)]
        public bool HasPrev
        {
            get { return PageIndex > 1; }
        }

        [ScaffoldColumn(false)]
        public bool HasNext
        {
            get
            {
                return PageIndex < PageCount;
            }
        }

        #endregion Properties

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PageSizeCollection.Any(o => o == PageSize))
            {
                yield return new ValidationResult("每页显示数量异常", new string[] { "PageSize" });
            }
        }

        public string GetPrevUrl(string url = null)
        {
            return this.GetUrl(PageIndex - 1, url);
        }

        public string GetNextUrl(string url = null)
        {
            return this.GetUrl(PageIndex + 1, url);
        }

        public IEnumerable<int> GetPageIndexs()
        {
            var left = DefaultLinkCount / 2;
            var start = PageIndex - left;
            start = start > 1 ? start : 1;
            var end = PageIndex + (DefaultLinkCount - left - 1);
            end = end > PageCount ? PageCount : end;
            if (start == 1 && end < PageCount)
            {
                while (end - start + 1 < DefaultLinkCount && end < PageCount)
                {
                    end++;
                }
            }
            if (end == PageCount && start > 1)
            {
                while (end - start + 1 < DefaultLinkCount && start > 1)
                {
                    start--;
                }
            }
            return this.GetIndexs(start, end);
        }

        private IEnumerable<int> GetIndexs(int start, int end)
        {
            var list = new List<int>();
            for (int i = start; i <= end; i++)
            {
                list.Add(i);
            }
            return list;
        }

        public string GetUrl(int i, string url = null)
        {
            url = url ?? HttpContext.Current.Request.Url.ToString();
            if (i == 1)
            {
                url = url.RemoveParam("PageIndex");
            }
            else
            {
                url = url.SetParam("PageIndex", i.ToString());
            }
            url = url.RemoveParam("PageSize", PageSize == DefaultPageSize);
            url = url.RemoveNullOrEmptyParam();
            return url;
        }

        public string ToHtmlString()
        {
            var tpl = "<a href=\"{0}\">{1}</a>";
            var ctpl = "<span class=\"current\">{0}</span>";

            var result = "<div class=\"pager\">";

            var links = this.GetPageIndexs();

            if (this.HasPrev)
            {
                result += string.Format(tpl, this.GetPrevUrl(), this.PrevDisplay);
            }

            if (PageIndex - 1 != 1 && !links.Any(o => o == 1))
            {
                result += string.Format(tpl, this.GetUrl(1), 1);
                result += "<span>...</span>";
            }
            foreach (var item in links)
            {
                if (item == PageIndex)
                {
                    result += string.Format(ctpl, item);
                }
                else
                {
                    result += string.Format(tpl, this.GetUrl(item), item);
                }
            }
            if (PageIndex + 1 != PageCount && !links.Any(o => o == PageCount))
            {
                result += "<span>···</span>";
                result += string.Format(tpl, this.GetUrl(PageCount), PageCount);
            }
            if (this.HasNext)
            {
                result += string.Format(tpl, this.GetNextUrl(), this.NextDisplay);
            }
            result += "</div>";
            return result;
        }
    }
}