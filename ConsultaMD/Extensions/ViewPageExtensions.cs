using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Text;
using System.Text.Encodings.Web;
using System.IO;

namespace ConsultaMD.Extensions
{
    public static class ViewPageExtensions
    {
        private const string SCRIPTBLOCK_BUILDER = "ScriptBlockBuilder";
        private const string STYLEBLOCK_BUILDER = "StyleBlockBuilder";
        private const string LO_SCRIPTBLOCK_BUILDER = "LO_ScriptBlockBuilder";
        private const string LO_STYLEBLOCK_BUILDER = "LO_StyleBlockBuilder";

        public static HtmlString ScriptBlock(this RazorPage webPage, Func<dynamic, HelperResult> template)
        {
            var sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            var encoder = (HtmlEncoder)webPage.ViewContext.HttpContext.RequestServices.GetService(typeof(HtmlEncoder));

            if (webPage.Context.Request.Headers["x-requested-with"] != "XMLHttpRequest")
            {
                var scriptBuilder = webPage.Context.Items[SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

                template.Invoke(null).WriteTo(tw, encoder);
                scriptBuilder.Append(sb.ToString());
                webPage.Context.Items[SCRIPTBLOCK_BUILDER] = scriptBuilder;

                return new HtmlString(string.Empty);
            }

            template.Invoke(null).WriteTo(tw, encoder);

            return new HtmlString(sb.ToString());
        }

        public static HtmlString WriteScriptBlocks(this RazorPage webPage)
        {
            var scriptBuilder = webPage.Context.Items[SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

            return new HtmlString(scriptBuilder.ToString());
        }

        public static HtmlString StyleBlock(this RazorPage webPage, Func<dynamic, HelperResult> template)
        {
            var sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            var encoder = (HtmlEncoder)webPage.ViewContext.HttpContext.RequestServices.GetService(typeof(HtmlEncoder));

            if (webPage.Context.Request.Headers["x-requested-with"] != "XMLHttpRequest")
            {
                var styleBuilder = webPage.Context.Items[STYLEBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

                template.Invoke(null).WriteTo(tw, encoder);
                styleBuilder.Append(sb.ToString());
                webPage.Context.Items[STYLEBLOCK_BUILDER] = styleBuilder;

                return new HtmlString(string.Empty);
            }

            template.Invoke(null).WriteTo(tw, encoder);

            return new HtmlString(sb.ToString());
        }

        public static HtmlString WriteStyleBlocks(this RazorPage webPage)
        {
            var styleBuilder = webPage.Context.Items[STYLEBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

            return new HtmlString(styleBuilder.ToString());
        }


        //LAYOUT

        public static HtmlString LO_ScriptBlock(this RazorPage webPage, Func<dynamic, HelperResult> template)
        {
            var sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            var encoder = (HtmlEncoder)webPage.ViewContext.HttpContext.RequestServices.GetService(typeof(HtmlEncoder));

            if (webPage.Context.Request.Headers["x-requested-with"] != "XMLHttpRequest")
            {
                var scriptBuilder = webPage.Context.Items[LO_SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

                template.Invoke(null).WriteTo(tw, encoder);
                scriptBuilder.Append(sb.ToString());
                webPage.Context.Items[LO_SCRIPTBLOCK_BUILDER] = scriptBuilder;

                return new HtmlString(string.Empty);
            }

            template.Invoke(null).WriteTo(tw, encoder);

            return new HtmlString(sb.ToString());
        }

        public static HtmlString LO_WriteScriptBlocks(this RazorPage webPage)
        {
            var scriptBuilder = webPage.Context.Items[LO_SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

            return new HtmlString(scriptBuilder.ToString());
        }

        public static HtmlString LO_StyleBlock(this RazorPage webPage, Func<dynamic, HelperResult> template)
        {
            var sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            var encoder = (HtmlEncoder)webPage.ViewContext.HttpContext.RequestServices.GetService(typeof(HtmlEncoder));

            if (webPage.Context.Request.Headers["x-requested-with"] != "XMLHttpRequest")
            {
                var styleBuilder = webPage.Context.Items[LO_STYLEBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

                template.Invoke(null).WriteTo(tw, encoder);
                styleBuilder.Append(sb.ToString());
                webPage.Context.Items[LO_STYLEBLOCK_BUILDER] = styleBuilder;

                return new HtmlString(string.Empty);
            }

            template.Invoke(null).WriteTo(tw, encoder);

            return new HtmlString(sb.ToString());
        }

        public static HtmlString LO_WriteStyleBlocks(this RazorPage webPage)
        {
            var styleBuilder = webPage.Context.Items[LO_STYLEBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

            return new HtmlString(styleBuilder.ToString());
        }
    }
}
