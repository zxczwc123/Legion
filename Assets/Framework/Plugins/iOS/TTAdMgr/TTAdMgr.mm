#import "TTAdMgr.h"
#import <BUAdSDK/BUAdSDK.h>

static TTAdMgr* _instance = nil;
static dispatch_once_t predicate;



@implementation TTAdMgr

+(TTAdMgr*) shareInstance
{
    dispatch_once(&predicate, ^{
        _instance =  [[TTAdMgr alloc] init];
    });
    return _instance;
}

-(id)init
{
    self = [super init];
    return self;
}

-(void) init :(NSString*) appID{
	[BUAdSDKManager setAppID: appID];
}

@end

extern "C"
{
	/*
     * 初始化
     */
    void init(const char* appID){
		NSString *text_appID = [NSString stringWithUTF8String: appID];
		[[TTAdMgr shareInstance] init:text_appID];
	}

}

