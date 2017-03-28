//
//  ImobileSdkAdsIosUnityPluginImpl.mm
//
//  Copyright 2014 i-mobile Co.Ltd. All rights reserved.
//

#import "ImobileSdkAdsIosUnityPluginImpl.h"

#ifdef __cplusplus
extern "C" {
#endif
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
#ifdef __cplusplus
}
#endif

@class ImobileSdkAdsIosUnityPluginImpl;

@interface ImobileNativeDataReciever : NSObject <IMobileSdkAdsDelegate>

@property (nonatomic, strong) NSArray *nativeAdObjectArray;
@property (atomic) int imageLoadedCounter;
@property (nonatomic) int adViewId;
@property (nonatomic, retain) NSString *gameObjectNameCreatedByImPlugin;

- (void)onNativeAdDataReciveCompleted:(NSString *)spotId nativeArray:(NSArray *)nativeArray;

@end

@implementation ImobileNativeDataReciever
BOOL getAdImageFlag;

- (void)dealloc {
#if !__has_feature(objc_arc)
    [self.gameObjectNameCreatedByImPlugin release];
    [super dealloc];
#endif
}

- (id)initWithAdViewId:(int)adViewId GameObjectName:(NSString *)gameObjectName getAdImageFlag:(BOOL)recievedGetAdImageFlag{
    self = [super init];
    if (self) {
        self.adViewId = adViewId;
        self.gameObjectNameCreatedByImPlugin = gameObjectName;
        getAdImageFlag = recievedGetAdImageFlag;
    }
    return self;
}

- (void)onNativeAdDataReciveCompleted:(NSString *)spotId nativeArray:(NSArray *)recievedNativeObjectArray{
    self.nativeAdObjectArray = recievedNativeObjectArray;

    if(getAdImageFlag){
        for (ImobileSdkAdsNativeObject *adObject in self.nativeAdObjectArray) {
            // image get
            [adObject getAdImageCompleteHandler:^(UIImage *loadimg) {
                self.imageLoadedCounter++;
                if(self.imageLoadedCounter >= [self.nativeAdObjectArray count]){
                    [self UnitySendMessageFromSDK : spotId];
                }
            }];
        }
    } else {
        [self UnitySendMessageFromSDK : spotId];
    }
    
}

-(void) UnitySendMessageFromSDK: (NSString *)spotId {
    NSString *sendMessage = [NSString stringWithFormat:@"%@", spotId];
    int cnt = 0;
    for (ImobileSdkAdsNativeObject *nativeAdObject in self.nativeAdObjectArray) {
        NSString* imageBinaryString = [[[NSData alloc] initWithData:UIImagePNGRepresentation([nativeAdObject getAdImage])] base64Encoding];
        NSArray *sendArray = [NSArray arrayWithObjects:
                                [NSString stringWithFormat:@"%d", cnt],
                                [nativeAdObject getAdTitle],
                                [nativeAdObject getAdDescription],
                                [nativeAdObject getAdSponsored],
                                [NSString stringWithFormat:@"%d",(int)[nativeAdObject getAdImage].size.width],
                                [NSString stringWithFormat:@"%d",(int)[nativeAdObject getAdImage].size.height],
                                imageBinaryString, nil];
        sendMessage = [NSString stringWithFormat:@"%@\\%@",sendMessage,[sendArray componentsJoinedByString:@":"]];
        cnt++;
    }
    
    // array to string
    UnitySendMessage([self.gameObjectNameCreatedByImPlugin UTF8String], [@"onNativeAdDataReciveCompleted" UTF8String], [sendMessage UTF8String]);
}

@end


@interface ImobileSdkAdsIosUnityPluginImpl ()
@end


@implementation ImobileSdkAdsIosUnityPluginImpl

// オブザーバ用のゲームオブジェクト名を保管するための変数（メディア側で作られたゲームオブジェクト名）
static const NSMutableSet *gameObjectNamesCreatedByUser = [NSMutableSet set];
// バナー広告などのUnityの上に乗せるViewのIDを管理するディクショナリ
static const NSMutableDictionary *adViewIdDictionary = [NSMutableDictionary dictionary];

static const NSMutableDictionary *nativeAdRecieverDictionary = [NSMutableDictionary dictionary];

extern UIViewController *UnityGetGLViewController();

// ----------------------------------------
#pragma mark - Call from inner C++
// ----------------------------------------

- (void)addObserver:(const char*)gameObjectName {
    [gameObjectNamesCreatedByUser addObject:[NSString stringWithUTF8String:gameObjectName]];
}

- (void)removeObserver:(const char*)gameObjectName {
    [gameObjectNamesCreatedByUser removeObject:[NSString stringWithUTF8String:gameObjectName]];
}

- (void)registerWithPublisherID:(const char*)publisherId MediaID:(const char*)mediaId SoptID:(const char*)soptId {
    [ImobileSdkAds registerWithPublisherID:[NSString stringWithUTF8String:publisherId]
                                   MediaID:[NSString stringWithUTF8String:mediaId]
                                    SpotID:[NSString stringWithUTF8String:soptId]];
    
    [ImobileSdkAds setSpotDelegate:[NSString stringWithUTF8String:soptId] delegate:self];
}

- (void)start {
    [ImobileSdkAds start];
}

- (void)stop {
    [ImobileSdkAds stop];
}

- (bool)startBySpotID:(const char*)spotId {
    return [ImobileSdkAds startBySpotID:[NSString stringWithUTF8String:spotId]];
}

- (bool)stopBySpotID:(const char*)spotId {
    return [ImobileSdkAds stopBySpotID:[NSString stringWithUTF8String:spotId]];
}

- (bool)showBySpotID:(const char*)spotId {
    return [ImobileSdkAds showBySpotID:[NSString stringWithUTF8String:spotId]];
}

- (bool)showWithParamStr:(const char*)paramStr {
    
    // parse paramStr
    NSArray *argument = [[NSString stringWithCString:paramStr encoding:NSUTF8StringEncoding] componentsSeparatedByString:@":"];
    NSString *publisherId = [argument objectAtIndex:0];
    NSString *mediaId = [argument objectAtIndex:1];
    NSString *spotId = [argument objectAtIndex:2];
    int left = [[argument objectAtIndex:3] intValue];
    int top = [[argument objectAtIndex:4] intValue];
    int width = [[argument objectAtIndex:5] intValue];
    int height = [[argument objectAtIndex:6] intValue];
    bool sizeAdjust = [[argument objectAtIndex:7] boolValue];
    int adViewId = [[argument objectAtIndex:8] intValue];
    
    // resister
    [self registerWithPublisherID:publisherId.UTF8String MediaID:mediaId.UTF8String SoptID:spotId.UTF8String];
    
    // start
    [self startBySpotID:spotId.UTF8String];
    
    // show
    UIView *adContainerView = [[UIView alloc] initWithFrame:CGRectMake(left, top, width, height)];
    [adViewIdDictionary setObject:adContainerView forKey:[NSString stringWithFormat:@"%d", adViewId]];
    [UnityGetGLViewController().view addSubview:adContainerView];
    
    return [ImobileSdkAds showBySpotID:spotId View:adContainerView SizeAdjust:sizeAdjust];
}

- (bool)getNativeAdParamsStr:(const char*)paramStr{
    
    // parse paramStr
    NSArray *argument = [[NSString stringWithCString:paramStr encoding:NSUTF8StringEncoding] componentsSeparatedByString:@":"];
    NSString *publisherId = [argument objectAtIndex:0];
    NSString *mediaId = [argument objectAtIndex:1];
    NSString *spotId = [argument objectAtIndex:2];
    int requestAdCount = [[argument objectAtIndex:3] intValue];
    BOOL imageGetFlag = [[argument objectAtIndex:4] boolValue];
    NSString *gameObjectName = [argument objectAtIndex:5];
    int adViewId = [[argument objectAtIndex:6] intValue];
    
    // adparams
    ImobileSdkAdsNativeParams *adParams = [[ImobileSdkAdsNativeParams alloc] init];
    adParams.requestAdCount = requestAdCount;
    adParams.nativeImageGetFlag = imageGetFlag;
    
    // resister
    [self registerWithPublisherID:publisherId.UTF8String MediaID:mediaId.UTF8String SoptID:spotId.UTF8String];
    
    // start
    [self startBySpotID:spotId.UTF8String];
    
    // reciever
    ImobileNativeDataReciever *nativeReciever = [[ImobileNativeDataReciever alloc] initWithAdViewId:adViewId GameObjectName:gameObjectName getAdImageFlag:adParams.nativeImageGetFlag];
    [nativeAdRecieverDictionary setObject:nativeReciever forKey:[NSNumber numberWithInt:adViewId]];
    
    UIView *adContainerView = [[UIView alloc] init];
    adContainerView.tag = adViewId;
    [UnityGetGLViewController().view addSubview:adContainerView];
    
    // get
    return [ImobileSdkAds getNativeAdData:spotId View:adContainerView Params:adParams Delegate:nativeReciever];
}

- (void) sendClickFromUnity:(int)adViewId nativeAdObjectIndex:(int)nativeAdObjectIndex {
    ImobileNativeDataReciever *nativeAdReciever = [nativeAdRecieverDictionary objectForKey:[NSNumber numberWithInt:adViewId]];
    [nativeAdReciever.nativeAdObjectArray[nativeAdObjectIndex] sendClick];
}

- (void) destroyNativeAd: (int)adViewId{
    ImobileNativeDataReciever *nativeDataReciever = [nativeAdRecieverDictionary objectForKey:[NSNumber numberWithInt:adViewId]];
    [nativeDataReciever.nativeAdObjectArray[0] destroy];
    nativeDataReciever.nativeAdObjectArray = nil;
    nativeDataReciever = nil;
    [nativeAdRecieverDictionary removeObjectForKey:[NSNumber numberWithInt:adViewId]];
    [[UnityGetGLViewController().view viewWithTag:adViewId] removeFromSuperview];
}

- (void)setAdOrientation:(ImobileSdkAdsAdOrientation)orientation {
    [ImobileSdkAds setAdOrientation:orientation];
}

- (void)setAdView:(int)adViewId visible:(bool)visible {
    UIView *adContainerView = [adViewIdDictionary objectForKey:[NSString stringWithFormat:@"%d", adViewId]];
    adContainerView.hidden = !visible;
}

- (void)setLegacyIosSdkMode:(bool)isLegacyMode {
    [ImobileSdkAds setLegacyIosSdkMode:isLegacyMode];
}

- (int)getScreenWidth:(bool)isPortrait {
    CGSize screenSize = UnityGetGLViewController().view.frame.size;
    if (isPortrait) {
        return MIN(screenSize.width, screenSize.height);
    } else {
        return MAX(screenSize.width, screenSize.height);
    }
}

- (int)getScreenHeight:(bool)isPortrait {
    CGSize screenSize = UnityGetGLViewController().view.frame.size;
    if (isPortrait) {
        return MAX(screenSize.width, screenSize.height);
    } else {
        return MIN(screenSize.width, screenSize.height);
    }
}

- (void)setTestMode:(bool)testMode {
    [ImobileSdkAds setTestMode:testMode];
}

// ----------------------------------------
#pragma mark Call from ImobileSdkAds
// ----------------------------------------

- (void)imobileSdkAdsSpot:(NSString *)spotId didReadyWithValue:(ImobileSdkAdsReadyResult)value {
    NSString *msg = [NSString stringWithFormat:@"%@", spotId];
    
    for (NSString *gameObjectName in gameObjectNamesCreatedByUser ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotDidReady" UTF8String],
                         [msg UTF8String]);
    }
}

- (void)imobileSdkAdsSpot:(NSString *)spotId didFailWithValue:(ImobileSdkAdsFailResult)value {
    NSString *msg = [NSString stringWithFormat:@"%@,%d", spotId,value];
    
    for (NSString *gameObjectName in gameObjectNamesCreatedByUser ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotDidFail" UTF8String],
                         [msg UTF8String]);
    }
}

- (void)imobileSdkAdsSpotIsNotReady:(NSString *)spotId {
    NSString *msg = [NSString stringWithFormat:@"%@", spotId];
    
    for (NSString *gameObjectName in gameObjectNamesCreatedByUser ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotIsNotReady" UTF8String],
                         [msg UTF8String]);
    }
}

- (void)imobileSdkAdsSpotDidClick:(NSString *)spotId {
    NSString *msg = [NSString stringWithFormat:@"%@", spotId];
    
    for (NSString *gameObjectName in gameObjectNamesCreatedByUser ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotDidClick" UTF8String],
                         [msg UTF8String]);
    }
}

- (void)imobileSdkAdsSpotDidClose:(NSString *)spotId {
    NSString *msg = [NSString stringWithFormat:@"%@", spotId];
    
    for (NSString *gameObjectName in gameObjectNamesCreatedByUser ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotDidClose" UTF8String],
                         [msg UTF8String]);
    }
}

- (void)imobileSdkAdsSpotDidShow:(NSString *)spotId {
    NSString *msg = [NSString stringWithFormat:@"%@", spotId];
    
    for (NSString *gameObjectName in gameObjectNamesCreatedByUser ) {
        UnitySendMessage([gameObjectName UTF8String],
                         [@"imobileSdkAdsSpotDidShow" UTF8String],
                         [msg UTF8String]);
    }
}
static ImobileSdkAdsIosUnityPluginImpl *unityPlugin = [[ImobileSdkAdsIosUnityPluginImpl alloc] init];

+ (ImobileSdkAdsIosUnityPluginImpl *) getPluginInstance {
    return unityPlugin;
}

// ----------------------------------------
#pragma mark - Call from Unity
// ----------------------------------------

#ifdef __cplusplus
extern "C" {
#endif
    
    
    void imobileAddObserver_(const char* gameObjectName) {
        [unityPlugin addObserver:gameObjectName];
    }
    
    void imobileRemoveObserver_(const char* gameObjectName) {
        [unityPlugin removeObserver:gameObjectName];
    }
    
    void imobileRegisterWithPublisherID_(const char* publisherId, const char* mediaId, const char* soptId) {
        [unityPlugin registerWithPublisherID:publisherId
                                     MediaID:mediaId
                                      SoptID:soptId];
    }
    
    void imobileStart_() {
        [unityPlugin start];
    }
    
    void imobileStop_() {
        [unityPlugin stop];
    }
    
    bool imobileStartBySpotID_(const char* spotId){
        return [unityPlugin startBySpotID:spotId];
    }
    
    bool imobileStopBySpotID_(const char* spotId) {
        return [unityPlugin stopBySpotID:spotId];
    }
    
    bool imobileShowBySpotID_(const char* spotId) {
        return [unityPlugin showBySpotID:spotId];
    }
    
    bool imobileShowBySpotIDWithPosition_(const char* paramStr){
        return [unityPlugin showWithParamStr:paramStr];
    }
    
    
    bool imobileGetNativeAdDataAndNativeAdParams_(const char* paramStr){
        return [unityPlugin getNativeAdParamsStr:paramStr];
        
    }
    
    void imobileSendClick_(int adViewId, int nativeAdObjectIndex){
        [unityPlugin sendClickFromUnity:adViewId nativeAdObjectIndex:nativeAdObjectIndex];
    }
    
    void imobileDestroyNativeAd_(int adViewId){
        [unityPlugin destroyNativeAd:adViewId];
    }
    
    void imobileSetAdOrientation_(ImobileSdkAdsAdOrientation orientation) {
        [unityPlugin setAdOrientation:orientation];
    }
    
    void imobileSetVisibility_(int adViewId, bool visible) {
        [unityPlugin setAdView:adViewId visible:visible];
    }
    
    void imobileSetLegacyIosSdkMode_(bool isLegacyMode) {
        [unityPlugin setLegacyIosSdkMode:isLegacyMode];
    }
    
    int getScreenWidth_(bool isPortrait) {
        return [unityPlugin getScreenWidth:isPortrait];
    }
    
    int getScreenHeight_(bool isPortrait) {
        return [unityPlugin getScreenHeight:isPortrait];
    }
    
    void setTestMode_(bool testMode) {
        return [unityPlugin setTestMode:testMode];
    }
    
#ifdef __cplusplus
}
#endif

@end