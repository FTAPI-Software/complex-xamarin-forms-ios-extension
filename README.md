# complex-xamarin-forms-ios-extension
A complex example of a Xamarin iOS Application Action Extension that uses application groups, keychain access groups and NLog.

### What is this repository for? ###
The repo provides sample code for devlopers looking for a more complex example of a Xamarin iOS Application Action Extension that uses multiple pages, application groups (com.apple.security.application-groups) to share data, keychain access groups (keychain-access-groups) for securely storing passwords and the NLog library for logging.

### How do I run this code? ###
1. Login to your Apple Developer account at: https://developer.apple.com/. 
Create an "App Group" for your iOS Application and iOS Extension so that they can share data. It will be similar to: "group.com.[your company name].ComplexXamarinFormsIosExtApp.iOS".
- Create an "App Id" for your Main iOS Application (Container Application) and a seperate "App Id" for the iOS Extension for that Application e.g. for your Bundle Id's use from the Main iOS Application use something like "com.[your company name].ComplexXamarinFormsIosExtApp.iOS" and for your Extension application something like "com.[your company name].ComplexXamarinFormsIosExtApp.iOS.ActionExtension". 
Ensure that you tick the Group capability for both these new "App Id"s and after registering these "App Ids" edit them to used the Group you created above.
2. Pull the branch
3. Edit the Info.plist files for both the Container Application and its Extension. Set the "Bundle Identifier" to the respective "App Id" you created in Step 1.
4. Edit the Entitlements.plist files in both iOS projects, Set the com.apple.security.application-groups value to your App Group name you created in step 1. Then add your Team ID and company name to the keychain-access-groups value field.
5. Edit the IosLogManager.cs and change the GROUP_ID to your App Group name you created in step 1. e.g. "group.com.[your company name].ComplexXamarinFormsIosExtApp.iOS"
6. Edit the IosSharedSettingsManager.cs and Add your Team ID and company name to the ACCESS_GROUP identifier. Set the GROUP_ID to your App Group name you created in step 1.
7. In the Project Properties for both the "ComplexXamarinFormsIosExtApp.iOS" and "ComplexXamarinFormsIosExtApp.iOS.ActionExtension" projects setup your Bundle Signing for your development configs to use your development certificate. Using the Automatic Provisioning option will really help you run the development builds here.
8. You can now run the App in the iPhone Simulator by selecting the "Debug" configuration and "iPhoneSimulator" platform.

#### iPhoneSimulator combined logging limitations ####
Please note that the iPhoneSimulator (at time of writting) is not able to write the log to the same shared file location, so the Action Extension application will log to its default file location: "${specialfolder:folder=MyDocuments}/../Library/Logs/ComplexXamarinFormsIosExtAppActionExtension.log"
When debugging on an actual devices this does not seem to be an issue and only one log file is written to.

### How does the code work? ###

#### Running the Application from the Icon: ####
When launched from the normal app icon, The ComplexXamarinFormsIosExtensionApp.iOS project launches the Xamarin Form application as normal from AppDelegate.FinishLaunching(). You can login (no actual web request is made just a 5 second thread is run) and your login details will be saved (if the option is ticked) via "IosSharedSettingsManager" class using the apple "NSUserDefaults" api and shared with the Action Extension via  application groups (com.apple.security.application-groups).

#### Running the Application from the Extension: ####
Browse to the Gallery, click the Share Icon, you should see this samples' custom icon (ComplexXamarinFormsIosExtensionApp.iOS.ActionExtension/Resources/Icon-60.png) at the bottom of the "Actions" list labeled "Sample Action Extension". This will call the ActionViewController class.

The ActionViewController constructor first initializes the Xamarin.Forms and IQKeyboardManager plugins. It then uses the DependencyService to register the iOS specific platform libraries for LogManager and the SharedSettingsManager. It then initializes the LoggerFactory to create a NLog instance using the configuration in: "ComplexXamarinFormsIosExtensionApp.iOS.ActionExtension/NLog.config" as default.

Next ActionViewController.ViewDidLoad() is called. This checks the NSExtensionContext for input items. If it finds items (i.e. the Gallery item you shared) GetAttachments() is called.
- If nothing is found the ActionViewController.DisplayLoginPage() is called and a Modal version of the Login Page is presented. 
- If there are attachments, ActionViewController.GetAttachments() Displays a UIAlert to display to the user that it is "Preparing Attachments". Using ActionViewController.PresentViewController() a call ActionViewController.LoadAttachments() is made.

In ActionViewController.LoadAttachments() the attachments streams are Asynchronously loaded one-by-one in the ActionViewController.LoadAttachment() method. Once all the Attachments are loaded the ActionViewController.DisplayLoginPage() is called and a Modal version of the Login Page is presented.

The Login page has been written to display an additonal "Cancel" Button at the top of the page when run for the Action Extension. This is because Action Extensions are meant to have a short lifecycle. You either perform your task or exit. They are not designed to be suspended and resumed etc...

### Who do I talk to? ###

* Tim Hobbs
