#import <Foundation/Foundation.h>
#import <BUAdSDK/BUAdSDK.h>

@interface TTAdInterstitialAdapter : NSObject

+(TTAdInterstitialAdapter*) shareInstance;

-(id)init;

-(void)loadAdInterstitialVideo:(NSString *)slotID;

-(void)showAdInterstitialVideo;


#pragma ---BUNativeExpresInterstitialAdDelegate
- (void)nativeExpresInterstitialAdDidLoad:(BUNativeExpressInterstitialAd *)interstitialAd;

-(void)nativeExpresInterstitialAd : (BUNativeExpressInterstitialAd *)interstitialAd didFailWithError : (NSError *)error;

-(void)nativeExpresInterstitialAdRenderSuccess : (BUNativeExpressInterstitialAd *)interstitialAd;

-(void)nativeExpresInterstitialAdRenderFail : (BUNativeExpressInterstitialAd *)interstitialAd error : (NSError *)error;

-(void)nativeExpresInterstitialAdWillVisible : (BUNativeExpressInterstitialAd *)interstitialAd;

-(void)nativeExpresInterstitialAdDidClick : (BUNativeExpressInterstitialAd *)interstitialAd;

-(void)nativeExpresInterstitialAdWillClose : (BUNativeExpressInterstitialAd *)interstitialAd;

-(void)nativeExpresInterstitialAdDidClose : (BUNativeExpressInterstitialAd *)interstitialAd;

-(void)nativeExpresInterstitialAdDidCloseOtherController : (BUNativeExpressInterstitialAd *)interstitialAd interactionType : (BUInteractionType)interactionType;

-(void)pbud_logWithSEL:(SEL)sel msg : (NSString *)msg;

@end
