//
//  UIImage+fixOrientation.h
//  CameraSample
//
//  Created by Noor Mohammad on 4/30/16.
//  Copyright © 2016 Tiam. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface UIImage (fixOrientation)

- (UIImage *)fixOrientation;
+ (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize;

@end
