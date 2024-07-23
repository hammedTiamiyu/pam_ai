using PAMAi.Application.Dto.SMS;

namespace PAMAi.Application.Services.Interfaces;
public interface ISmsRepository
{
    Task<Result<SmsResponse>> SendSmsAsync(SmsRequest message);
}
