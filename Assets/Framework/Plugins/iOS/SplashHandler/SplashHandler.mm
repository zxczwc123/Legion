#import "SplashHandler.h"

static SplashHandler* _instance = nil;
static dispatch_once_t predicate;

@implementation SplashHandler

+(SplashHandler*) shareInstance
{
    dispatch_once(&predicate, ^{
        _instance =  [[SplashHandler alloc] init];
    });
    return _instance;
}

-(id)init{
    self = [super init];
    return self;
}

-(void) showSplash:(UIWindow*) window
{
    self.window = window;
    // 启动splash 关闭太快加一层
    UIImageView  *imageView=[[UIImageView alloc] initWithFrame:CGRectMake(0, 0, window.bounds.size.width, window.bounds.size.height)];
    [imageView setImage:[UIImage imageNamed:@"splash_logo.png"]];
    imageView.center = window.center;
    imageView.backgroundColor = UIColor.blackColor;
    imageView.contentMode = UIViewContentModeScaleAspectFill;
    [self.window addSubview:imageView];
    self.imageView = imageView;
}

-(void) hideSplash
{
    if(self.imageView){
        [self.imageView removeFromSuperview];
    }
}

@end

extern "C"
{

    /*
     * 隐藏splash
     */
    void hideSplash()
    {
        [[SplashHandler shareInstance] hideSplash];
    }
}

