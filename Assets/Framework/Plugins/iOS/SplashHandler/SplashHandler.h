#import <Foundation/Foundation.h>

@interface SplashHandler : NSObject

@property(nonatomic,strong) UIWindow* window;

@property(nonatomic,strong) UIView* view;

@property(nonatomic,strong) UIImageView* imageView;

+(SplashHandler*) shareInstance;

-(void) showSplash:(UIWindow*) window;

-(void) hideSplash;
@end
