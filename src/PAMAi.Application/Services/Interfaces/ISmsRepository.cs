using PAMAi.Application.Dto.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMAi.Application.Services.Interfaces;
public interface ISmsRepository
{
    Task<Result<SmsResponse>> SendSmsAsync(SmsRequest message);
}
