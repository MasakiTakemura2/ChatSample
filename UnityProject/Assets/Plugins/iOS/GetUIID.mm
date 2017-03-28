#import <Foundation/Foundation.h>
#import "UIApplication+UIID.h"
#import "LUKeychainAccess.h"
char szDst[256];



char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;

    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

extern "C"{
    char* getUiid() {
        NSString *uiid = [[UIApplication sharedApplication] uniqueInstallationIdentifier];
        //NSLog(@"[uiid]%@", uiid);
        return MakeStringCopy([uiid UTF8String]);
    }
}
extern "C"{
    char* getUdid() {
        NSString *uiid = [[UIApplication sharedApplication] uniqueDeviceIdentifier];
        //NSLog(@"[udid]%@", uiid);
        return MakeStringCopy([uiid UTF8String]);
    }
}
extern "C"{
    char* getIdfv() {
        NSUUID *vendorUUID = [UIDevice currentDevice].identifierForVendor;
        NSString *idfv = vendorUUID.UUIDString;
        NSLog(@"[idfv]%@", idfv);
        return MakeStringCopy([idfv UTF8String]);
    }
}
extern "C"{
    char* getBuildVersion()
    {
        NSString *buildVersion = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleShortVersionString"];

        return MakeStringCopy([buildVersion UTF8String]);
    }
}


extern "C" {
    const char* getkeyChain_();
    void setkeyChain_(const char* info);
}

const char* getkeyChain_()
{
    NSString *info = [[LUKeychainAccess standardKeychainAccess] stringForKey:@"htpartner_inc"];

    if (info == nil){
        info = [NSString stringWithFormat:@""];
    }
    NSLog(@"Rarararara %@",info);

    return MakeStringCopy([info UTF8String]);
}

void setkeyChain_ (const char* info)
{
    NSString *nsstr = [NSString stringWithUTF8String: info];

    NSLog(@"#saveToKeyChain %@",nsstr);

    [[LUKeychainAccess standardKeychainAccess] setString:nsstr forKey:@"htpartner_inc"];
}
