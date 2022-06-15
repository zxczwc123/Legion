#import "EmailHandler.h"

static EmailHandler* _instance = nil;
static dispatch_once_t predicate;

@implementation EmailHandler

+(EmailHandler*) shareInstance
{
    dispatch_once(&predicate, ^{
        _instance =  [[EmailHandler alloc] init];
    });
    return _instance;
}

-(id)init{
    self = [super init];
    return self;
}

-(void) openEmail:(NSString*) uri{
    NSString* url =[NSString stringWithFormat:@"%@%@",@"mailto://",uri];
    NSURL * urlStr = [NSURL URLWithString:url];
    [[UIApplication sharedApplication] openURL:urlStr];
}
@end

extern "C"
{
    /*
     * 打开邮箱
     */
    void openEmail(const char* uri)
    {
        NSString* url = [NSString stringWithUTF8String:uri];
        [[EmailHandler shareInstance] openEmail:url];
    }
}
