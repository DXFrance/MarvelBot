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
    using Models;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface which exposes methods to call Marvel platform.
    /// </summary>
    public interface IMarvelClient
    {
        /// <summary>
        /// Fetches lists of comic characters
        /// </summary>
        /// <returns>A <see cref="CharactersResponse"/> object.</returns>
        Task<CharactersResponse> GetCharactersAsync(string name);

        /// <summary>
        /// Fetches lists of comics 
        /// </summary>
        /// <returns>A <see cref="ComicsResponse"/> object.</returns>
        Task<ComicsResponse> GetComicsAsync(DateTime startDate, DateTime endDate);
    }
}
