#import "CommonUtil.h"
#import <AdSupport/AdSupport.h>

@implementation CommonUtil

+(NSString *) getDeviceId{
    NSString *idfa = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
    return idfa;
}

+(NSString *) getDeviceModel{
	
	NSString *deviceModel = [[UIDevice currentDevice] model];
	return  deviceModel;
}

+(void) copyTextToClipboard : (NSString*)data{
	UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
	pasteboard.string = data;
}


+(void) openQQChat:(NSString *) qqNumber{
	if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"mqq://"]]) {
		NSURL * url=[NSURL URLWithString:[NSString stringWithFormat:@"mqq://im/chat?chat_type=wpa&uin=%@&version=1&src_type=web",qqNumber]];
        [[UIApplication sharedApplication] openURL:url];
	}else{
        NSString *strTitle = [NSString stringWithFormat:@"支付结果"];
        NSString *strMsg = [NSString stringWithFormat:@"未安装QQ"];
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:strTitle message:strMsg delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil, nil];
        [alert show];
	}
}

@end

extern "C"
{
	#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

	void openQQChat(const char* qq){
		NSString *text = [NSString stringWithUTF8String: qq] ;
		[CommonUtil openQQChat: text];
	}

    void copyTextToClipboard(const char* data){
		NSString *text = [NSString stringWithUTF8String: data] ;
		[CommonUtil copyTextToClipboard: text];
	}

    const char* getDeviceId(){
		NSString *deviceId = [CommonUtil getDeviceId];
		return MakeStringCopy(deviceId) ;
	}

    const char* getDeviceModel(){
		NSString *deviceModel = [CommonUtil getDeviceModel];
		return MakeStringCopy(deviceModel);
	}

}

