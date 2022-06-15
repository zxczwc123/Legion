#import "AlipayMgr.h"
#import <AlipaySDK/AlipaySDK.h>

static AlipayMgr* _instance = nil;
static dispatch_once_t predicate;


extern "C"
{
    /**
    * 设置监听 与 监听回调
    */
    typedef void (*AliPayCallback) (unsigned int  code,const char* result);
    AliPayCallback aliPayCallback;

    void setAliPayCallback(AliPayCallback callback)
    {
        aliPayCallback = callback;
    }
    
    void onAliPayCallback(unsigned int  code,const char* result){
        aliPayCallback(code,result);
    }

    typedef void (*AliAuthCallback) (unsigned int  code,const char* result);
    AliAuthCallback aliAuthCallback;

    void setAliAuthCallback(AliAuthCallback callback)
    {
        aliAuthCallback = callback;
    }
    
    void onAliAuthCallback(unsigned int  code,const char* result){
        aliAuthCallback(code,result);
    }

}


@implementation AlipayMgr

+(AlipayMgr*) shareInstance
{
    dispatch_once(&predicate, ^{
        _instance =  [[AlipayMgr alloc] init];
    });
    return _instance;
}

-(id)init
{
    self = [super init];
    return self;
}

-(bool) isAliAppInstalled
{
	return true;
}

//
// 选中商品调用支付宝极简支付
//
-(void) pay: (NSString *) orderString
{
    NSLog(@"orderString = %@",orderString);
    
    //应用注册scheme,在AliSDKDemo-Info.plist定义URL types
    NSString *appScheme = @"zwdzz";
    
    // NOTE: 调用支付结果开始支付
    [[AlipaySDK defaultService] payOrder:orderString fromScheme:appScheme callback:^(NSDictionary *resultDic) {
        NSLog(@"reslut = %@",resultDic);
        /* JSON data for obj, or nil if an internal error occurs. The resulting data is a encoded in UTF-8.*/
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:resultDic options:0 error:NULL];
        NSString* reslut =  [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        NSString* resultStatus = [resultDic valueForKey:@"resultStatus"];
        if([resultStatus isEqualToString:@"9000" ]){
            onAliPayCallback(1,[[NSString stringWithFormat:@"%@", reslut] UTF8String]);
        }else{
            onAliPayCallback(0,[[NSString stringWithFormat:@"%@", reslut] UTF8String]);
        }
    }];
}

- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
{
    if ([url.host isEqualToString:@"safepay"]) {
        // 支付跳转支付宝钱包进行支付，处理支付结果
        [[AlipaySDK defaultService] processOrderWithPaymentResult:url standbyCallback:^(NSDictionary *resultDic) {
            NSLog(@"result = %@",resultDic);
        }];

    }
    return YES;
}

@end

extern "C"
{
	/*
     * 初始化
     */
    void initAli(){
		
	}

	/*
     * 是否安装支付宝
     */
    bool isAliAppInstalled(){
		[[AlipayMgr shareInstance] isAliAppInstalled];
	}

	/*
     * 支付
     */
    void payAli(const char* orderInfo){
		NSString *text_orderInfo = [NSString stringWithUTF8String: orderInfo];
		[[AlipayMgr shareInstance] pay:text_orderInfo];
	}
}

