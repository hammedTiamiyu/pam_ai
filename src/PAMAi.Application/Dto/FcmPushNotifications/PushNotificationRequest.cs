﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMAi.Application.Dto.FcmPushNotifications;
public class PushNotificationRequest
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string Token { get; set; }
}
