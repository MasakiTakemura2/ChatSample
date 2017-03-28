//
//  PTConnector.h
//  partytrack
//
//  Created by Jamie on 3/3/13.
//  Copyright (c) 2013 adways. All rights reserved.
//

#import <Foundation/Foundation.h>

void start_( int app_id, const char* app_ley);
void disableApplicationTracking_();
void openDebugInfo_();
void setConfigure_(const char* name, const char* value);
void setCustomEventParameter_(const char* name, const char* value);
void sendEventWithId_( int event_id );
void sendEventWithName_( const char* event_name );
void sendPayment_( const char* item_name, int item_num, const char *item_price_currency, double item_price);
