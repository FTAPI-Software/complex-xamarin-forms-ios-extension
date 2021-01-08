# complex-xamarin-forms-ios-extension
A complex example of a Xamarin iOS Application Action Extension that uses application groups, keychain access groups and NLog.

### What is this repository for? ###
The repo provide sample code for devleopers looking for a more complex example of a Xamarin iOS Application Action Extension that uses application groups (com.apple.security.application-groups), keychain access groups (keychain-access-groups) and NLog.

### How do I run this code? ###
1. Pull the branch
2. You can run the App and its Action Extension with the Simulator by selecting the "Debug" configuration

### How does the code work? ###

#### Running the Application from the Icon: ####
When launched from the normal app icon, The ComplexXamarinFormsIosExtensionApp.iOS project runs lauchces the Xamarin Form application as normal from AppDelegate.FinishLaunching(). You can login (no actual web request is made) your login details will be saved (is ticked) into the settings and shared with the Action Extension via application-groups.

#### Running the Application from the Extension: ####
Browse to the Gallery, click the Share Icon, you should see this samples' custom icon (ComplexXamarinFormsIosExtensionApp.iOS.ActionExtension/Resources/Icon-60.png) at the bottom of the "Actions" list labeled "Sample Action Extension". This will call the ActionViewController.

The ActionViewController constructor first initializes the Xamarin.Forms and IQKeyboardManager plugins. It then uses the DependencyService to register the Ios platform libraries for LogManager and the SharedSettingsManager. It then initializes the LoggerFactory to create a NLog instance using the configuration in: (ComplexXamarinFormsIosExtensionApp.iOS.ActionExtension/NLog.config)

Next ViewDidLoad() is called. This checks the NSExtensionContext for input items. If it finds items (i.e. the Gallery item you shared) GetAttachments() is called. If nothing is found the DisplayLoginPage() is called and a Modal version of the Login Page is presented. If there are attachments, GetAttachments() Displays a UIAlert to display to the user that it is "Preparing Attachments". Using PresentViewController() call LoadAttachments() is called.

In LoadAttachments() the attachments streams are Asynchronously loaded one-by-one in the LoadAttachment() moethod. Once all the Attachments are loaded the DisplayLoginPage() is called and a Modal version of the Login Page is presented.

The Login page will display and Extra Cancel Button at the top of the page. This is because Action Extensions are meant to have a short lifecycle. You either perform your task or exit. They are not designed to be susspended etc...

### Who do I talk to? ###

* Tim Hobbs
