// ******************************************************************
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

namespace MarvelLibrary.Models
{
    using Newtonsoft.Json;
    using System;

    public class Comic
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("digitalId")]
        public int DigitalId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("issueNumber")]
        public int IssueNumber { get; set; }

        [JsonProperty("variantDescription")]
        public string VariantDescription { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        [JsonProperty("isbn")]
        public string Isbn { get; set; }

        [JsonProperty("upc")]
        public string Upc { get; set; }

        [JsonProperty("diamondCode")]
        public string DiamondCode { get; set; }

        [JsonProperty("ean")]
        public string Ean { get; set; }

        [JsonProperty("issn")]
        public string Issn { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("pageCount")]
        public int PageCount { get; set; }

        [JsonProperty("textObjects")]
        public TextInfo[] TextObjects { get; set; }

        [JsonProperty("resourceURI")]
        public string resourceURI { get; set; }

        [JsonProperty("urls")]
        public Url[] urls { get; set; }

        [JsonProperty("prices")]
        public PriceInfo[] Prices { get; set; }

        [JsonProperty("thumbnail")]
        public Image Thumbnail { get; set; }

        [JsonProperty("images")]
        public Image[] Images { get; set; }

        [JsonProperty("series")]
        public ResourceList Series { get; set; }

        [JsonProperty("creators")]
        public ResourceList Creators { get; set; }

        [JsonProperty("characters")]
        public ResourceList Characters { get; set; }

        [JsonProperty("stories")]
        public ResourceList Stories { get; set; }

        [JsonProperty("events")]
        public ResourceList Events { get; set; }
    }
}
