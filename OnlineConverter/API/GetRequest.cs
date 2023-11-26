using System.Net;

namespace OnlineConverter.API
{
    public class GetRequest
    {
        HttpWebRequest _request;

        private string _address;

        public string Response {  get; set; }   

        public GetRequest(string addres)
        {
            _address = addres; 
        }

        public void Run()
        {
            _request = (HttpWebRequest)WebRequest.Create(_address);
            _request.Method = "GET";

            try
            {
                HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                var stream = response.GetResponseStream();

                if (stream != null)
                {
                    Response = new StreamReader(stream).ReadToEnd();
                }
            }
            catch (Exception)
            {               
            }
        }
    }
}
