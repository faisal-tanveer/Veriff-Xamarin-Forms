using System.Threading;
using System.Threading.Tasks;
using VeriffReleaseLibraryIOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using IpQualityBindings;
using System;


[assembly: Dependency(typeof(AppServices))]
namespace SampleApp.iOS.Native
{
    class AppServices : IAppServices
    {
        public static TaskCompletionSource<VeriffResult> veriffTask;
        public async Task<VeriffResponse> GetVeriffResponse(object req, View loader, string url)
        {
            try
            {
                veriffTask = new TaskCompletionSource<VeriffResult>();
                var session = string.IsNullOrWhiteSpace(url) ? await Client.RestClient.GetVeriffSession(req) : new VeriffResponse();

                var veriff = VeriffSdk.Shared;
                var branding = new VeriffBranding(Color.FromHex("#C4161C").ToUIColor(), new UIImage());
                branding.PrimaryTextColor = Color.FromHex("#C4161C").ToUIColor();
                branding.SecondaryTextColor = Color.FromHex("#666666").ToUIColor();
                branding.StatusBarColor = Color.FromHex("#FFFFFF").ToUIColor();
                branding.BackgroundColor = Color.FromHex("#FFFFFF").ToUIColor();
                branding.SetButtonCornerRadius(25f);
                branding.PrimaryButtonBackgroundColor = Color.FromHex("#C4161C").ToUIColor();
                var configuration = new VeriffConfiguration(branding: branding, languageLocale: new NSLocale("en"));
                veriff.Delegate = new VeriffDelegate();
                loader.IsVisible = false;
                veriff.StartAuthenticationWithSessionUrl(sessionUrl: string.IsNullOrWhiteSpace(url) ? session.verification.url : url, configuration: configuration);

                var veriffResult = await veriffTask.Task;
                if (veriffResult != null && veriffResult.Status == VeriffStatus.Done)
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
    public class VeriffDelegate : VeriffSdkDelegate
    {
        public override void SessionDidEndWithResult(VeriffResult result)
        {
            AppServices.veriffTask?.TrySetResult(result);
        }
    }
}