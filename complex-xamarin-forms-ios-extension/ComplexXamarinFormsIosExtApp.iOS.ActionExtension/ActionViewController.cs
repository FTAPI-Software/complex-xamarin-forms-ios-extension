using ComplexXamarinFormsIosExtApp.Controllers;
using ComplexXamarinFormsIosExtApp.Interfaces;
using ComplexXamarinFormsIosExtApp.Pages;
using ComplexXamarinFormsIosExtApp.Utilities;
using Foundation;
using MobileCoreServices;
using NLog;
using Rg.Plugins.Popup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UIKit;
using Xamarin;
using Xamarin.Forms;

namespace ComplexXamarinFormsIosExtApp.iOS.ActionExtension
{
    public partial class ActionViewController : UIViewController
    {
        #region Fields

        private Logger log;

        private UIAlertController loadingUIAlertController;
        private UIViewController previousViewController;

        private UIViewController loginPageUIViewController;
        private LoginPageModal loginPage;

        private UIViewController mainPageUIViewController;
        private MainPageModal mainPage;

        private UIViewController errorPageUIViewController;

        private List<Stream> attachments = new List<Stream>();
        private int totalNumberOfAttachmentsLoaded;
        private int totalNumberOfAttachmentsToLoad;
        private CancellationTokenSource attachmentLoadCancellationTokenSource;

        #endregion

        #region Constructors

        public ActionViewController(IntPtr handle) : base(handle)
        {
            Popup.Init();
            Forms.Init();
            IQKeyboardManager.SharedManager.Enable = true;
            LoggerFactory.Initialize();

            //Register Dependencies manually for use by Xamarin dependency service. This avoids needing duplicate code with the namespace iOS.ActionExtension
            DependencyService.Register<ILogManager, IosLogManager>();
            DependencyService.Register<ISettingsManager, IosSharedSettingsManager>();

            this.log = LoggerFactory.GetCurrentClassLogger();
            LoggerFactory.ChangeMinimumLogLevel(LogLevel.Debug);

            log.Debug("Initialized Xamarin Forms");
        }

        #endregion

        #region Methods

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            log.Debug("DidReceiveMemoryWarning called");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Always adopt a light interface style.    
            OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;

            log.Debug("ViewDidLoad() base run");

            log.Info("ActionExtension Started");

            if (ExtensionContext.InputItems != null)
            {
                this.GetAttachments(ExtensionContext.InputItems);
            }
            else
            {
                log.Info("No Attachments to load!");
                this.DisplayLoginPage();
            }
        }

        private void GetAttachments(NSExtensionItem[] inputItems)
        {
            foreach (NSExtensionItem item in inputItems)
            {
                if (item.Attachments != null)
                {
                    log.Info("Found " + item.Attachments.Length + " potential Attachment(s)");

                    // Create Alert
                    loadingUIAlertController = UIAlertController.Create(NSBundle.MainBundle.GetLocalizedString("lblPreparingAttachmentsCaption"), "Preparing Attachments", UIAlertControllerStyle.Alert);
                    loadingUIAlertController.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;

                    //Add Action
                    this.attachmentLoadCancellationTokenSource = new CancellationTokenSource();

                    loadingUIAlertController.AddAction(UIAlertAction.Create(NSBundle.MainBundle.GetLocalizedString("btnCancel"), UIAlertActionStyle.Default, (UIAlertAction action) =>
                    {
                        this.attachmentLoadCancellationTokenSource.Cancel();
                        this.CloseExtension();
                    }));

                    // Present Alert
                    PresentViewController(loadingUIAlertController, false, () => {
                        LoadAttachments(item.Attachments);
                    });
                }
                else
                {
                    log.Info("No Attachments to load!");
                }
            }
        }

        public void LoadAttachments(NSItemProvider[] attachments)
        {
            List<NSItemProvider> compatibleAttachments = this.GetCompatibleAttachments(attachments);

            if (compatibleAttachments.Count > 0)
            {
                foreach (NSItemProvider itemProvider in compatibleAttachments)
                {
                    LoadAttachment(itemProvider);
                }
            }
            else
            {
                log.Info("No Compatible attachments to load!");
                this.AttachmentsLoaded();
            }
        }

        private void LoadAttachment(NSItemProvider itemProvider)
        {
            NSString typeOfItem = NSString.Empty;

            if (itemProvider.HasItemConformingTo(UTType.URL))
            {
                typeOfItem = UTType.URL;
            }
            else if (itemProvider.HasItemConformingTo(UTType.Image))
            {
                typeOfItem = UTType.Image;
            }

            if (typeOfItem != NSString.Empty)
            { 
                log.Debug("Calling Load Item..." + typeOfItem.ToString());
                itemProvider.LoadItemAsync(typeOfItem, null).ContinueWith((task) =>
                {
                    log.Debug("Load Item complete");
                    if (task != null && task.Result != null)
                    {
                        log.Debug("Processing Data");
                        ItemLoadCompletedUrl(task.Result);
                    }
                    else
                    {
                        log.Error("Error loading item. No result returned!");
                        this.totalNumberOfAttachmentsLoaded++;
                        CheckAttachmentsProcessed();
                    }
                });
            }
        }

        private List<NSItemProvider> GetCompatibleAttachments(NSItemProvider[] attachments)
        {
            List<NSItemProvider> compatibleAttachments = new List<NSItemProvider>();

            foreach (NSItemProvider itemProvider in attachments)
            {
                if (itemProvider.HasItemConformingTo(UTType.URL) || itemProvider.HasItemConformingTo(UTType.Image))
                {
                    log.Debug("Found URL or Image Attachment");
                    compatibleAttachments.Add(itemProvider);
                }
                else
                {
                    log.Info("Attachment " + itemProvider.SuggestedName + " is not Compatible type.");
                }
            }

            log.Debug("Found " + compatibleAttachments.Count + " compatible Attachment(s)");
            this.totalNumberOfAttachmentsToLoad = compatibleAttachments.Count;

            return compatibleAttachments;
        }

        private void ItemLoadCompletedUrl(NSObject url)
        {
            log.Debug("ItemLoadCompletedUrl called of type: " + url.GetType().ToString());

            try
            {
                NSUrl fileUrl = url as NSUrl;

                if (fileUrl != null)
                {
                    log.Debug("Loading Attachment: " + fileUrl.ToString());

                    NSData data = null;
                    string path = fileUrl.Path;
                    DateTime creationDate = DateTime.Now;
                    string fileName = Path.GetFileName(path);
                    string extension = Path.GetExtension(fileUrl.Path);

                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        try
                        {
                            NSFileAttributes attributes = NSFileManager.DefaultManager.GetAttributes(fileUrl.Path);
                            creationDate = (DateTime)attributes.CreationDate;
                        }
                        catch (Exception ex)
                        {
                            log.Debug(ex, "Unable to get Attributes for path: " + fileUrl.Path);
                        }

                        fileName = "FILE_" + creationDate.ToString("yyyyMMddTHHmmssFFF") + extension;
                    }
                    data = NSData.FromUrl(fileUrl);

                    Stream stream = data.AsStream();
                    attachments.Add(stream);
                    log.Info("Added Stream for File: " + fileName);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error loading Attachment: " + url.ToString());
            }

            this.totalNumberOfAttachmentsLoaded++;
            CheckAttachmentsProcessed();
        }

        private void CheckAttachmentsProcessed()
        {
            if (this.totalNumberOfAttachmentsToLoad == totalNumberOfAttachmentsLoaded)
            {
                log.Debug("Loaded All " + this.totalNumberOfAttachmentsToLoad + " Attachments");
                this.AttachmentsLoaded();
            }
        }

        private void AttachmentsLoaded()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.loadingUIAlertController.DismissViewController(false, null);
                this.DisplayLoginPage();
            });
        }

        private void DisplayLoginPage()
        {
            log.Debug("Instantiating Login Page...");
            this.loginPage = new LoginPageModal();
            this.loginPage.AuthorizationCompleted += LoginPage_AuthorizationCompleted;
            this.loginPage.LoginCancelled += LoginPage_LoginCancelled;

            log.Debug("Creating View Controller from Login Page...");
            this.loginPageUIViewController = loginPage.CreateViewController();

            this.loginPageUIViewController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            this.loginPageUIViewController.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;

            log.Debug("Presenting loginPage...");
            this.PresentViewController(this.loginPageUIViewController, false, null);

            log.Debug("Login Page Presented");
        }

        private void DisplayMainPage()
        {
            log.Debug("Instantiating Main Page...");
            this.mainPage = new MainPageModal();
            this.mainPage.Closed += MainPage_Closed;
            this.mainPage.LoggedOut += MainPage_LoggedOut;

            log.Debug("Creating View Controller from Send Page...");
            this.mainPageUIViewController = mainPage.CreateViewController();

            this.mainPageUIViewController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            this.mainPageUIViewController.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;

            log.Debug("Presenting mainPage...");
            this.PresentViewController(this.mainPageUIViewController, false, null);

            log.Debug("Main Page Presented");
        }

        private void DisplayErrorPage(string errorMessage, UIViewController returnToViewController)
        {
            this.NavigateFromParentPage(returnToViewController);

            log.Debug("Instantiating Error Page....");
            ErrorPageModal errorPage = new ErrorPageModal(errorMessage);
            errorPage.Closed += ErrorPage_Closed;

            log.Debug("Creating View Controller from Error Page...");
            this.errorPageUIViewController = errorPage.CreateViewController();

            this.errorPageUIViewController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            this.errorPageUIViewController.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;

            log.Debug("Presenting Error Page...");
            this.PresentViewController(this.errorPageUIViewController, false, null);

            log.Debug("Error Page Presented");
        }

        private void NavigateFromParentPage(UIViewController returnToViewController)
        {
            if (returnToViewController != null)
            {
                log.Debug("Dismissing Parent Page: " + returnToViewController.Class.Name);
                this.previousViewController = returnToViewController;
                returnToViewController.DismissViewController(false, null);
            }
        }

        private void NavigateBackToParentPage()
        {
            if (this.previousViewController != null)
            {
                log.Debug("Returning to page: " + this.previousViewController.Class.Name);
                this.PresentViewController(this.previousViewController, false, null);
                this.previousViewController = null;
            }
        }

        private void CloseExtension()
        {
            // Return any edited content to the host app.
            // This template doesn't do anything, so we just echo the passed-in items.
            log.Info("Closing Extension...");
            SessionController.DisposeSession();
            ExtensionContext.CompleteRequest(ExtensionContext.InputItems, null);
        }

        #endregion

        #region Event Handlers

        private void LoginPage_AuthorizationCompleted(bool success, string errorMessage)
        {
            if (success)
            {
                log.Debug("Login complete");
                this.loginPage.AuthorizationCompleted -= LoginPage_AuthorizationCompleted;

                log.Debug("Dismissing Login view....");
                this.loginPageUIViewController.DismissViewController(false, null);
                log.Debug("Display Send Page...");
                this.DisplayMainPage();
            }
            else
            {
                log.Error("Login failed with Status: " + errorMessage);

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    this.DisplayErrorPage(errorMessage, this.loginPageUIViewController);
                }
                this.loginPage.EnableInput(true);
            }
        }

        private void LoginPage_LoginCancelled()
        {
            this.loginPage.AuthorizationCompleted -= LoginPage_AuthorizationCompleted;
            this.loginPage.LoginCancelled -= LoginPage_LoginCancelled;

            this.CloseExtension();
        }

        private void MainPage_Closed()
        {
            log.Debug("Closing Main Page");
            this.mainPage.Closed -= MainPage_Closed;
            this.mainPage.LoggedOut -= MainPage_LoggedOut;
            this.mainPageUIViewController.DismissViewController(false, null);
            this.CloseExtension();
        }
        private void MainPage_LoggedOut()
        {
            log.Debug("Logging out from Main Page");
            this.mainPage.Closed -= MainPage_Closed;
            this.mainPage.LoggedOut -= MainPage_LoggedOut;

            SessionController.Instance.Logout();
            this.mainPageUIViewController.DismissViewController(false, null);
            this.DisplayLoginPage();
        }

        private void ErrorPage_Closed()
        {
            log.Debug("Dismissing Error Page view....");
            this.errorPageUIViewController.DismissViewController(false, null);

            if (this.previousViewController != null)
            {
                this.NavigateBackToParentPage();
            }
            else
            {
                this.CloseExtension();
            }
        }

        #endregion
    }
}
