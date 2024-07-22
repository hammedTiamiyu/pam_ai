using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMAi.Application.Dto.Account;
public class PasswordResetRequest
{
    public string PhoneNumber { get; set; }
    public string ResetLink { get; set; }
}