using Microsoft.Data.SqlClient;
using myAuth.Abstraction;
using Dapper;
using myAuth.Models;
using Azure.Core;
using OfficeOpenXml;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace myAuth.Service
{
    public class AuthService : IService
    {
        IConfiguration configuration;
        public AuthService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool ChangePassword(ChangePsw psw)
        {
            using (SqlConnection db = new SqlConnection(configuration["db"]))
            {
                
                var res = db.Query<dynamic>("pUserChangePassword", new {@login = psw.login, @oldPsw = psw.old_psw, @newPsw = psw.new_psw }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return res != null ? true : false;
            }
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

        private byte[] GenerateExcelFile(IEnumerable<RoleResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                // Создаем лист в Excel-файле
                var worksheet = package.Workbook.Worksheets.Add("Roles");

                // Записываем заголовки
                worksheet.Cells[1, 1].Value = "Role ID";
                worksheet.Cells[1, 2].Value = "Role Name";

                // Записываем данные
                int row = 2;
                foreach (var role in data)
                {
                    worksheet.Cells[row, 1].Value = role.id;
                    worksheet.Cells[row, 2].Value = role.name;
       
                    row++;
                }

                // Автоширина колонок
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Возвращаем содержимое Excel-файла в виде массива байтов
                return package.GetAsByteArray();
            }
        }
    }

}

