using PAMAi.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAM_Ai.ExternalServices.Services.FcmNotifications;
internal class FcmService
{
    //private readonly FirebaseApp _firebaseApp;

    //public FcmService(string credentialsPath)
    //{
    //    _firebaseApp = FirebaseApp.Create(new AppOptions
    //    {
    //        Credential = GoogleCredential.FromFile(credentialsPath)
    //    });
    //}

    //public async Task<Result<string>> SendNotificationAsync(string title, string body, string token)
    //{
    //    var message = new Message
    //    {
    //        Notification = new Notification
    //        {
    //            Title = title,
    //            Body = body
    //        },
    //        Token = token
    //    };

    //    try
    //    {
    //        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
    //        return Result<string>.Success(response);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Result<string>.Failure(ex.Message);
    //    }
    //}
}
