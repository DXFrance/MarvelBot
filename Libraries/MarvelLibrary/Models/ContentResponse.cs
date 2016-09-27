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

    public class ContentResponse
    {
        [JsonProperty("code")]
        public int code { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("copyright")]
        public string copyright { get; set; }

        [JsonProperty("attributionText")]
        public string attributionText { get; set; }

        [JsonProperty("attributionHTML")]
        public string attributionHTML { get; set; }

        [JsonProperty("etag")]
        public string etag { get; set; }

        [JsonProperty("data")]
        public Data data { get; set; }
    }
}
