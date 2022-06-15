#import <Foundation/Foundation.h>
#import "WXApi.h"

@interface WeChatMgr : NSObject<WXApiDelegate>

+(WeChatMgr*) shareInstance;

-(id)init;

-(bool)isWeChatAppInstalled;

-(void)pay: (NSString *)orderInfo;

- (void)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions ;

- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url ;

- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation ;

- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void(^)(NSArray<id<UIUserActivityRestoring>> * __nullable restorableObjects))restorationHandler ;
@end
