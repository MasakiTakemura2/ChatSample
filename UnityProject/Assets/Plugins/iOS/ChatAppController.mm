//
//  ChatAppController.m
//  Unity-iPhone
//
//  Created by Noor Mohammad on 3/3/16.
//
//
#import "ChatAppController.h"
#import <CoreLocation/CoreLocation.h>
@implementation ChatAppController
//アプリのプッシュポップアップが押された時の起動時処理。
- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    // Override point for customization after application launch.

    pushMessage_ = nil;
    locationStatus_ = [NSString stringWithFormat:@"0"];

    [self locationServiceCheck];

    NSDictionary *pushInfo = [launchOptions objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey];
    if(pushInfo!=nil){

        if([pushInfo objectForKey:@"NativeInfo"]!=nil){

            NSMutableString *unityMessage = [[NSMutableString alloc] init];

            if([[pushInfo objectForKey:@"NativeInfo"]objectForKey:@"view_name"]!=nil){
                NSString *viewName = (NSString*)[[pushInfo objectForKey:@"NativeInfo"]objectForKey:@"view_name"];
                [unityMessage appendString:viewName];
                [unityMessage appendString:@" "];
            }

            if([[pushInfo objectForKey:@"NativeInfo"]objectForKey:@"id"]!=nil){
                NSString *viewName = (NSString*)[[pushInfo objectForKey:@"NativeInfo"]objectForKey:@"id"];
                [unityMessage appendString:viewName];
            }
            pushMessage_ = [NSString stringWithFormat:@"%@",unityMessage];
        }

    }
NSLog(@"[pushMessage_ notify pop touch] %@", pushMessage_);
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

//リモートプッシュをキャッチ
- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler
{
    // Override point for customization after application launch.

    pushMessage_ = nil;

    [self locationServiceCheck];

    if(userInfo　!=　nil){
        //アプリがフォアグラウンドで動いている状態だった
        if (application.applicationState == UIApplicationStateActive)
        {
            if([userInfo objectForKey:@"aps"]!=nil)
            {
                NSMutableString *unityMessage = [[NSMutableString alloc] init];
                NSString *alert    = nil;
                NSString *aroundId = nil;
                NSString *viewName = nil;
                NSString *postMsg = nil;
                if([[userInfo objectForKey:@"aps"]objectForKey:@"alert"]!=nil)
                {
                    alert = (NSString*)[[userInfo objectForKey:@"aps"]objectForKey:@"alert"];
                }

                if([[userInfo objectForKey:@"NativeInfo"]objectForKey:@"view_name"]!=nil){
                    aroundId = (NSString*)[[userInfo objectForKey:@"NativeInfo"]objectForKey:@"view_name"];
                }

                if([[userInfo objectForKey:@"NativeInfo"]objectForKey:@"id"]!=nil){
                    viewName = (NSString*)[[userInfo objectForKey:@"NativeInfo"]objectForKey:@"id"];
                }

                if (viewName != nil && aroundId != nil) {
                    postMsg = [NSString stringWithFormat:@"%@-%@-%@",alert, aroundId, viewName];
                } else {
                    postMsg = alert;
                }

                [unityMessage appendString:postMsg];
                [unityMessage appendString:@" "];

                pushMessage_ = [NSString stringWithFormat:@"%@",unityMessage];
                UnitySendMessage("NotifyMessageCatch", "Message", [pushMessage_ UTF8String]);
            }
        }
        //バックグラウンドにいる状態で、通知を受け取り、通知領域をタップしてフォアグラウンドになった
        else if (application.applicationState == UIApplicationStateInactive)
        {

            if([userInfo objectForKey:@"NativeInfo"]!=nil){

                NSMutableString *unityMessage = [[NSMutableString alloc] init];

                if([[userInfo objectForKey:@"NativeInfo"]objectForKey:@"view_name"]!=nil){
                    NSString *viewName = (NSString*)[[userInfo objectForKey:@"NativeInfo"]objectForKey:@"view_name"];
                    [unityMessage appendString:viewName];
                    [unityMessage appendString:@" "];
                }

                if([[userInfo objectForKey:@"NativeInfo"]objectForKey:@"id"]!=nil){
                    NSString *viewName = (NSString*)[[userInfo objectForKey:@"NativeInfo"]objectForKey:@"id"];
                    [unityMessage appendString:viewName];
                }
                pushMessage_ = [NSString stringWithFormat:@"%@",unityMessage];
            }
        }
    }

    NSLog(@"[pushMessage_ notify touch] %@", pushMessage_);
    [super application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:handler];
}
#pragma mark - Location Service Check
-(void)locationServiceCheck{
    if([CLLocationManager locationServicesEnabled]){

        switch ([CLLocationManager authorizationStatus]) {
            case kCLAuthorizationStatusAuthorizedAlways:
            case kCLAuthorizationStatusAuthorizedWhenInUse:
                //取得して良い場合。
                locationStatus_ = [NSString stringWithFormat:@"1"];
                　　
                break;

            case kCLAuthorizationStatusDenied:
            case kCLAuthorizationStatusRestricted:
                //ダメな場合。
                locationStatus_ = [NSString stringWithFormat:@"2"];
                break;
            default :
                break;
        }
    }
    else{
        locationStatus_ = [NSString stringWithFormat:@"5"];
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"" message:@"位置情報の設定を変更してください。" delegate:self cancelButtonTitle:@"キャンセル" otherButtonTitles:@"設定", nil];

        [alertView show];
    }

}
#pragma mark - AlertView delegate
- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex{


    if(buttonIndex==1){

        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
    }

}
-(NSString*)getPushMessage{

    return pushMessage_;

}
-(NSString*)getLocationStatus{

    return locationStatus_;

}
@end
