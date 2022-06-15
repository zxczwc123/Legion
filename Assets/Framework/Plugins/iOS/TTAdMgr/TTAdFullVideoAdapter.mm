#import "TTAdFullVideoAdapter.h"
#import <BUAdSDK/BUAdSDK.h>

static dispatch_once_t predicate;
static TTAdFullVideoAdapter* _instance = nil;


extern "C"
{

    typedef void (*FullVideoDidLoadCallback) ();
    FullVideoDidLoadCallback fullVideoDidLoadCallback;
    void setFullVideoDidLoadCallback(FullVideoDidLoadCallback callback)
    {
        fullVideoDidLoadCallback = callback;
    }
    void onFullVideoDidLoadCallback(){
        fullVideoDidLoadCallback();
    }

    typedef void (*FullVideoDidLoadFailCallback) (unsigned int code,const char* msg);
    FullVideoDidLoadFailCallback fullVideoDidLoadFailCallback;
    void setFullVideoDidLoadFailCallback(FullVideoDidLoadFailCallback callback)
    {
        fullVideoDidLoadFailCallback = callback;
    }
    void onFullVideoDidLoadFailCallback(unsigned int code,const char* msg){
        fullVideoDidLoadFailCallback(code,msg);
    }

    typedef void (*FullVideoRenderSuccessCallback) ();
    FullVideoRenderSuccessCallback fullVideoRenderSuccessCallback;
    void setFullVideoRenderSuccessCallback(FullVideoRenderSuccessCallback callback)
    {
        fullVideoRenderSuccessCallback = callback;
    }
    void onFullVideoRenderSuccessCallback(){
        fullVideoRenderSuccessCallback();
    }

    typedef void (*FullVideoRenderFailCallback) (unsigned int code,const char* msg);
    FullVideoRenderFailCallback fullVideoRenderFailCallback;
    void setFullVideoRenderFailCallback(FullVideoRenderFailCallback callback)
    {
        fullVideoRenderFailCallback = callback;
    }
    void onFullVideoRenderFailCallback(unsigned int code,const char* msg){
        fullVideoRenderFailCallback(code,msg);
    }

    typedef void (*FullVideoDidVisibleCallback) ();
    FullVideoDidVisibleCallback fullVideoDidVisibleCallback;
    void setFullVideoDidVisibleCallback(FullVideoDidVisibleCallback callback)
    {
        fullVideoDidVisibleCallback = callback;
    }
    void onFullVideoDidVisibleCallback(){
        fullVideoDidVisibleCallback();
    }

    typedef void (*FullVideoDidCloseCallback) ();
    FullVideoDidCloseCallback fullVideoDidCloseCallback;
    void setFullVideoDidCloseCallback(FullVideoDidCloseCallback callback)
    {
        fullVideoDidCloseCallback = callback;
    }
    void onFullVideoDidCloseCallback(){
        fullVideoDidCloseCallback();
    }

    typedef void (*FullVideoDidClickCallback) ();
    FullVideoDidClickCallback fullVideoDidClickCallback;
    void setFullVideoDidClickCallback(FullVideoDidClickCallback callback)
    {
        fullVideoDidClickCallback = callback;
    }
    void onFullVideoDidClickCallback(){
        fullVideoDidClickCallback();
    }

    typedef void (*FullVideoDidClickSkipCallback) ();
    FullVideoDidClickSkipCallback fullVideoDidClickSkipCallback;
    void setFullVideoDidClickSkipCallback(FullVideoDidClickSkipCallback callback)
    {
        fullVideoDidClickSkipCallback = callback;
    }
    void onFullVideoDidClickSkipCallback(){
        fullVideoDidClickSkipCallback();
    }
}


@interface TTAdFullVideoAdapter ()<BUNativeExpressFullscreenVideoAdDelegate>
@property (nonatomic, strong) BUNativeExpressFullscreenVideoAd *fullscreenAd;
@end

@implementation TTAdFullVideoAdapter

+(TTAdFullVideoAdapter*) shareInstance
{
    dispatch_once(&predicate, ^{
        _instance =  [[TTAdFullVideoAdapter alloc] init];
    });
    return _instance;
}

-(id)init
{
    self = [super init];
    return self;
}

-(void) loadAdFullVideo:(NSString *) slotID {
	self.fullscreenAd = [[BUNativeExpressFullscreenVideoAd alloc] initWithSlotID:slotID];
    self.fullscreenAd.delegate = self;
    [self.fullscreenAd loadAdData];
}

-(void) showAdFullVideo {
	if (self.fullscreenAd) {
        [self.fullscreenAd showAdFromRootViewController:UnityGetGLViewController()];
    }
}

#pragma mark - BUNativeExpressFullscreenVideoAdDelegate
- (void)nativeExpressFullscreenVideoAdDidLoad:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onFullVideoDidLoadCallback();
}

- (void)nativeExpressFullscreenVideoAd:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd didFailWithError:(NSError *_Nullable)error {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"%@", error]];
    onFullVideoDidLoadFailCallback(error.code, [[NSString stringWithFormat:@"%@",error] UTF8String]);
}

- (void)nativeExpressFullscreenVideoAdViewRenderSuccess:(BUNativeExpressFullscreenVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onFullVideoRenderSuccessCallback();
}

- (void)nativeExpressFullscreenVideoAdViewRenderFail:(BUNativeExpressFullscreenVideoAd *)rewardedVideoAd error:(NSError *_Nullable)error {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"%@", error]];
    onFullVideoRenderFailCallback(error.code, [[NSString stringWithFormat:@"%@", error] UTF8String]);
}

- (void)nativeExpressFullscreenVideoAdDidDownLoadVideo:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
}

- (void)nativeExpressFullscreenVideoAdWillVisible:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
}

- (void)nativeExpressFullscreenVideoAdDidVisible:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onFullVideoDidVisibleCallback();
}

- (void)nativeExpressFullscreenVideoAdDidClick:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onFullVideoDidClickCallback();
}

- (void)nativeExpressFullscreenVideoAdDidClickSkip:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onFullVideoDidClickCallback();
}

- (void)nativeExpressFullscreenVideoAdWillClose:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];

}

- (void)nativeExpressFullscreenVideoAdDidClose:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onFullVideoDidCloseCallback();
}

- (void)nativeExpressFullscreenVideoAdDidPlayFinish:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd didFailWithError:(NSError *_Nullable)error {
    [self pbud_logWithSEL:_cmd msg:@""];
}

- (void)nativeExpressFullscreenVideoAdCallback:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd withType:(BUNativeExpressFullScreenAdType) nativeExpressVideoAdType{
    [self pbud_logWithSEL:_cmd msg:@""];
}

- (void)nativeExpressFullscreenVideoAdDidCloseOtherController:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd interactionType:(BUInteractionType)interactionType {
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

#pragma mark - Log
- (void)pbud_logWithSEL:(SEL)sel msg:(NSString *)msg {
    NSLog(@"SDKDemoDelegate BUNativeExpressFullscreenVideoAd In VC (%@) extraMsg:%@", NSStringFromSelector(sel), msg);
}


@end


extern "C"
{
	void loadAdFullVideo(const char* codeId) {
		NSString *text_codeId = [NSString stringWithUTF8String: codeId];
		[[TTAdFullVideoAdapter shareInstance] loadAdFullVideo:text_codeId];
	}

	void showAdFullVideo() {
		[[TTAdFullVideoAdapter shareInstance] showAdFullVideo];
	}

}
