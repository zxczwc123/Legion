#import "WeChatMgr.h"
#import <UIKit/UIKit.h>
#import "WXApi.h"

#define WetchatAppID @"wx44a7d0eb068d0024"

static WeChatMgr* _instance = nil;
static dispatch_once_t predicate;


extern "C"
{

    /**
    * native 回调 unity （设置监听 与 监听回调）
    */
    typedef void (*WeChatPayCallback) (unsigned int code,const char* orderId);
    WeChatPayCallback weChatPayCallback;

    void setWeChatPayCallback(WeChatPayCallback callback)
    {
        weChatPayCallback = callback;
    }
    
    void onWeChatPayCallback(unsigned int code,const char* orderId){
        weChatPayCallback(code,orderId);
    }

    typedef void (*WeChatLoginCallback) (const  char*  code);
    WeChatLoginCallback weChatLoginCallback;

    void setWeChatLoginCallback(WeChatLoginCallback callback)
    {
        weChatLoginCallback = callback;
    }
    
    void onWeChatLoginCallback(const char* code){
        weChatLoginCallback(code);
    }

    typedef void (*WeChatShareCallback) (unsigned int code);
    WeChatShareCallback weChatShareCallback;

    void setWeChatShareCallback(WeChatShareCallback callback)
    {
        weChatShareCallback = callback;
    }
    
    void onWeChatShareCallback(unsigned int  code){
        weChatShareCallback(code);
    }

    typedef void (*WeChatMiniProgramCallback) (const char* extraData);
    WeChatMiniProgramCallback weChatMiniProgramCallback;

    void setWeChatMiniProgramCallback(WeChatMiniProgramCallback callback)
    {
        weChatMiniProgramCallback = callback;
    }
    
    void onWeChatMiniProgramCallback(const char* extraData){
        weChatMiniProgramCallback(extraData);
    }
}

@implementation WeChatMgr

+(WeChatMgr*) shareInstance
{
    dispatch_once(&predicate, ^{
        _instance =  [[WeChatMgr alloc] init];
    });
    return _instance;
}

-(id)init
{
    self = [super init];
    return self;
}


// appdelegate 调用部分
- (void)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    //向微信注册
    [WXApi registerApp:WetchatAppID universalLink:@"https://down.tking996.com"];
}

- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation {
    return [WXApi handleOpenURL:url delegate:self];
}

- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url {
    return  [WXApi handleOpenURL:url delegate:self];
}

- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void(^)(NSArray<id<UIUserActivityRestoring>> * __nullable restorableObjects))restorationHandler {
    return [WXApi handleOpenUniversalLink:userActivity delegate:self];
}
// appdelegate 调用部分

#pragma mark - WXApiDelegate
- (void)onResp:(BaseResp *)resp {
    if([resp isKindOfClass:[PayResp class]]){
        //支付返回结果，实际支付结果需要去微信服务器端查询
        NSString *strMsg,*strTitle = [NSString stringWithFormat:@"支付结果"];
        
        switch (resp.errCode) {
            case WXSuccess:
                strMsg = @"支付结果：成功！";
                NSLog(@"支付成功－PaySuccess，retcode = %d", resp.errCode);
				onWeChatPayCallback(resp.errCode, "");
                break;
            default:
                strMsg = [NSString stringWithFormat:@"支付结果：失败！retcode = %d, retstr = %@", resp.errCode,resp.errStr];
                NSLog(@"错误，retcode = %d, retstr = %@", resp.errCode,resp.errStr);
				onWeChatPayCallback(resp.errCode,[resp.errStr UTF8String]);
                break;
        }
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:strTitle message:strMsg delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil, nil];
        [alert show];
    } else if ([resp isKindOfClass:[SendMessageToWXResp class]]) {
		// 分享回调
		SendMessageToWXResp *messageResp = (SendMessageToWXResp *)resp;
		onWeChatShareCallback(1);
    } else if ([resp isKindOfClass:[SendAuthResp class]]) {
		// 登陆回调
		SendAuthResp *authResp = (SendAuthResp *)resp;
        onWeChatLoginCallback([authResp.code UTF8String]);
    } else if ([resp isKindOfClass:[AddCardToWXCardPackageResp class]]) {
		AddCardToWXCardPackageResp *addCardResp = (AddCardToWXCardPackageResp *)resp;
    } else if ([resp isKindOfClass:[WXChooseCardResp class]]) {
		WXChooseCardResp *chooseCardResp = (WXChooseCardResp *)resp;
    } else if ([resp isKindOfClass:[WXChooseInvoiceResp class]]){
        WXChooseInvoiceResp *chooseInvoiceResp = (WXChooseInvoiceResp *)resp;
    } else if ([resp isKindOfClass:[WXSubscribeMsgResp class]]){
		WXSubscribeMsgResp * subscribeMsgResp = (WXSubscribeMsgResp *)resp;
    } else if ([resp isKindOfClass:[WXLaunchMiniProgramResp class]]){
		// 小程序回调 
		WXLaunchMiniProgramResp *launchMiniProgramResp = (WXLaunchMiniProgramResp *)resp;
		onWeChatMiniProgramCallback([launchMiniProgramResp.extMsg UTF8String]);
    } else if([resp isKindOfClass:[WXInvoiceAuthInsertResp class]]){
        WXInvoiceAuthInsertResp *invoiceAuthInsertResp = (WXInvoiceAuthInsertResp *) resp;
    } else if([resp isKindOfClass:[WXNontaxPayResp class]]){
        WXNontaxPayResp *nontaxPayResp = (WXNontaxPayResp *)resp;
    } else if ([resp isKindOfClass:[WXPayInsuranceResp class]]){
        WXPayInsuranceResp* payInsuranceResp = (WXPayInsuranceResp *)resp;
    }
}

- (void)onReq:(BaseReq *)req {
	if ([req isKindOfClass:[ShowMessageFromWXReq class]]) {
        ShowMessageFromWXReq *showMessageReq = (ShowMessageFromWXReq *)req;
    } else if ([req isKindOfClass:[LaunchFromWXReq class]]) {
        LaunchFromWXReq *launchReq = (LaunchFromWXReq *)req;
    }
}

-(void) initWeChat{
		
}

-(bool) isWXAppInstalled{
    return [WXApi isWXAppInstalled];
}

-(void) login{
    SendAuthReq* req = [[SendAuthReq alloc] init];
    req.scope = @"snsapi_userinfo";
    req.openID = WetchatAppID;
    req.state = @"only123";
    [WXApi sendAuthReq:req viewController:UnityGetGLViewController() delegate:self completion:^(BOOL success){
        
    }];
}

-(void) shareText:(NSString*) title withText:(NSString*) text withScene:(int) scene {
	SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
	req.bText = YES;
	req.text = text;
	req.scene = scene;
    [WXApi sendReq:req completion:^(BOOL success){}];
}

-(void) shareUrl:(NSString*) title withUrl:(NSString*) url withDescription:(NSString*)description withScene:(int) scene{
	WXWebpageObject *webpageObject = [WXWebpageObject object];
	webpageObject.webpageUrl = url;
	WXMediaMessage *message = [WXMediaMessage message];
	message.title = title;
	message.description = description;
	[message setThumbImage:[UIImage imageNamed:@"AppIcon72x72"]];
	message.mediaObject = webpageObject;
	SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
	req.bText = NO;
	req.message = message;
	req.scene = scene;
    [WXApi sendReq:req completion:^(BOOL success){}];
}

-(void) shareImage:(NSString*) title withImage: (NSString*)imagePath withDescription:(NSString*)description withScene: (int)scene{
    WXMediaMessage *message = [WXMediaMessage message];
    UIImage *image =[UIImage imageWithContentsOfFile:imagePath];
    float _size = 1280.0f;
    float _scale = image.size.width / _size;
    if(_scale > 1)
        image = [self reSizeImage:image toSize:CGSizeMake(_size, image.size.height /_scale)];
    
    [UIImageJPEGRepresentation(image,1)writeToFile:imagePath atomically:YES];
   
    [message setTitle:title];
    [message setDescription:description];
    [message setThumbImage:[self reSizeImage:image toSize:CGSizeMake(64, 64)]];
    
    WXImageObject *imageObject = [WXImageObject object];
    imageObject.imageData = [NSData dataWithContentsOfFile:imagePath];
    message.mediaObject = imageObject;

	SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
	req.bText = NO;
	req.message = message;
	req.scene = scene;
    [WXApi sendReq:req completion:^(BOOL success){}];
}

- (UIImage *)reSizeImage:(UIImage *)image toSize:(CGSize)reSize{
    UIGraphicsBeginImageContext(CGSizeMake(reSize.width, reSize.height));
    [image drawInRect:CGRectMake(0, 0, reSize.width, reSize.height)];
    UIImage *reSizeImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return reSizeImage;
}

-(void) pay:(NSString *) partnerid withPrepayid:(NSString *) prepayid withNoncestr:(NSString*)noncestr withTimestamp: (NSString*) timestamp withPack:(NSString*)pack withSign: (NSString*)sign{
                
    //调起微信支付
    PayReq* req             = [[PayReq alloc] init];
    req.partnerId           = partnerid;
    req.prepayId            = prepayid;
    req.nonceStr            = noncestr;
    req.timeStamp           = [timestamp longLongValue];
    req.package             = pack;
    req.sign                = sign;
    [WXApi sendReq:req completion:^(BOOL success){}];
    //日志输出
    NSLog(@"partid=%@\nprepayid=%@\nnoncestr=%@\ntimestamp=%ld\npackage=%@\nsign=%@",req.partnerId,req.prepayId,req.nonceStr,(long)req.timeStamp,req.package,req.sign );
}

-(void) launchMiniProgram:(NSString*) username withPath: (NSString*)path{
	WXLaunchMiniProgramReq *launchMiniProgramReq = [WXLaunchMiniProgramReq object];
	launchMiniProgramReq.userName = username;  //拉起的小程序的username
	launchMiniProgramReq.path = path;    ////拉起小程序页面的可带参路径，不填默认拉起小程序首页，对于小游戏，可以只传入 query 部分，来实现传参效果，如：传入 "?foo=bar"。
	launchMiniProgramReq.miniProgramType = WXMiniProgramTypeRelease; //拉起小程序的类型
    [WXApi sendReq:launchMiniProgramReq completion:^(BOOL success){}];
}

@end



extern "C"
{
    // unity 调用 native
    void initWeChat(){
        [[WeChatMgr shareInstance] initWeChat];
    }

    bool isWXAppInstalled(){
        return true;
    }

    void loginWeChat(){
        [[WeChatMgr shareInstance] login];
    }

    void shareText(const char* title, const char* text, int scene){
        NSString *text_title = [NSString stringWithUTF8String: title];
        NSString *text_text = [NSString stringWithUTF8String: text];
        [[WeChatMgr shareInstance] shareText: text_title withText: text_text withScene: scene];
    }

    void shareUrl(const char* title, const char* url, const char* description, int scene){
        NSString *text_title = [NSString stringWithUTF8String: title];
        NSString *text_url = [NSString stringWithUTF8String: url];
        NSString *text_description = [NSString stringWithUTF8String: description];
        [[WeChatMgr shareInstance] shareUrl: text_title withUrl: text_url withDescription:text_description withScene: scene];
    }

    void shareImage(const char* title, const char* imagePath, const char* description, int scene){
        NSString *text_title = [NSString stringWithUTF8String: title];
        NSString *text_imagePath = [NSString stringWithUTF8String: imagePath];
        NSString *text_description = [NSString stringWithUTF8String: description];
        [[WeChatMgr shareInstance] shareImage: text_title withImage: text_imagePath withDescription:text_description withScene: scene];
    }

    void pay(const char* partnerid, const char* prepayid, const char* noncestr, const char* timestamp, const char* pack, const char* sign){
        NSString *text_partnerid = [NSString stringWithUTF8String: partnerid];
        NSString *text_prepayid = [NSString stringWithUTF8String: prepayid];
        NSString *text_noncestr = [NSString stringWithUTF8String: noncestr];
        NSString *text_timestamp = [NSString stringWithUTF8String: timestamp];
        NSString *text_pack = [NSString stringWithUTF8String: pack];
        NSString *text_sign = [NSString stringWithUTF8String: sign];
        [[WeChatMgr shareInstance] pay: text_partnerid withPrepayid: text_prepayid withNoncestr:text_noncestr withTimestamp: text_timestamp withPack:text_pack withSign: text_sign];
    }

    void launchMiniProgram(const char* username, const char* path){
        NSString *text_username = [NSString stringWithUTF8String: username];
        NSString *text_path = [NSString stringWithUTF8String: path];
        [[WeChatMgr shareInstance] launchMiniProgram: text_username withPath: text_path];
    }
}

