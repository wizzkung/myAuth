using Microsoft.Data.SqlClient;
using myAuth.Abstraction;
using Dapper;
using myAuth.Models;
using Azure.Core;

namespace myAuth.Service
{
    public class AuthService : IService
    {
        IConfiguration configuration;
        public AuthService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<RoleResponse> GetRoles()
        {
            using (SqlConnection db = new SqlConnection(configuration["db"]))
            {
                
                return db.Query<RoleResponse>("pRole", commandType: System.Data.CommandType.StoredProcedure);
                
            }
        }

        public bool LogIn(string login, string psw)
        {
            try
            {
                using (SqlConnection db = new SqlConnection(configuration["db"]))
                {
                    var result = db.QuerySingleOrDefault<int>(
                        "pUsers;3", new { @login = login, @psw = psw }, commandType: System.Data.CommandType.StoredProcedure);
                    return result == 1;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool SignIn(SignInRequest request)
        {
            using (SqlConnection db = new SqlConnection(configuration["db"]))
            {
                DynamicParameters p = new DynamicParameters(request);
               var res =  db.Query<dynamic>("pUsers;3", p, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return res != null ? true: false;
            }
        }

        public bool SignUp(SignUpRequest add)
        {
            using (SqlConnection db = new SqlConnection(configuration["db"]))
            {
                //DynamicParameters p = new DynamicParameters(add);
                var res = db.ExecuteScalar<string>("pUsers;2", new {@login = add.login, @psw = add.psw, @role = add.role }, commandType: System.Data.CommandType.StoredProcedure);
                return res == "1" ? false : true;

            }
        }
    }
}
