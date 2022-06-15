#import <Foundation/Foundation.h>
#import <BUAdSDK/BUAdSDK.h>

@interface TTAdRewardVideoAdapter : NSObject

+(TTAdRewardVideoAdapter*) shareInstance;

-(id)init;

-(void)loadAdRewardVideo: (NSString *)slotID;

-(void)showAdRewardVideo;

#pragma mark - BUNativeExpressRewardedVideoAdDelegate
- (void)nativeExpressRewardedVideoAdDidLoad:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd;

-(void)nativeExpressRewardedVideoAd : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd didFailWithError : (NSError *_Nullable)error;

-(void)nativeExpressRewardedVideoAdCallback : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd withType : (BUNativeExpressRewardedVideoAdType)nativeExpressVideoType;

-(void)nativeExpressRewardedVideoAdDidDownLoadVideo : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd;

-(void)nativeExpressRewardedVideoAdViewRenderSuccess : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd;

-(void)nativeExpressRewardedVideoAdViewRenderFail : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd error : (NSError *_Nullable)error;

-(void)nativeExpressRewardedVideoAdWillVisible : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd;

-(void)nativeExpressRewardedVideoAdDidVisible : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd;

-(void)nativeExpressRewardedVideoAdWillClose : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd;

-(void)nativeExpressRewardedVideoAdDidClose : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd;

-(void)nativeExpressRewardedVideoAdDidClick : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd;

-(void)nativeExpressRewardedVideoAdDidClickSkip : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd;

-(void)nativeExpressRewardedVideoAdDidPlayFinish : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd didFailWithError : (NSError *_Nullable)error;

-(void)nativeExpressRewardedVideoAdServerRewardDidSucceed : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd verify : (BOOL)verify;

-(void)nativeExpressRewardedVideoAdServerRewardDidFail : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd error : (NSError * _Nullable)error;

-(void)nativeExpressRewardedVideoAdDidCloseOtherController : (BUNativeExpressRewardedVideoAd *)rewardedVideoAd interactionType : (BUInteractionType)interactionType;

#pragma mark - Log

- (void)pbud_logWithSEL:(SEL)sel msg : (NSString *)msg;
@end
