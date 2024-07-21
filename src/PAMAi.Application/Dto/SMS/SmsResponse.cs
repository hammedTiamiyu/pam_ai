using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMAi.Application.Dto.SMS;
public class SmsResponse
{
    public string MessageId { get; set; }
    public string Message { get; set; }
    public int Balance { get; set; }
    public string User { get; set; }
}
