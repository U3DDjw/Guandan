//
//  Clipboard.m
//  Unity-iPhone
//
//  Created by haoyun on 2017/8/5.
//
//

#import <Foundation/Foundation.h>


@interface Clipboard : NSObject


    /*  compare the namelist with system processes  */
    void _copyTextToClipboard(const char *textList);


@end


@implementation Clipboard
//将文本复制到IOS剪贴板
- (void)objc_copyTextToClipboard : (NSString*)text
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = text;
}
@end

    static Clipboard *iosClipboard;
    void _copyTextToClipboard(const char *textList)
    {
        NSString *text = [NSString stringWithUTF8String: textList] ;
        if(iosClipboard == NULL)
        {
            iosClipboard = [[Clipboard alloc] init];
        }
        [iosClipboard objc_copyTextToClipboard: text];
    }  
