using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using EssentialPlateform = Xamarin.Essentials.Platform;

[assembly: Xamarin.Forms.Dependency(typeof(AppServices))]
namespace SampleApp.Droid.Native
{
    class AppServices : IAppServices
    {
        public static TaskCompletionSource<Com.Veriff.VeriffResult> veriffTask;
      
 
        public async Task<VeriffResponse> GetVeriffResponse(object req, Xamarin.Forms.View loader, string url)
        {
            try
            {
                veriffTask = new TaskCompletionSource<Com.Veriff.VeriffResult>();
                var session = string.IsNullOrWhiteSpace(url) ? await Client.RestClient.GetVeriffSession(req) : new VeriffResponse();

                var branding = new Com.Veriff.VeriffBranding.Builder()
                            .ThemeColor(EssentialPlateform.CurrentActivity.Resources.GetColor(Resource.Color.VerifthemeColor))
                            .BackgroundColor(EssentialPlateform.CurrentActivity.Resources.GetColor(Resource.Color.Veriflauncher_background))
                            .StatusBarColor(EssentialPlateform.CurrentActivity.Resources.GetColor(Resource.Color.Veriflauncher_background))
                            .PrimaryTextColor(EssentialPlateform.CurrentActivity.Resources.GetColor(Resource.Color.VerifprimarTextColor))
                            .SecondaryTextColor(EssentialPlateform.CurrentActivity.Resources.GetColor(Resource.Color.VerifsecondaryTextColor))
                            .ToolbarIcon(Resource.Drawable.icon_Address)
                            .ButtonCornerRadius(25f)
                            .Build();
                var configuration = new Com.Veriff.VeriffConfiguration.Builder().Branding(branding).Build();
                var intent = Com.Veriff.VeriffSdk.CreateLaunchIntent(EssentialPlateform.CurrentActivity, string.IsNullOrWhiteSpace(url) ? session.verification.url : url, configuration);
                
                EssentialPlateform.CurrentActivity.StartActivityForResult(intent, 1062);
                var veriffResult = await veriffTask.Task;
                if (veriffResult != null && veriffResult.GetStatus() == Com.Veriff.VeriffResult.Status.Done)
                {
                    session.code = 1;
                    return session;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
       
    }
     
}