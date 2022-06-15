#import "TTAdInterstitialAdapter.h"
#import <BUAdSDK/BUAdSDK.h>

static dispatch_once_t predicate;
static TTAdInterstitialAdapter* _instance = nil;


extern "C"
{
    
    typedef void (*InterstitialDidLoadCallback) ();
    InterstitialDidLoadCallback interstitialDidLoadCallback;
    void setInterstitialDidLoadCallback(InterstitialDidLoadCallback callback)
    {
        interstitialDidLoadCallback = callback;
    }
    void onInterstitialDidLoadCallback(){
        interstitialDidLoadCallback();
    }

    typedef void (*InterstitialDidLoadFailCallback) (unsigned int code,const char* msg);
    InterstitialDidLoadFailCallback interstitialDidLoadFailCallback;
    void setInterstitialDidLoadFailCallback(InterstitialDidLoadFailCallback callback)
    {
        interstitialDidLoadFailCallback = callback;
    }
    void onInterstitialDidLoadFailCallback(unsigned int code,const char* msg){
        interstitialDidLoadFailCallback(code, msg);
    }

    typedef void (*InterstitialRenderSuccessCallback) ();
    InterstitialRenderSuccessCallback interstitialRenderSuccessCallback;
    void setInterstitialRenderSuccessCallback(InterstitialRenderSuccessCallback callback)
    {
        interstitialRenderSuccessCallback = callback;
    }
    void onInterstitialRenderSuccessCallback(){
        interstitialRenderSuccessCallback();
    }

    typedef void (*InterstitialRenderFailCallback) (unsigned int code,const char* msg);
    InterstitialRenderFailCallback interstitialRenderFailCallback;
    void setInterstitialRenderFailCallback(InterstitialRenderFailCallback callback)
    {
        interstitialRenderFailCallback = callback;
    }
    void onInterstitialRenderFailCallback(unsigned int code,const char* msg){
        interstitialRenderFailCallback(code, msg);
    }

    typedef void (*InterstitialWillVisibleCallback) ();
    InterstitialWillVisibleCallback interstitialWillVisibleCallback;
    void setInterstitialWillVisibleCallback(InterstitialWillVisibleCallback callback)
    {
        interstitialWillVisibleCallback = callback;
    }
    void onInterstitialWillVisibleCallback(){
        interstitialWillVisibleCallback();
    }

    typedef void (*InterstitialDidCloseCallback) ();
    InterstitialDidCloseCallback interstitialDidCloseCallback;
    void setInterstitialDidCloseCallback(InterstitialDidCloseCallback callback)
    {
        interstitialDidCloseCallback = callback;
    }
    void onInterstitialDidCloseCallback(){
        interstitialDidCloseCallback();
    }

    typedef void (*InterstitialDidClickCallback) ();
    InterstitialDidClickCallback interstitialDidClickCallback;
    void setInterstitialDidClickCallback(InterstitialDidClickCallback callback)
    {
        interstitialDidClickCallback = callback;
    }
    void onInterstitialDidClickCallback(){
        interstitialDidClickCallback();
    }
}


@interface TTAdInterstitialAdapter ()<BUNativeExpresInterstitialAdDelegate>
@property (nonatomic, strong) BUNativeExpressInterstitialAd *interstitialAd;
@end

@implementation TTAdInterstitialAdapter

+(TTAdInterstitialAdapter*) shareInstance
{
    dispatch_once(&predicate, ^{
        _instance =  [[TTAdInterstitialAdapter alloc] init];
    });
    return _instance;
}

-(id)init
{
    self = [super init];
    return self;
}

-(void) loadAdInterstitialVideo:(NSString *) slotID {
    CGSize size = CGSizeMake(600, 600);
    CGFloat width = CGRectGetWidth([UIScreen mainScreen].bounds)-40;
    CGFloat height = width/size.width*size.height;
    self.interstitialAd = [[BUNativeExpressInterstitialAd alloc] initWithSlotID:slotID adSize:CGSizeMake(width, height)];
    self.interstitialAd.delegate = self;
    [self.interstitialAd loadAdData];
    
}

-(void) showAdInterstitialVideo {
	if (self.interstitialAd) {
        [self.interstitialAd showAdFromRootViewController:UnityGetGLViewController()];
    }
}


#pragma ---BUNativeExpresInterstitialAdDelegate
- (void)nativeExpresInterstitialAdDidLoad:(BUNativeExpressInterstitialAd *)interstitialAd {
    [self pbud_logWithSEL:_cmd msg:@""];
	onInterstitialDidLoadCallback();
}

- (void)nativeExpresInterstitialAd:(BUNativeExpressInterstitialAd *)interstitialAd didFailWithError:(NSError *)error {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"error:%@", error]];
    onInterstitialDidLoadFailCallback(error.code,[[NSString stringWithFormat:@"error:%@", error] UTF8String]);
}

- (void)nativeExpresInterstitialAdRenderSuccess:(BUNativeExpressInterstitialAd *)interstitialAd {
    [self pbud_logWithSEL:_cmd msg:@""];
	onInterstitialRenderSuccessCallback();
}

- (void)nativeExpresInterstitialAdRenderFail:(BUNativeExpressInterstitialAd *)interstitialAd error:(NSError *)error {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"error:%@", error]];
	onInterstitialRenderFailCallback(error.code,[[NSString stringWithFormat:@"error:%@", error] UTF8String]);
}

- (void)nativeExpresInterstitialAdWillVisible:(BUNativeExpressInterstitialAd *)interstitialAd {
    [self pbud_logWithSEL:_cmd msg:@""];
	onInterstitialWillVisibleCallback();
}

- (void)nativeExpresInterstitialAdDidClick:(BUNativeExpressInterstitialAd *)interstitialAd {
    [self pbud_logWithSEL:_cmd msg:@""];
	onInterstitialDidClickCallback();
}

- (void)nativeExpresInterstitialAdWillClose:(BUNativeExpressInterstitialAd *)interstitialAd {
    [self pbud_logWithSEL:_cmd msg:@""];
}

- (void)nativeExpresInterstitialAdDidClose:(BUNativeExpressInterstitialAd *)interstitialAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    self.interstitialAd = nil;
	onInterstitialDidCloseCallback();
}

- (void)nativeExpresInterstitialAdDidCloseOtherController:(BUNativeExpressInterstitialAd *)interstitialAd interactionType:(BUInteractionType)interactionType {
    NSString *str;
    if (interactionType == BUInteractionTypePage) {
        str = @"ladingpage";
    } else if (interactionType == BUInteractionTypeVideoAdDetail) {
        str = @"videoDetail";
    } else {
        str = @"appstoreInApp";
    }
    [self pbud_logWithSEL:_cmd msg:str];
}
- (void)pbud_logWithSEL:(SEL)sel msg:(NSString *)msg {
    NSLog(@"SDKDemoDelegate BUNativeExpressInterstitialAd In VC (%@) extraMsg:%@", NSStringFromSelector(sel), msg);
}

@end


extern "C"
{
	void loadAdInterstitialVideo(const char* codeId) {
		NSString *text_codeId = [NSString stringWithUTF8String: codeId];
		[[TTAdInterstitialAdapter shareInstance] loadAdInterstitialVideo: text_codeId];
	}

	void showAdInterstitialVideo() {
		[[TTAdInterstitialAdapter shareInstance] showAdInterstitialVideo];
	}

}
