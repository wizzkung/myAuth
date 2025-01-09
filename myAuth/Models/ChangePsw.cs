using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace myAuth.Models
{
    public class ChangePsw
    {
        [Required, DisplayName("Логин")]
        public string login {  get; set; }
        [Required, DisplayName("Пароль"), DataType(DataType.Password)]
        public string old_psw { get; set; }
        [Required, DisplayName("Новый пароль"), DataType(DataType.Password)]
        public string new_psw { get; set; }
    }
}
