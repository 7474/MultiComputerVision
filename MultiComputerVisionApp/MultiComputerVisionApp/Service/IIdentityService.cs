using System;
using System.Collections.Generic;
using System.Text;

namespace MultiComputerVisionApp.Service
{
    public interface IIdentityService
    {
        string CreateAuthorizationRequest();
        string CreateLogoutRequest(string token);
        bool IsLoginCallback(string url);
        bool IsLogoutCallback(string url);
    }
}
