#import <Foundation/Foundation.h>

@interface CommonUtil : NSObject

+ (NSString *)getDeviceId;

+(NSString *)getDeviceModel;

+(void)copyTextToClipboard: (NSString*)data;

@end
