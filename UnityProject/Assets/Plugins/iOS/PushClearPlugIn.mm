//
//  PushClearPlugIn.m
//  DownLoadManagerProject
//
//  Created by Noor Mohammad on 3/18/15.
//  Copyright (c) 2015 Tiam. All rights reserved.
//

#import "PushClearPlugIn.h"

@implementation PushClearPlugIn



extern "C" {
    
    void remotePushClear(){
    

        UIApplication *app = [UIApplication sharedApplication];
        
        if([app respondsToSelector:@selector(currentUserNotificationSettings)]){
            
                UIUserNotificationSettings *currentSettings = [[UIApplication sharedApplication] currentUserNotificationSettings];
            
            if(currentSettings.types & UIUserNotificationTypeBadge){

                app.applicationIconBadgeNumber = -1;
            }
        }
        else{
            
            app.applicationIconBadgeNumber = -1;
        }
        
    }
    
}

@end
