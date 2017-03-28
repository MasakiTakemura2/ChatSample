import os
import shutil

from sys import argv
from mod_pbxproj import XcodeProject
#import appcontroller

path = argv[1]
frameworks = argv[2].split(' ')
cflags = argv[3].split(' ')
ldflags = argv[4].split(' ')
cpfwpath = argv[5]

print('Step 1: add system libraries ')
    #if framework is optional, add `weak=True`
project = XcodeProject.Load(path +'/Unity-iPhone.xcodeproj/project.pbxproj')
for frwrk in frameworks:
    #project.add_file_if_doesnt_exist('System/Library/Frameworks/' + frwrk, tree='SDKROOT')
    if not project.add_file_if_doesnt_exist('System/Library/Frameworks/' + frwrk, tree='SDKROOT'):
        if os.path.isdir(os.path.join(cpfwpath, frwrk)):
            print('Step 1.1: copy framework ' + frwrk)
            if not os.path.isdir(os.path.join(path, frwrk)):
                shutil.copytree(os.path.join(cpfwpath, frwrk), os.path.join(path, frwrk))
                project.add_file_if_doesnt_exist(frwrk, tree='SOURCE_ROOT')

print('Step 2: add CFLAGS ')
for cf in cflags:
	project.add_other_cflags(cf)

print('Step 3: add LDFLAGS ')
for ldf in ldflags:
	project.add_other_ldflags(ldf)

print('Step 4: change build setting')
project.add_other_buildsetting('GCC_ENABLE_OBJC_EXCEPTIONS', 'YES')

print('Step 5: save our change to xcode project file')
if project.modified:
    project.backup()
    project.save_new_format()
