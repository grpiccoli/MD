using Microsoft.AspNetCore.Html;

namespace ConsultaMD.Models.VM
{
    public class ValidationEmailVM
    {
        public HtmlString Header { get; set; }
        public HtmlString Body { get; set; }
        public HtmlString ButtonTxt { get; set; }
        public HtmlString Url { get; set; }
        public string Color { get; set; }
        public string LogoId { get; set; }
    }
}
