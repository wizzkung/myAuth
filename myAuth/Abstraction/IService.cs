using myAuth.Models;

namespace myAuth.Abstraction
{
    public interface IService
    {
        public bool SignUp(SignUpRequest Add);
        //public bool LogIn(string login, string psw);
        public bool SignIn(SignInRequest request);
        public IEnumerable<RoleResponse> GetRoles();
        public bool ChangePassword(ChangePsw psw);
    }
}
