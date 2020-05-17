using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MultiComputerVisionApp.Droid
{
    // https://docs.microsoft.com/en-us/xamarin/essentials/web-authenticator?context=xamarin%2Fxamarin-forms&tabs=android
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = CALLBACK_SCHEME)]
    public class WebAuthenticationCallbackActivity : Xamarin.Essentials.WebAuthenticatorCallbackActivity
    {
        const string CALLBACK_SCHEME = "xamarinessentials";
    }
}