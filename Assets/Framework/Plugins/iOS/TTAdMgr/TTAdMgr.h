#import <Foundation/Foundation.h>

@interface TTAdMgr : NSObject

+(TTAdMgr*) shareInstance;

-(id) init;

-(void) init: (NSString*) appID;

@end
