#import "MLServerManager.h"
#import <CoreML/CoreML.h>
#import "<#MODELNAME#>.h"
#import <Vision/Vision.h>

@interface MLServerManager()

@property (nonatomic,strong) <#MODELNAME#> *ModelInstance;
@property (nonatomic,strong) NSTimer *MLTimer;
@property (nonatomic,strong) VNRequest *vnRequest;
@property (nonatomic,strong) VNCoreMLModel *vmModel;

@end

@implementation MLServerManager

#pragma mark Singleton Methods

+ (id)sharedManager {
    static MLServerManager *sharedMyManager = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedMyManager = [[self alloc] init];
    });
    return sharedMyManager;
}

- (id)init {
    if (self = [super init]) {
        self.ModelInstance = [[<#MODELNAME#> alloc] init];
        NSError *vnMLModelErr;
        self.vmModel = [VNCoreMLModel modelForMLModel:self.ModelInstance.model error:&vnMLModelErr];
        
        if(vnMLModelErr){NSLog(@"%@",vnMLModelErr.description);}
        
        self.vnRequest = [[VNCoreMLRequest alloc] initWithModel:self.vmModel completionHandler:^(VNRequest *req, NSError *vnRequestErr)
                          {
                              if (vnRequestErr)
                              {
                                  NSLog(@"%@",vnRequestErr.description);
                              }
                              NSOperationQueue *quene = [[NSOperationQueue alloc] init];
                              [quene addOperationWithBlock:^
                               {
                                   
                                   NSArray<VNClassificationObservation *> * res = req.results;
                                   NSMutableDictionary *mutableDictionary = [[NSMutableDictionary alloc] init];
                                   
                                   NSArray<VNClassificationObservation *> *sortedRes;
                                   sortedRes = [res sortedArrayUsingComparator:^NSComparisonResult(VNClassificationObservation * second, VNClassificationObservation * first)
                                                {
                                                    if (first.confidence > second.confidence)
                                                    {
                                                        return NSOrderedDescending;
                                                    } else if (first.confidence < second.confidence)
                                                    {
                                                        return NSOrderedAscending;
                                                    } else {
                                                        return NSOrderedSame;
                                                    }
                                                }];
                                   //  NSLog(@"Prediction High = %f, Low = %f",[sortedRes firstObject].confidence,[sortedRes lastObject].confidence);
                                   sortedRes = [sortedRes subarrayWithRange:NSMakeRange(0, 5)];
                                   for (VNClassificationObservation *observation in sortedRes)
                                   {
                                       // NSLog(@"%@ %f",observation.identifier,observation.confidence);
                                       
                                       [mutableDictionary setObject:[NSNumber numberWithFloat:observation.confidence] forKey:observation.identifier];
                                   }
                                   
                                   NSError* jsonErr = nil;
                                   NSData *data = [NSJSONSerialization dataWithJSONObject:mutableDictionary options:NSJSONWritingPrettyPrinted error:&jsonErr];
                                   NSString *jsonString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
                                   //NSLog(@"%@",jsonString);
                                   if (jsonErr) {
                                       NSLog(@"%@",jsonErr);
                                   }
                                   
                                   [[NSOperationQueue mainQueue] addOperationWithBlock:^{
                                       // Main thread work
                                       if (
                                           [NSStringFromClass([self.ModelInstance class]) isEqualToString:@"MobileNet"] ||
                                           [NSStringFromClass([self.ModelInstance class]) isEqualToString:@"SqueezeNet"] ||
                                           [NSStringFromClass([self.ModelInstance class]) isEqualToString:@"Resnet50"] ||
                                           [NSStringFromClass([self.ModelInstance class]) isEqualToString:@"Inceptionv3"]
                                           )
                                       {
                                           UnitySendMessage("Manager", "GetPredictionFromNative", [jsonString UTF8String]);
                                       }
                                       if ([NSStringFromClass([self.ModelInstance class])isEqualToString:@"TinyYOLO"])
                                       {
                                           //http://machinethink.net/blog/yolo-coreml-versus-mps-graph/
                                       }
                                   }];
                               }];
                          }];
        
        self.MLTimer = [NSTimer timerWithTimeInterval:1.0f/5.0f repeats:YES block:^(NSTimer *t){
            NSLog(@"Timer Fired");
            
            if (self.arSession && self.arSession.currentFrame && self.arSession.currentFrame.capturedImage)
            {
                NSLog(@"arSession Not Null, Begin ML Stuff");
                NSError *imageRequestErr;
                VNImageRequestHandler *handler = [[VNImageRequestHandler alloc] initWithCVPixelBuffer:self.arSession.currentFrame.capturedImage options:@{}];
                [handler performRequests:@[self.vnRequest] error:&imageRequestErr];
                if (imageRequestErr)
                {
                    NSLog(@"%@",imageRequestErr.description);
                }
            }else
            {
                NSLog(@"ARFrame CaputerImage = NULL");
            }
        }];
        [[NSRunLoop mainRunLoop] addTimer:self.MLTimer forMode:NSDefaultRunLoopMode];
    }
    return self;
}

- (void)dealloc
{
     [self.MLTimer invalidate];
}

- (void)setArSession:(ARSession *)arSession
{
    _arSession = arSession;
}
@end
