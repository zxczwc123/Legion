#import <Foundation/Foundation.h>
#import <BUAdSDK/BUAdSDK.h>

@interface TTAdBannerAdapter : NSObject

+(TTAdBannerAdapter*) shareInstance;

-(id)init;

-(void)loadAdBanner: (NSString *)slotID;

-(void)showAdBanner;

#pragma BUNativeExpressBannerViewDelegate
-(void)nativeExpressBannerAdViewDidLoad:(BUNativeExpressBannerView *)bannerAdView;

-(void)nativeExpressBannerAdView : (BUNativeExpressBannerView *)bannerAdView didLoadFailWithError : (NSError *)error;

-(void)nativeExpressBannerAdViewRenderSuccess : (BUNativeExpressBannerView *)bannerAdView;

-(void)nativeExpressBannerAdViewRenderFail : (BUNativeExpressBannerView *)bannerAdView error : (NSError *)error;

-(void)nativeExpressBannerAdViewWillBecomVisible : (BUNativeExpressBannerView *)bannerAdView;

-(void)nativeExpressBannerAdViewDidClick : (BUNativeExpressBannerView *)bannerAdView;

-(void)nativeExpressBannerAdView : (BUNativeExpressBannerView *)bannerAdView dislikeWithReason : (NSArray<BUDislikeWords *> *)filterwords;

-(void)nativeExpressBannerAdViewDidCloseOtherController:(BUNativeExpressBannerView *)bannerAdView interactionType : (BUInteractionType)interactionType;

-(void)pbud_logWithSEL:(SEL)sel msg : (NSString *)msg;

@end
