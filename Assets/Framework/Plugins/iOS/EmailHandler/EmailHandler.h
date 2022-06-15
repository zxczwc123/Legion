#import <Foundation/Foundation.h>

@interface EmailHandler : NSObject

+(EmailHandler*) shareInstance;

-(void) openEmail:(NSString*) uri;

@end
