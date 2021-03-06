﻿using DotnetSpiderLite.Html;

namespace DotnetSpiderLite.HtmlAgilityPack
{
    public class HtmlElementSelectorFactory : IHtmlElementSelectorFactory
    {
        public IHtmlElementSelector GetSelector(string html)
        {
            return new HtmlElementSelector(html);
        }
    }
}