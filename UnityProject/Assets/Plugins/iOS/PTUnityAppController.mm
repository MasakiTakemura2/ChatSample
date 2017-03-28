//
//  AppController+PTOpenURL.mm
//  Unity-iPhone
//
//  Created by キョ　暁毅 on 2014/10/01.
//
//

#import "UnityAppController.h"
#import "Partytrack.h"

@interface PTUnityAppController : UnityAppController

@end


@implementation PTUnityAppController

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions{
    if (launchOptions != nil && [launchOptions objectForKey:UIApplicationLaunchOptionsURLKey] != nil) {
        [[NSUserDefaults standardUserDefaults] setObject:[[launchOptions objectForKey:UIApplicationLaunchOptionsURLKey] absoluteString] forKey:@"pt_url_scheme"];
        [[NSUserDefaults standardUserDefaults] synchronize];
    }
    
    UIViewController *rootViewController = UnityGetGLViewController();
    BOOL result = [super application:application didFinishLaunchingWithOptions:launchOptions];
    [[Partytrack sharedInstance] setDisplayWindow: self.window];
    return result;
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(PTUnityAppController);