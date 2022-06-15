#import "TTAdRewardVideoAdapter.h"
#import <BUAdSDK/BUAdSDK.h>

static TTAdRewardVideoAdapter* _instance = nil;
static dispatch_once_t predicate;


extern "C"
{
    typedef void (*RewardVideoDidLoadCallback) ();
    RewardVideoDidLoadCallback rewardVideoDidLoadCallback;
    void setRewardVideoDidLoadCallback(RewardVideoDidLoadCallback callback)
    {
        rewardVideoDidLoadCallback = callback;
    }
    void onRewardVideoDidLoadCallback(){
        rewardVideoDidLoadCallback();
    }

    typedef void (*RewardVideoDidLoadFailCallback) (unsigned int code,const char* msg);
    RewardVideoDidLoadFailCallback rewardVideoDidLoadFailCallback;
    void setRewardVideoDidLoadFailCallback(RewardVideoDidLoadFailCallback callback)
    {
       rewardVideoDidLoadFailCallback = callback;
    }
    void onRewardVideoDidLoadFailCallback(unsigned int code,const char* msg){
        rewardVideoDidLoadFailCallback(code, msg);
    }

    typedef void (*RewardVideoRenderSuccessCallback) ();
    RewardVideoRenderSuccessCallback rewardVideoRenderSuccessCallback;
    void setRewardVideoRenderSuccessCallback(RewardVideoRenderSuccessCallback callback)
    {
       rewardVideoRenderSuccessCallback = callback;
    }
    void onRewardVideoRenderSuccessCallback(){
        rewardVideoRenderSuccessCallback();
    }

    typedef void (*RewardVideoRenderFailCallback) (unsigned int code,const char* msg);
    RewardVideoRenderFailCallback rewardVideoRenderFailCallback;
    void setRewardVideoRenderFailCallback(RewardVideoRenderFailCallback callback)
    {
       rewardVideoRenderFailCallback = callback;
    }
    void onRewardVideoRenderFailCallback(unsigned int code,const char* msg){
        rewardVideoRenderFailCallback(code, msg);
    }

    typedef void (*RewardVideoDidVisibleCallback) ();
    RewardVideoDidVisibleCallback rewardVideoDidVisibleCallback;
    void setRewardVideoDidVisibleCallback(RewardVideoDidVisibleCallback callback)
    {
       rewardVideoDidVisibleCallback = callback;
    }
    void onRewardVideoDidVisibleCallback(){
        rewardVideoDidVisibleCallback();
    }

    typedef void (*RewardVideoDidCloseCallback) ();
    RewardVideoDidCloseCallback rewardVideoDidCloseCallback;
    void setRewardVideoDidCloseCallback(RewardVideoDidCloseCallback callback)
    {
       rewardVideoDidCloseCallback = callback;
    }
    void onRewardVideoDidCloseCallback(){
        rewardVideoDidCloseCallback();
    }

    typedef void (*RewardVideoDidClickCallback) ();
    RewardVideoDidClickCallback rewardVideoDidClickCallback;
    void setRewardVideoDidClickCallback(RewardVideoDidClickCallback callback)
    {
       rewardVideoDidClickCallback = callback;
    }
    void onRewardVideoDidClickCallback(){
        rewardVideoDidClickCallback();
    }

    typedef void (*RewardVideoDidClickSkipCallback) ();
    RewardVideoDidClickSkipCallback rewardVideoDidClickSkipCallback;
    void setRewardVideoDidClickSkipCallback(RewardVideoDidClickSkipCallback callback)
    {
       rewardVideoDidClickSkipCallback = callback;
    }
    void onRewardVideoDidClickSkipCallback(){
        rewardVideoDidClickSkipCallback();
    }

    typedef void (*RewardVideoServerRewardDidSuccessCallback) ();
    RewardVideoServerRewardDidSuccessCallback rewardVideoServerRewardDidSuccessCallback;
    void setRewardVideoServerRewardDidSuccessCallback(RewardVideoServerRewardDidSuccessCallback callback)
    {
        rewardVideoServerRewardDidSuccessCallback = callback;
    }
    void onRewardVideoServerRewardDidSuccessCallback(){
        rewardVideoServerRewardDidSuccessCallback();
    }

    typedef void (*RewardVideoServerRewardDidFailCallback) (unsigned int code,const char* msg);
    RewardVideoServerRewardDidFailCallback rewardVideoServerRewardDidFailCallback;
    void setRewardVideoServerRewardDidFailCallback(RewardVideoServerRewardDidFailCallback callback)
    {
        rewardVideoServerRewardDidFailCallback = callback;
    }
    void onRewardVideoServerRewardDidFailCallback(unsigned int code,const char* msg){
        rewardVideoServerRewardDidFailCallback(code,msg);
    }
}


@interface TTAdRewardVideoAdapter ()<BUNativeExpressRewardedVideoAdDelegate>
@property (nonatomic, strong) BUNativeExpressRewardedVideoAd *rewardedAd;
@end

@implementation TTAdRewardVideoAdapter

+(TTAdRewardVideoAdapter*) shareInstance
{
    dispatch_once(&predicate, ^{
        _instance =  [[TTAdRewardVideoAdapter alloc] init];
    });
    return _instance;
}

-(id)init
{
    self = [super init];
    return self;
}

-(void) loadAdRewardVideo: (NSString *) slotID
{
	BURewardedVideoModel *model = [[BURewardedVideoModel alloc] init];
	model.userId = @"10505";
	self.rewardedAd = [[BUNativeExpressRewardedVideoAd alloc] initWithSlotID:slotID rewardedVideoModel:model];
	self.rewardedAd.delegate = self;
	[self.rewardedAd loadAdData];
}

-(void) showAdRewardVideo{
	if (self.rewardedAd) {
		[self.rewardedAd showAdFromRootViewController:UnityGetGLViewController()];
	}
}

#pragma mark - BUNativeExpressRewardedVideoAdDelegate
- (void)nativeExpressRewardedVideoAdDidLoad:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onRewardVideoDidLoadCallback();
}

- (void)nativeExpressRewardedVideoAd:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *_Nullable)error {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"%@", error]];
    onRewardVideoDidLoadFailCallback(error.code, [[NSString stringWithFormat:@"%@", error] UTF8String]);
}

- (void)nativeExpressRewardedVideoAdCallback:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd withType:(BUNativeExpressRewardedVideoAdType)nativeExpressVideoType{
    [self pbud_logWithSEL:_cmd msg:@""];
    
}

- (void)nativeExpressRewardedVideoAdDidDownLoadVideo:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
}

- (void)nativeExpressRewardedVideoAdViewRenderSuccess:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onRewardVideoRenderSuccessCallback();
}

- (void)nativeExpressRewardedVideoAdViewRenderFail:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd error:(NSError *_Nullable)error {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"%@", error]];
    onRewardVideoRenderFailCallback(error.code, [[NSString stringWithFormat:@"%@", error] UTF8String]);
}

- (void)nativeExpressRewardedVideoAdWillVisible:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    
}

- (void)nativeExpressRewardedVideoAdDidVisible:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onRewardVideoDidVisibleCallback();
}

- (void)nativeExpressRewardedVideoAdWillClose:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
}

- (void)nativeExpressRewardedVideoAdDidClose:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onRewardVideoDidCloseCallback();
}

- (void)nativeExpressRewardedVideoAdDidClick:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onRewardVideoDidClickCallback();
}

- (void)nativeExpressRewardedVideoAdDidClickSkip:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    [self pbud_logWithSEL:_cmd msg:@""];
    onRewardVideoDidClickSkipCallback();
}

- (void)nativeExpressRewardedVideoAdDidPlayFinish:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *_Nullable)error {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"%@", error]];
    onRewardVideoServerRewardDidSuccessCallback();
}

- (void)nativeExpressRewardedVideoAdServerRewardDidSucceed:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd verify:(BOOL)verify {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"verify:%@ rewardName:%@ rewardMount:%ld",verify?@"true":@"false",rewardedVideoAd.rewardedVideoModel.rewardName,(long)rewardedVideoAd.rewardedVideoModel.rewardAmount]];
    onRewardVideoServerRewardDidSuccessCallback();
}

- (void)nativeExpressRewardedVideoAdServerRewardDidFail:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd error:(NSError * _Nullable)error {
    [self pbud_logWithSEL:_cmd msg:[NSString stringWithFormat:@"rewardName:%@ rewardMount:%ld error:%@",rewardedVideoAd.rewardedVideoModel.rewardName,(long)rewardedVideoAd.rewardedVideoModel.rewardAmount,error]];
    onRewardVideoServerRewardDidFailCallback(error.code, [[NSString stringWithFormat:@"%@" ,error] UTF8String]);
}

- (void)nativeExpressRewardedVideoAdDidCloseOtherController:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd interactionType:(BUInteractionType)interactionType {
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
    NSLog(@"SDKDemoDelegate BUNativeExpressRewardedVideoAd In VC (%@) extraMsg:%@", NSStringFromSelector(sel), msg);
}

@end


extern "C"
{
	void loadAdRewardVideo(const char* codeId) {
		NSString *text_codeId = [NSString stringWithUTF8String: codeId];
		[[TTAdRewardVideoAdapter shareInstance] loadAdRewardVideo:text_codeId];
	}
	
	void showAdRewardVideo() {
		[[TTAdRewardVideoAdapter shareInstance] showAdRewardVideo];
	}
}
