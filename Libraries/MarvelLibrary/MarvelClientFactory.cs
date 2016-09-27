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
    /// <summary>
    /// Marvel Client Factory class.
    /// </summary>
    public static class MarvelClientFactory
    {
        /// <summary>
        /// Create The Marvel Platform client
        /// </summary>
        /// <returns>Marvel client core object.</returns>
        public static IMarvelClient CreateMarvelClient(string publicKey, string privateKey)
        {
            return new MarvelClient(publicKey, privateKey);
        }
    }
}
