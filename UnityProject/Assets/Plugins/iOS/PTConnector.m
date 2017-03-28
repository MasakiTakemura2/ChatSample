//
//  PTConnector.m
//  partytrack
//
//  Created by Jamie on 3/3/13.
//  Copyright (c) 2013 adways. All rights reserved.
//

#import "PTConnector.h"
#import "Partytrack.h"

void start_( int app_id, const char* app_key){
    NSDictionary *launchOptions = nil;

    NSString *pt_url_scheme = [[NSUserDefaults standardUserDefaults] objectForKey:@"pt_url_scheme"];
    if (pt_url_scheme != nil) {
        launchOptions = [NSDictionary dictionaryWithObjectsAndKeys:[NSURL URLWithString:pt_url_scheme],UIApplicationLaunchOptionsURLKey,nil];
        [[NSUserDefaults standardUserDefaults] removeObjectForKey:@"pt_url_scheme"];
        [[NSUserDefaults standardUserDefaults] synchronize];
    }
    
    [[Partytrack sharedInstance] startWithAppID:app_id AndKey:[NSString stringWithCString:app_key encoding:NSUTF8StringEncoding] AndOptions:launchOptions];
}

void openDebugInfo_(){
    [Partytrack openDebugInfo];
}

void setConfigure_(const char* name, const char* value){
    [[Partytrack sharedInstance] setConfigureWithName:[NSString stringWithCString:name encoding:NSUTF8StringEncoding] AndValue:[NSString stringWithCString:value encoding:NSUTF8StringEncoding]];
}

void setCustomEventParameter_(const char* name, const char* value){
    [[Partytrack sharedInstance] setCustomEventParameterWithName:[NSString stringWithCString:name encoding:NSUTF8StringEncoding] AndValue:[NSString stringWithCString:value encoding:NSUTF8StringEncoding]];
}

void sendEventWithId_( int event_id ){
    [[Partytrack sharedInstance] sendEventWithID:event_id];
}

void sendEventWithName_( const char* event_name ){
    [[Partytrack sharedInstance] sendEventWithName:[NSString stringWithCString:event_name encoding:NSUTF8StringEncoding]];
}

void sendPayment_( const char* item_name, int item_num, const char *item_price_currency, double item_price){
    NSDictionary *data = [[NSDictionary alloc] initWithObjectsAndKeys:[NSNumber numberWithDouble:item_price],@"item_price",[NSString stringWithCString:item_price_currency encoding:NSUTF8StringEncoding] , @"item_price_currency", [NSNumber numberWithInt:item_num], @"item_num", [NSString stringWithCString:item_name encoding:NSUTF8StringEncoding], @"item_name", nil];
    [[Partytrack sharedInstance] sendPaymentWithParameters: data];
}

void disableApplicationTracking_(){
    [[Partytrack sharedInstance] disableApplicationTracking];
}
