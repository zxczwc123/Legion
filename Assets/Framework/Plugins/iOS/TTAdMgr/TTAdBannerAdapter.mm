#import "TTAdBannerAdapter.h"
#import <BUAdSDK/BUAdSDK.h>

static dispatch_once_t predicate;
static TTAdBannerAdapter* _instance = nil;


extern "C"
{
    typedef void (*BannerAdViewDidLoadCallback) ();
    BannerAdViewDidLoadCallback bannerAdViewDidLoadCallback;
    void setBannerAdViewDidLoadCallback(BannerAdViewDidLoadCallback callback)
    {
        bannerAdViewDidLoadCallback = callback;
    }
    void onBannerAdViewDidLoadCallback(){
        bannerAdViewDidLoadCallback();
    }

    typedef void (*BannerAdViewDidLoadFailCallback) (unsigned int  code,const char* msg);
    BannerAdViewDidLoadFailCallback bannerAdViewDidLoadFailCallback;
    void setBannerAdViewDidLoadFailCallback(BannerAdViewDidLoadFailCallback callback)
    {
        bannerAdViewDidLoadFailCallback = callback;
    }
    void onBannerAdViewDidLoadFailCallback(unsigned int code,const char* msg){
        bannerAdViewDidLoadFailCallback(code, msg);
    }
    
    typedef void (*BannerAdViewRenderSuccessCallback) ();
    BannerAdViewRenderSuccessCallback bannerAdViewRenderSuccessCallback;
    void setBannerAdViewRenderSuccessCallback(BannerAdViewRenderSuccessCallback callback)
    {
        bannerAdViewRenderSuccessCallback = callback;
    }
    void onBannerAdViewRenderSuccessCallback(){
        bannerAdViewRenderSuccessCallback();
    }

    typedef void (*BannerAdViewRenderFailCallback) (unsigned int  code,const char* msg);
    BannerAdViewRenderFailCallback bannerAdViewRenderFailCallback;
    void setBannerAdViewRenderFailCallback(BannerAdViewRenderFailCallback callback)
    {
        bannerAdViewRenderFailCallback = callback;
    }
    void onBannerAdViewRenderFailCallback(unsigned int code,const char* msg){
        bannerAdViewRenderFailCallback(code, msg);
    }

    typedef void (*BannerAdViewWillBecomVisibleCallback) ();
    BannerAdViewWillBecomVisibleCallback bannerAdViewWillBecomVisibleCallback;
    void setBannerAdViewWillBecomVisibleCallback(BannerAdViewWillBecomVisibleCallback callback)
    {
        bannerAdViewWillBecomVisibleCallback = callback;
    }
    void onBannerAdViewWillBecomVisibleCallback(){
        bannerAdViewWillBecomVisibleCallback();
    }

    typedef void (*BannerAdViewDidClickCallback) ();
    BannerAdViewDidClickCallback bannerAdViewDidClickCallback;
    void setBannerAdViewDidClickCallback(BannerAdViewDidClickCallback callback)
    {
        bannerAdViewDidClickCallback = callback;
    }
    void onBannerAdViewDidClickCallback(){
        bannerAdViewDidClickCallback();
    }

    typedef void (*BannerAdViewDislikeWithReasonCallback) ();
    BannerAdViewDislikeWithReasonCallback bannerAdViewDislikeWithReasonCallback;
    void setBannerAdViewDislikeWithReasonCallback(BannerAdViewDislikeWithReasonCallback callback)
    {
        bannerAdViewDislikeWithReasonCallback = callback;
    }
    void onBannerAdViewDislikeWithReasonCallback(){
        bannerAdViewDislikeWithReasonCallback();
    }

    typedef void (*BannerAdViewDidCloseOtherControllerCallback) ();
    BannerAdViewDidCloseOtherControllerCallback bannerAdViewDidCloseOtherControllerCallback;
    void setBannerAdViewDidCloseOtherControllerCallback(BannerAdViewDidCloseOtherControllerCallback callback)
    {
        bannerAdViewDidCloseOtherControllerCallback = callback;
    }
    void onBannerAdViewDidCloseOtherControllerCallback(){
        bannerAdViewDidCloseOtherControllerCallback();
    }
}


@interface TTAdBannerAdapter ()<BUNativeExpressBannerViewDelegate>
@property(nonatomic, strong) BUNativeExpressBannerView *bannerView;

@end

@implementation TTAdBannerAdapter

+(TTAdBannerAdapter*) shareInstance
{
    dispatch_once(&predicate, ^{
        _instance =  [[TTAdBannerAdapter alloc] init];
    });
    return _instance;
}

-(id)init
{
    self = [super init];
    return self;
}

-(void) loadAdBanner: (NSString *) slotID
{
     [self.bannerView removeFromSuperview];
    
    UIWindow *window = nil;
    if ([[UIApplication sharedApplication].delegate respondsToSelector:@selector(window)]) {
        window = [[UIApplication sharedApplication].delegate window];
    }
    if (![window isKindOfClass:[UIView class]]) {
        window = [UIApplication sharedApplication].keyWindow;
    }
    if (!window) {
        window = [[UIApplication sharedApplication].windows objectAtIndex:0];
    }
    CGFloat bottom = 0.0;
    if (@available(iOS 11.0, *)) {
        bottom = window.safeAreaInsets.bottom;
    } else {
        // Fallback on earlier versions
    }
    
    CGSize size = CGSizeMake(640, 100);

    self.bannerView = [[BUNativeExpressBannerView alloc] initWithSlotID:slotID rootViewController:UnityGetGLViewController() adSize:size];

    self.bannerView.delegate = self;
    [self.bannerView loadAdData];
}

-(void) showAdBanner{
	[UnityGetGLViewController().view addSubview:self.bannerView];
}

#pragma BUNativeExpressBannerViewDelegate
- (void)nativeExpressBannerAdViewDidLoad:(BUNativeExpressBannerView *)bannerAdView {
    [self pbud_logWithSEL:_cmd msg:@""];
	onBannerAdViewDidLoadCallback();
}

- (void)nativeExpressBannerAdView:(BUNativeExpressBannerView *)bannerAdView didLoadFailWithError:(NSError *)error {

    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"error:%@", error]];
	onBannerAdViewDidLoadFailCallback(error.code,[[NSString stringWithFormat:@"error:%@", error] UTF8String]);
}

- (void)nativeExpressBannerAdViewRenderSuccess:(BUNativeExpressBannerView *)bannerAdView {
    [self pbud_logWithSEL:_cmd msg:@""];
	onBannerAdViewRenderSuccessCallback();
}

- (void)nativeExpressBannerAdViewRenderFail:(BUNativeExpressBannerView *)bannerAdView error:(NSError *)error {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"error:%@", error]];
	onBannerAdViewRenderFailCallback(error.code,[[NSString stringWithFormat:@"error:%@", error] UTF8String]);
}

- (void)nativeExpressBannerAdViewWillBecomVisible:(BUNativeExpressBannerView *)bannerAdView {
    [self pbud_logWithSEL:_cmd msg:@""];
	onBannerAdViewWillBecomVisibleCallback();
}

- (void)nativeExpressBannerAdViewDidClick:(BUNativeExpressBannerView *)bannerAdView {
    [self pbud_logWithSEL:_cmd msg:@""];
	onBannerAdViewDidClickCallback();
}

- (void)nativeExpressBannerAdView:(BUNativeExpressBannerView *)bannerAdView dislikeWithReason:(NSArray<BUDislikeWords *> *)filterwords {
    [UIView animateWithDuration:0.25 animations:^{
        bannerAdView.alpha = 0;
    } completion:^(BOOL finished) {
        [bannerAdView removeFromSuperview];
        self.bannerView = nil;
    }];
    [self pbud_logWithSEL:_cmd msg:@""];
	onBannerAdViewDislikeWithReasonCallback();
}

- (void)nativeExpressBannerAdViewDidCloseOtherController:(BUNativeExpressBannerView *)bannerAdView interactionType:(BUInteractionType)interactionType {
    NSString *str;
    if (interactionType == BUInteractionTypePage) {
        str = @"ladingpage";
    } else if (interactionType == BUInteractionTypeVideoAdDetail) {
        str = @"videoDetail";
    } else {
        str = @"appstoreInApp";
    }
    [self pbud_logWithSEL:_cmd msg:str];
	onBannerAdViewDidCloseOtherControllerCallback();
}
- (void)pbud_logWithSEL:(SEL)sel msg:(NSString *)msg {
    NSLog(@"SDKDemoDelegate BUNativeExpressBannerView In VC (%@) extraMsg:%@", NSStringFromSelector(sel), msg);
}

@end


extern "C"
{
	void loadAdBanner(const char* codeId) {
		NSString *text_codeId = [NSString stringWithUTF8String: codeId];
		[[TTAdBannerAdapter shareInstance] loadAdBanner:text_codeId];
	}

	void showAdBanner() {
		[[TTAdBannerAdapter shareInstance] showAdBanner];
	}
}
