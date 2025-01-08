using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace myAuth.Models
{
    public class SignUpRequest
    {
        [Required, DisplayName("Логин")]
        public string login { get; set; }
        [Required, DisplayName("Пароль"), DataType(DataType.Password)]
        public string psw { get; set; }
        [Required, DisplayName("Подтвердите пароль"), DataType(DataType.Password), Compare("psw", ErrorMessage ="Пароль не совпадает")]
        public string psw2 { get; set; }
        [Required, DisplayName("Роль")]
        public string role { get; set; }
    }
}
