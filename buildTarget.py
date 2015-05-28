#!/usr/bin/python

import sys
import shutil
import os
import fileinput
import time


if len(sys.argv) < 3:
    print 'arg 1 = Target Build Sku'
    print 'arg 2 = Output File Name'
    print 'arg 3 = Unity Project Path'
    sys.exit(1)


#This system only works with Unity 5.x.x

targetBuildSku = sys.argv[1] 	#build target - iOS, Android
outputFileName = sys.argv[2] 	#name of the output file
unityProjectPath = sys.argv[3] 	#this is our project that we are building from


print 'Target Build Sku : ' + targetBuildSku
print 'Output File Name : ' + outputFileName
print 'Unity Project Path : ' + unityProjectPath

os.system("/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath " + unityProjectPath + " -executeMethod AutoBuilder.PerformAndroidBuild " + outputFileName)

print 'cleaning up...'
print 'Operation Complete!'

