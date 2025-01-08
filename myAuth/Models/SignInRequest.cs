using System.Diagnostics.Eventing.Reader;

namespace myAuth.Models
{
    public class SignInRequest
    {
        public string login { get; set; }
        public string psw { get; set; }
    }
}
