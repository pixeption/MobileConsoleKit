#import "NativeHelper.h"

const char* C_Unknown = "Unknown";
const char* C_Empty = "Unknown";

char* _NHCStringCopy(const char* string) {
    if (string == nullptr)
        return nullptr;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

const char* _NHGetVersionCode()
{
	NSString* versionCode = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleVersion"];
	if (versionCode != nil)
	{
        return _NHCStringCopy([versionCode UTF8String]);
	}
    
	return C_Unknown;
}

const char* _NHGetAllPlayerPrefsKeys(const char* identifier)
{
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSDictionary *defaultAsDic = [defaults persistentDomainForName:[NSString stringWithUTF8String:identifier]];
    NSArray *keyArr = [defaultAsDic allKeys];
    NSString *joinedKeys = [keyArr componentsJoinedByString:@","];
    
    // Safe check
    if (joinedKeys == nil)
        return C_Empty;
    
    return _NHCStringCopy([joinedKeys UTF8String]);
}