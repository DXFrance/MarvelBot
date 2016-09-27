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
        private string _apiKey = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarvelClient"/> class.
        /// </summary>
        internal MarvelClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// Fetches lists of comic characters
        /// </summary>
        /// <returns>A <see cref="ContentResponse"/> object.</returns>
        public async Task<ContentResponse> GetCharactersAsync(string name)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("apikey", _apiKey);
            parameters.Add("name", Uri.EscapeDataString(name));

            var template = new UriTemplate("/v1/public/characters?nameStartsWith={name}&apikey={apikey}");

            return await GetWithRetryAsync<ContentResponse>(_hostUri, template, parameters);
        }
    }
}
