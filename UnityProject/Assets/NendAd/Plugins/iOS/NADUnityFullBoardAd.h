//
//  NADUnityFullBoardAd.h
//  Unity-iPhone
//

#import <NendAd/NADFullBoardLoader.h>

typedef const void *NendIOSFullBoardAdPtr;
typedef const void *NendUnityFullBoardAdPtr;
typedef void (*NendUnityFullBoardAdCallback)(NendUnityFullBoardAdPtr unityPtr, NSInteger result);

extern "C" {
    NendIOSFullBoardAdPtr _CreateFullBoardAd(const char* spotId, const char* apiKey);
    void _LoadFullBoardAd(NendUnityFullBoardAdPtr unityPtr, NendIOSFullBoardAdPtr iOSPtr, NendUnityFullBoardAdCallback callback);
    void _ShowFullBoardAd(NendUnityFullBoardAdPtr unityPtr, NendIOSFullBoardAdPtr iOSPtr);
    void _ReleaseFullBoardAd(NendIOSFullBoardAdPtr iOSPtr);
}
