//
//  CameraHelper.m
//  CameraSample
//
//  Created by Noor Mohammad on 1/19/16.
//  Copyright © 2016 Tiam. All rights reserved.
//

#import "CameraHelper.h"
#import "UIImage+fixOrientation.h"
#import <Photos/Photos.h>//;

@implementation CameraHelper{
    NSString *chosenImageLocation;
}
extern "C" void UnitySendMessage(const char *, const char *, const char *);


-(void)cameraOpen{

    AVAuthorizationStatus avAuthorizationStatus = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
    if(avAuthorizationStatus == AVAuthorizationStatusAuthorized) {
        UIImagePickerController *pickerViewController = [[UIImagePickerController alloc] init];
        [pickerViewController setDelegate:self];
        pickerViewController.allowsEditing = YES;
        pickerViewController.sourceType = UIImagePickerControllerSourceTypeCamera;
        [[[[[UIApplication sharedApplication] windows] objectAtIndex:0] rootViewController] presentViewController:pickerViewController animated:YES completion:nil];
    } else if(avAuthorizationStatus == AVAuthorizationStatusRestricted ||avAuthorizationStatus == AVAuthorizationStatusDenied ||avAuthorizationStatus == AVAuthorizationStatusNotDetermined){
        [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
            if(granted == false) {
                UnitySendMessage("CameraOrGallery", "AuthSettingOff", "");
            }
        }];
    }
}

-(void)galleryOpen{

    if (NSFoundationVersionNumber > NSFoundationVersionNumber_iOS_7_0) {
        PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatus];
        if (status == PHAuthorizationStatusAuthorized) {
            UIImagePickerController *pickerViewController = [[UIImagePickerController alloc] init];
            [pickerViewController setDelegate:self];
            pickerViewController.allowsEditing = YES;
            pickerViewController.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;

            [[[[[UIApplication sharedApplication] windows] objectAtIndex:0] rootViewController] presentViewController:pickerViewController animated:YES completion:nil];
        } else if (status == PHAuthorizationStatusDenied || status == PHAuthorizationStatusNotDetermined || status == PHAuthorizationStatusRestricted) {
            // Access has not been determined.
            [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {
                NSLog(@"------------------------------");
                if (status != PHAuthorizationStatusAuthorized) {
                    UnitySendMessage("CameraOrGallery", "AuthSettingOff", "");
                }
            }];
        }
    }
}


#pragma mark - UIImagePickerViewDelegate

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info{
    
    
    UIImage *myUIImage = [info objectForKey:UIImagePickerControllerOriginalImage];
    
    myUIImage = [UIImage imageWithImage:myUIImage scaledToSize:CGSizeMake(640, 960)];
    
    
    
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    
    // 適当なファイル名をつける.
    NSString *filePath;
    // 適当なファイル名をつける.
    if(picker.sourceType == UIImagePickerControllerSourceTypePhotoLibrary){
        
        filePath = [documentsDirectory stringByAppendingPathComponent:@"tmp_library.jpg"];
        
    } else if (picker.sourceType == UIImagePickerControllerSourceTypeCamera) {
        
        myUIImage = [myUIImage fixOrientation];
        filePath = [documentsDirectory stringByAppendingPathComponent:@"tmp_camera.jpg"];
    }
    
    NSData *imageData = UIImageJPEGRepresentation(myUIImage, 0.5);

    [imageData writeToFile:filePath atomically:YES];
    char* _mstSelectedImage;
    _mstSelectedImage = (char *)[(NSMutableString *)filePath UTF8String];
    //Here We Get Image
    UnitySendMessage("CameraOrGallery", "PictCatch", _mstSelectedImage);

    [picker dismissViewControllerAnimated:YES completion:nil];
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker{

    [picker dismissViewControllerAnimated:YES completion:nil];
}

-(NSString*)getChosenImageLocation{

    return chosenImageLocation;
}
@end




extern "C" {

    char* MakeStringCopy (const char* string)
    {
        if (string == NULL)
            return NULL;

        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }

    static CameraHelper *cameraHelper = nil;

    // Converts C style string to NSString
    NSString* CreateNSString (const char* string)
    {
        if (string)
            return [NSString stringWithUTF8String: string];
        else
            return [NSString stringWithUTF8String: ""];
    }

    void _cameraOpeniOSDevice(){

        if(cameraHelper==nil){
            cameraHelper = [[CameraHelper alloc] init];
        }

        [cameraHelper cameraOpen];

    }
    void _galleryOpeniOSDevice(){

        if(cameraHelper==nil){
            cameraHelper = [[CameraHelper alloc] init];
        }

        [cameraHelper galleryOpen];
    }
}
