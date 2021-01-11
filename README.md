# complex-xamarin-forms-ios-extension
A complex example of a Xamarin iOS Application Action Extension that uses application groups, keychain access groups and NLog.

### What is this repository for? ###
The repo provide sample code for devlopers looking for a more complex example of a Xamarin iOS Application Action Extension that uses multiple pages, application groups (com.apple.security.application-groups) to sharing data, keychain access groups (keychain-access-groups) for securly storing passwords and NLog library for logging.

### How do I run this code? ###
1. Login to your Apple Developer account at: https://developer.apple.com/. 
- Create an "App Id" for your Main iOS Application (Container Application) and a seperate "App Id" for the iOS Extension for that Application. e.g. "com.companyname.ComplexXamarinformsIosExtApp.iOS" and  "com.companyname.ComplexXamarinformsiIosExtApp.iOS.ActionExtension" (These BundleIdentifiers will be need in the Info.plist files for both the Container Applciation and the Extension).
- Create an "App Group" your iOS Application and iOS Extension so that they can share data. It will be similar to: "group.com.companyname.ComplexXamarinformsiIosExtApp.iOS". This will be used in the Entitlements.plist.
2. Pull the branch
3. Edit the Info.plist files for both the Container Application and its Extension. Set the "Bundle Identifier" to the respective "App Id" you created in Step 1.
4. In the Project Properties for both the "ComplexXamarinformsiIosExtApp.iOS" and "ComplexXamarinformsiIosExtApp.iOS.ActionExtension" projects setup your Bundle Signing to use your development certificate
5. Edit the Entitlements.plist. Under App Groups add your Group identifier you created in Step 1.
6. You can run the App in the iPhone Simulator by selecting the "Debug" configuration and "iPhoneSimulator" platform.

### How does the code work? ###

#### Running the Application from the Icon: ####
When launched from the normal app icon, The ComplexXamarinFormsIosExtensionApp.iOS project launches the Xamarin Form application as normal from AppDelegate.FinishLaunching(). You can login (no actual web request is made) and your login details will be saved (if the option is ticked) via "IosSharedSettingsManager" class using the apple "NSUserDefaults" api and shared with the Action Extension via application-groups.

#### Running the Application from the Extension: ####
Browse to the Gallery, click the Share Icon, you should see this samples' custom icon (ComplexXamarinFormsIosExtensionApp.iOS.ActionExtension/Resources/Icon-60.png) at the bottom of the "Actions" list labeled "Sample Action Extension". This will call the ActionViewController class.

The ActionViewController constructor first initializes the Xamarin.Forms and IQKeyboardManager plugins. It then uses the DependencyService to register the iOS specific platform libraries for LogManager and the SharedSettingsManager. It then initializes the LoggerFactory to create a NLog instance using the configuration in: (ComplexXamarinFormsIosExtensionApp.iOS.ActionExtension/NLog.config)

Next ViewDidLoad() is called. This checks the NSExtensionContext for input items. If it finds items (i.e. the Gallery item you shared) GetAttachments() is called. If nothing is found the DisplayLoginPage() is called and a Modal version of the Login Page is presented. If there are attachments, GetAttachments() Displays a UIAlert to display to the user that it is "Preparing Attachments". Using PresentViewController() call LoadAttachments() is called.

In LoadAttachments() the attachments streams are Asynchronously loaded one-by-one in the LoadAttachment() moethod. Once all the Attachments are loaded the DisplayLoginPage() is called and a Modal version of the Login Page is presented.

The Login page has been written to display and Extra Cancel Button at the top of the page when run for the Action Extension. This is because Action Extensions are meant to have a short lifecycle. You either perform your task or exit. They are not designed to be suspended and resumed etc...

### Who do I talk to? ###

* Tim Hobbs
