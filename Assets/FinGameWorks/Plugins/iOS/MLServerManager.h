#import <Foundation/Foundation.h>
#import <ARKit/ARKit.h>

@interface MLServerManager : NSObject

+ (id)sharedManager;

@property (nonatomic,weak) ARSession *arSession;
- (void)setArSession:(ARSession *)arSession;

@end
