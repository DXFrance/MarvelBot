// ******************************************************************
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

namespace MarvelLibrary
{
    using Helpers;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Core Marvel platform class. It exposes methods to call Marvel platform.
    /// </summary>
    public sealed class MarvelClient : SimpleServiceClient, IMarvelClient
    {
        private Uri _hostUri = new Uri("http://gateway.marvel.com");
        private string _publicKey = string.Empty;
        private string _privateKey = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarvelClient"/> class.
        /// </summary>
        internal MarvelClient(string publicKey, string privateKey)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        /// <summary>
        /// Fetches lists of comic characters
        /// </summary>
        /// <returns>A <see cref="CharactersResponse"/> object.</returns>
        public async Task<CharactersResponse> GetCharactersAsync(string name)
        {
            var timestamp = DateTime.Now.Ticks.ToString();
            var hash = timestamp + _privateKey + _publicKey;

            var parameters = new Dictionary<string, string>();
            parameters.Add("apikey", _publicKey);
            parameters.Add("timestamp", timestamp);
            parameters.Add("name", Uri.EscapeDataString(name));
            parameters.Add("hash", HashHelper.GetMd5Hash(hash));

            var template = new UriTemplate("/v1/public/characters?ts={timestamp}&name={name}&apikey={apikey}&hash={hash}");

            return await GetWithRetryAsync<CharactersResponse>(_hostUri, template, parameters);
        }

        /// <summary>
        /// Fetches lists of comics 
        /// </summary>
        /// <returns>A <see cref="ComicsResponse"/> object.</returns>
        public async Task<ComicsResponse> GetComicsAsync(DateTime startDate, DateTime endDate)
        {
            var timestamp = DateTime.Now.Ticks.ToString();
            var hash = timestamp + _privateKey + _publicKey;

            var parameters = new Dictionary<string, string>();
            parameters.Add("apikey", _publicKey);
            parameters.Add("timestamp", timestamp);
            parameters.Add("daterange", Uri.EscapeDataString($"{startDate.ToString("yyyy-MM-dd")},{endDate.ToString("yyyy-MM-dd")}"));
            parameters.Add("hash", HashHelper.GetMd5Hash(hash));

            //dateDescriptor = lastWeek
            //var template = new UriTemplate("/v1/public/comics?ts={timestamp}&format=comic&formatType=comic&dateRange={daterange}&apikey={apikey}&hash={hash}");
            var template = new UriTemplate("/v1/public/comics?ts={timestamp}&format=comic&formatType=comic&dateDescriptor=lastWeek&apikey={apikey}&hash={hash}");

            return await GetWithRetryAsync<ComicsResponse>(_hostUri, template, parameters);
        }
    }
}
