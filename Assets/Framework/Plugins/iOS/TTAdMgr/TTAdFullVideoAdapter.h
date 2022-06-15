#import <Foundation/Foundation.h>
#import <BUAdSDK/BUAdSDK.h>

@interface TTAdFullVideoAdapter : NSObject

+ (TTAdFullVideoAdapter*)shareInstance;

-(id)init;

-(void)loadAdFullVideo:(NSString *)slotID;

-(void)showAdFullVideo;

#pragma mark - BUNativeExpressFullscreenVideoAdDelegate
- (void)nativeExpressFullscreenVideoAdDidLoad:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd;

-(void)nativeExpressFullscreenVideoAd : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd didFailWithError : (NSError *_Nullable)error;

-(void)nativeExpressFullscreenVideoAdViewRenderSuccess : (BUNativeExpressFullscreenVideoAd *)rewardedVideoAd;

-(void)nativeExpressFullscreenVideoAdViewRenderFail : (BUNativeExpressFullscreenVideoAd *)rewardedVideoAd error : (NSError *_Nullable)error;

-(void)nativeExpressFullscreenVideoAdDidDownLoadVideo : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd;

-(void)nativeExpressFullscreenVideoAdWillVisible : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd;

-(void)nativeExpressFullscreenVideoAdDidVisible : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd;

-(void)nativeExpressFullscreenVideoAdDidClick : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd;

-(void)nativeExpressFullscreenVideoAdDidClickSkip : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd;

-(void)nativeExpressFullscreenVideoAdWillClose : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd;

-(void)nativeExpressFullscreenVideoAdDidClose : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd;

-(void)nativeExpressFullscreenVideoAdDidPlayFinish : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd didFailWithError : (NSError *_Nullable)error;

-(void)nativeExpressFullscreenVideoAdCallback : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd withType : (BUNativeExpressFullScreenAdType)nativeExpressVideoAdType;

-(void)nativeExpressFullscreenVideoAdDidCloseOtherController : (BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd interactionType : (BUInteractionType)interactionType;

#pragma mark - Log
- (void)pbud_logWithSEL:(SEL)sel msg : (NSString *)msg;

@end
