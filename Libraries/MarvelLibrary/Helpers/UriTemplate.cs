// ******************************************************************
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

namespace MarvelLibrary.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A class that represents a Uniform Resource Identifier (URI) template.
    /// </summary>
    public class UriTemplate
    {
        private const string _parameterPattern = @"\{[a-z, A-Z, _, \[, \]]*\}";
        private const string _queryParameterPattern = @"[\?, &][a-z, A-Z, _, \[, \]]*=" + _parameterPattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="UriTemplate"/> class.
        /// </summary>
        /// <param name="template">The string template.</param>
        public UriTemplate(string template)
        {
            this.Template = template;
        }

        /// <summary>
        /// Gets the template string.
        /// </summary>
        public string Template { get; private set; }

        /// <summary>
        /// Creates a new URI from the template and the collection of parameters.
        /// </summary>
        /// <param name="baseUri">The base address.</param>
        /// <param name="parameters">A dictionary that contains a collection of parameter name/value pairs.</param>
        /// <returns>A URI.</returns>
        public Uri BindByName(Uri baseUri, IDictionary<string, string> parameters)
        {
            string pathSegment = this.Template.Split(new char[] { '?' })[0];
            string querySegment = "?" + this.Template.Split(new char[] { '?' })[1];

            // Substitute all variables in Path Segment
            foreach (string variable in parameters.Keys)
            {
                pathSegment = pathSegment.Replace("{" + variable + "}", parameters[variable]);
            }

            // There should be any unsubstituted variable in path segment anymore
            if (Regex.IsMatch(pathSegment, UriTemplate._parameterPattern))
            {
                throw new ArgumentException("One or more path segment parameter values were missing. All path segment parameters must be substituted.");
            }

            // Query Segment
            foreach (string variable in parameters.Keys)
            {
                querySegment = querySegment.Replace("{" + variable + "}", parameters[variable]);
            }

            // Remove unsubstituted query parameter "parameter=value" pairs
            foreach (Match match in Regex.Matches(querySegment, UriTemplate._queryParameterPattern))
            {
                querySegment = querySegment.Replace(match.Value, string.Empty);
            }

            // If the first query parameter was missing we ended up removing the '?' separator and have an extraneous '&'
            if (querySegment.StartsWith("&"))
            {
                querySegment = "?" + querySegment.Substring(1);
            }

            return new Uri(baseUri, pathSegment + querySegment);
        }
    }
}
