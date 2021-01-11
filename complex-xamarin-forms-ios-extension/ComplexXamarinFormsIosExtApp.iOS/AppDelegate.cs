using Foundation;
using Rg.Plugins.Popup;
using System;
using UIKit;
using Xamarin;

namespace ComplexXamarinFormsIosExtApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            Console.WriteLine("ContinueUserActivity called, App is being passed a task.");

            return base.ContinueUserActivity(application, userActivity, completionHandler);
        }
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Popup.Init();
            global::Xamarin.Forms.Forms.Init();

            IQKeyboardManager.SharedManager.Enable = true;

            LoadApplication(new App());

            #if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
            #endif
            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            Console.WriteLine("OnActivated called, App is active.");
            base.OnActivated(uiApplication);
        }
        public override void WillEnterForeground(UIApplication uiApplication)
        {
            Console.WriteLine("App will enter foreground");
            base.WillEnterForeground(uiApplication);

        }
        public override void OnResignActivation(UIApplication uiApplication)
        {
            Console.WriteLine("OnResignActivation called, App moving to inactive state.");
            base.OnResignActivation(uiApplication);
        }
        public override void DidEnterBackground(UIApplication uiApplication)
        {
            Console.WriteLine("App entering background state.");
            base.DidEnterBackground(uiApplication);
        }
        // not guaranteed that this will run
        public override void WillTerminate(UIApplication uiApplication)
        {
            Console.WriteLine("App is terminating.");
            base.WillTerminate(uiApplication);
        }
    }
}
