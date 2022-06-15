#import <Foundation/Foundation.h>

@interface AlipayMgr : NSObject

+(AlipayMgr*) shareInstance;

-(id)init;

-(bool)isAliAppInstalled;

-(void)pay : (NSString *)orderInfo;

- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;
@end
