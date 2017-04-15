﻿using MySecondHand.Helpers.Interfaces;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using MySecondHand.Spider.Model;

namespace MySecondHand.Spider
{
    public class WpSpider
    {
        public const string BASE_URL = "www.wallapop.es";
        private IHtmlClientHelper _htmlClientHelper;

        private const string TITTLE_XPATH = ".//*[contains(@class, 'product-info-title')]";
        private const string CATEGORY_XPATH = ".//*[contains(@class, 'product-info-category')]";
        private const string PRICE_XPATH = ".//*[contains(@class, 'product-info-price')]";
        private const string IMAGE_XPATH = ".//img[contains(@class, 'card-product-image')]";


        public WpSpider(IHtmlClientHelper htmlClientHelper)
        {
            _htmlClientHelper = htmlClientHelper;
        }

        public HtmlDocument DoSearch(SearchParameter parameter)
        {
            var completeurl = ComposeSearchUrl(parameter);

            return _htmlClientHelper.GetInnerHtml(completeurl);
        }

        public string ComposeSearchUrl(SearchParameter parameter)
        {
            string searchParams = string.Empty;
            
            if (parameter != null)
            {
                searchParams = $"/search?kws={parameter.SearchKey}&lat={parameter.SearchKey}&lng={parameter.SearchKey}";
            }
            return string.Concat(BASE_URL, searchParams);
        }

        public IList<ProductItem> GetSearchedItems(HtmlDocument searchResult)
        {
            IList<ProductItem> items = new List<ProductItem>();

            var findItems = searchResult.DocumentNode
                .Descendants("div")
                .Where(d => d.Attributes["class"] != null && d.Attributes["class"].Value.Contains("card ") && d.Attributes["class"].Value.Contains("card-product ")
                );

            foreach (var documentNode in findItems)
            {
                if (IsValidNode(documentNode))
                {
                    var productItem = new ProductItem();
                    productItem.ItemName = documentNode.SelectNodes(TITTLE_XPATH).First().InnerHtml;
                    productItem.ItemLink = documentNode.SelectNodes(TITTLE_XPATH).First().GetAttributeValue("href", "");
                    productItem.ItemCategory = documentNode.SelectNodes(CATEGORY_XPATH).First().InnerHtml;
                    productItem.ItemCategoryLink = documentNode.SelectNodes(CATEGORY_XPATH).First().GetAttributeValue("href", "");
                    productItem.ItemPrice = documentNode.SelectNodes(PRICE_XPATH).First().InnerHtml;
                    productItem.ItemImage = documentNode.SelectNodes(IMAGE_XPATH)
                        .First()
                        .GetAttributeValue("src", "");
                    productItem.ItemHtml = documentNode.InnerHtml;

                    items.Add(productItem);
                }
            }

            return items;
        }

        private bool IsValidNode(HtmlNode node)
        {
            bool valid = false;

            if (node == null)
            {
                return valid;
            }
            else
            {
                valid = node.SelectNodes(TITTLE_XPATH) != null
                    && node.SelectNodes(CATEGORY_XPATH) != null
                    && node.SelectNodes(PRICE_XPATH) != null
                    && node.SelectNodes(IMAGE_XPATH) != null;

                return valid;
            }
        }
    }
}
