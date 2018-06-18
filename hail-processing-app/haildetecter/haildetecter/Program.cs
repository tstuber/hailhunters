using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace haildetecter
{
    class Program
    {
        // Replace<Subscription Key> with your valid subscription key.
        const string subscriptionKey = "xxx";
        const string subscriptionKeyText = "yyy";

        // You must use the same region in your REST call as you used to
        // get your subscription keys. For example, if you got your
        // subscription keys from westus, replace "westcentralus" in the URL
        // below with "westus".
        //
        // Free trial subscription keys are generated in the westcentralus region.
        // If you use a free trial subscription key, you shouldn't need to change
        // this region.
        const string uriBase =
            "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/analyze";

        const string uriBaseText = "https://westeurope.api.cognitive.microsoft.com/text/analytics/v2.0";

        static void Main()
        {
            // Select text or image. 
            analyseText();

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }

        static void analyseText() {
            Console.WriteLine("Analyze text:");
            Console.Write("Enter the text you wish to analyze: ");
            string textToProcess = Console.ReadLine();

            MakeTextAnalysisRrequest(textToProcess).Wait();
        }

        static void analyseImage() { 

            // Get the path and filename to process from the user.
            Console.WriteLine("Analyze an image:");
            Console.Write("Enter the path to the image you wish to analyze: ");
            string imageFilePath = Console.ReadLine();

            if (File.Exists(imageFilePath))
            {
                // Make the REST API call.
                Console.WriteLine("\nWait a moment for the results to appear.\n");
                MakeAnalysisRequest(imageFilePath).Wait();
            }
            else
            {
                Console.WriteLine("\nInvalid file path");
            }

        }

        static async Task MakeTextAnalysisRrequest(string text) {

            try
            {
                HttpClient client = new HttpClient();
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKeyText);
                var uri = "https://westeurope.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases?" + queryString;
                HttpResponseMessage response;


                // Request body
                // byte[] byteData = Encoding.UTF8.GetBytes("{ \"documents\": [ { \"language\": \"en\", \"id\": \"10\", \"text\": \"" + text + "\" }  ]}");
                // byte[] byteData = Encoding.UTF8.GetBytes(" {\"documents\": [ { \"language\": \"en\", \"id\":\"1\", \"text\": \"Hello world.This is some input text that I love.\"}]} ");
                byte[] byteData = Encoding.UTF8.GetBytes(" {\"documents\": [ { \"language\": \"en\", \"id\":\"1\", \"text\": \"" + text + "\"}]} ");

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await client.PostAsync(uri, content);
                }      

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Console.WriteLine("\nResponse:\n\n{0}\n",
                    JToken.Parse(contentString).ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        /// <summary>
        /// Gets the analysis of the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file to analyze.</param>
        static async Task MakeAnalysisRequest(string imageFilePath)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters. A third optional parameter is "details".
                string requestParameters =
                    "visualFeatures=Categories,Description,Color";

                // Assemble the URI for the REST API Call.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Request body. Posts a locally stored JPEG image.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses content type "application/octet-stream".
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Make the REST API call.
                    response = await client.PostAsync(uri, content);
                }

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Console.WriteLine("\nResponse:\n\n{0}\n",
                    JToken.Parse(contentString).ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

    }
}
