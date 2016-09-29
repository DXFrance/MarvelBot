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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using MarvelLibrary.Helpers;
    using Newtonsoft.Json;

    /// <summary>
    /// Http simple service client. Implements exponential retry pattern.
    /// </summary>
    public class SimpleServiceClient : IDisposable
    {
        private HttpClient httpClient;
        private CancellationTokenSource cts;

        private int xRateLimitRemaining = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleServiceClient"/> class.
        /// </summary>
        public SimpleServiceClient()
        {
            httpClient = new HttpClient();
            cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Simple Http REST request. Implements exponential retry pattern.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="baseUri">The base address.</param>
        /// <param name="template">The string template.</param>
        /// <param name="parameters">A dictionary that contains a collection of parameter name/value pairs.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<T> GetWithRetryAsync<T>(Uri baseUri, UriTemplate template, Dictionary<string, string> parameters)
            where T : class, new()
        {
            Uri uri = template.BindByName(baseUri, parameters);
            string jsonContent = string.Empty;

            T content = null;

            if (xRateLimitRemaining < 10)
            {
                await Task.Delay(2000);
            }

            var response = await InvokeWebOperationWithRetryAsync(async () =>
            {
                IEnumerable<string> headers = null;
                var httpResponse = await httpClient.GetAsync(uri);

                if (httpResponse.Headers.TryGetValues("X-RateLimit-Remaining", out headers))
                {
                    xRateLimitRemaining = int.Parse(headers.First());
                    Debug.WriteLine(string.Format("X-RateLimit-Remaining: {0}", xRateLimitRemaining.ToString()));
                }

                httpResponse.EnsureSuccessStatusCode();
                return httpResponse;
            });

            jsonContent = await response.Content.ReadAsStringAsync();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            content = JsonConvert.DeserializeObject<T>(jsonContent, settings);

            return content;
        }

        /// <summary>
        /// Propagates notification that operations should be canceled.
        /// </summary>
        public void Cancel()
        {
            cts.Cancel();
            cts.Dispose();

            // Re-create the CancellationTokenSource.
            cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Use this method to close or release unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Use this method to close or release unmanaged resources.
        /// </summary>
        /// <param name="disposing">Boolean value.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }

                if (cts != null)
                {
                    cts.Dispose();
                    cts = null;
                }
            }
        }

        private static bool IsTransientException(Exception ex)
        {
            return true;
        }

        private static async Task<T> InvokeWebOperationWithRetryAsync<T>(Func<Task<T>> retriableOperation)
        {
            int baselineDelay = 1000;
            const int maxAttempts = 4;

            Random random = new Random();

            int attempt = 0;

            while (++attempt <= maxAttempts)
            {
                try
                {
                    return await retriableOperation();
                }
                catch (Exception ex)
                {
                    if (attempt == maxAttempts || !IsTransientException(ex))
                    {
                        throw;
                    }

                    int delay = baselineDelay + random.Next((int)(baselineDelay * 0.5), baselineDelay);

                    await Task.Delay(delay);

                    // Increment base-delay time
                    baselineDelay *= 2;
                }
            }

            // The logic above assures that this exception will never be thrown.
            throw new InvalidOperationException("This exception statement should never be thrown.");
        }
    }
}
