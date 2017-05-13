#import <Foundation/Foundation.h>
#import "UIKit/UIKit.h"
#import "UnityAppController.h"
#import "Reachability.h"
#import "AudioToolbox/AudioToolbox.h"
#if defined(__cplusplus)
extern "C"{
#endif
    extern void iOS_weixin_login();
    extern void iOS_weixin_invite(char* str1,char* str2,char* str3);
    extern void iOS_weixin_openweb(char* str);
    extern void iOS_weixin_scenetimeline(char* str1,char* str2,char* str3);
    extern void iOS_weixin_screenshot(char* str,int len);
    extern int iOS_askNetType();
    extern float iOS_askBattery();
    extern void iOS_weixin_phoneshake();
#if defined(__cplusplus)
}
#endif


#if defined(__cplusplus)
extern "C"{
#endif
    //WXApi.h  isWxAppInstalled 是否安装微信
    void iOS_weixin_login()
    {
        NSLog(@"login++++++++");
        UnityAppController* controller = (UnityAppController*)[UIApplication sharedApplication].delegate;
        [controller sendAuthRequest];
    }
    void iOS_weixin_invite(char* str1,char* str2,char* str3){
        NSLog(@"invite++++++++");
        UnityAppController* controller = (UnityAppController*)[UIApplication sharedApplication].delegate;
        NSString* msg1 = [[NSString alloc] initWithCString:(const char*)str1 encoding:NSUTF8StringEncoding];//NSASCIIStringEncoding
        NSString* msg2 = [[NSString alloc] initWithCString:(const char*)str2 encoding:NSUTF8StringEncoding];
        NSString* msg3 = [[NSString alloc] initWithCString:(const char*)str3 encoding:NSUTF8StringEncoding];
        NSLog(@"*************");
        NSLog(@"%@",msg1);
        NSLog(@"*************");
        [controller sendURL: msg1 secondMsg: msg2 thridMsg: msg3];
    }
    void iOS_weixin_openweb(char* str){
        NSLog(@"OpenWeb++++++");
        //NSString* str = @"http://a.app.qq.com/o/simple.jsp?pkgname=com.xianlai.mahjongguangxi";
        NSString* url = [[NSString alloc] initWithCString:(const char*)str encoding:NSUTF8StringEncoding];
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:url]];
    }
    void iOS_weixin_scenetimeline(char* str1,char* str2,char* str3){
        NSLog(@"scenetimeline++++++++");
        UnityAppController* controller = (UnityAppController*)[UIApplication sharedApplication].delegate;
        NSString* msg1 = [[NSString alloc] initWithCString:(const char*)str1 encoding:NSUTF8StringEncoding];
        NSString* msg2 = [[NSString alloc] initWithCString:(const char*)str2 encoding:NSUTF8StringEncoding];
        NSString* msg3 = [[NSString alloc] initWithCString:(const char*)str3 encoding:NSUTF8StringEncoding];
        [controller sendWXSceneTimeline: msg1 secondMsg: msg2 thridMsg: msg3];
    }
    void iOS_weixin_screenshot(char* str,int len){
        NSLog(@"screenshot++++++++");
        NSData *adata = [[NSData alloc] initWithBytes:str length:len];
        UnityAppController* controller = (UnityAppController*)[UIApplication sharedApplication].delegate;
        [controller sendWXScreenshot: adata];
    }
    int iOS_askNetType(){
        
        int type = -1;
        BOOL isExistenceNetwork;
        Reachability *reachability = [Reachability reachabilityWithHostName:@"www.apple.com"];
        switch([reachability currentReachabilityStatus]){
            case NotReachable: isExistenceNetwork = FALSE;
                type= -1;
                NSLog(@"没网");
                break;
            case ReachableViaWWAN: isExistenceNetwork = TRUE;
                type = 2;
                NSLog(@"有网");
                break;
            case ReachableViaWiFi: isExistenceNetwork = TRUE;
                type = 1;
                NSLog(@"wifi");
                break;
        }
        return type;
    }
    float iOS_askBattery(){
        UIDevice * device = [UIDevice currentDevice];
        device.batteryMonitoringEnabled = true;
        float level = device.batteryLevel;
        NSLog(@"level = %lf",level);
        
        return level;
    }
    void iOS_weixin_phoneshake(){
        AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
    }
#if defined(__cplusplus)
}
#endif
