#!/bin/sh

path=$(dirname $0)
name=$(basename ${path})
archiveName=${name}.xcarchive

cd ${path}
date >> 1.log
echo ${path} >> 1.log
echo ${name} >> 1.log
echo ${archiveName} >> 1.log

security set-keychain-settings -t 3600 -l ~/Library/Keychains/login.keychain-db
security unlock-keychain -p "qwe" ~/Library/Keychains/login.keychain-db

xcodebuild clean -UseNewBuildSystem=NO >> 1.log
xcodebuild archive -scheme "Unity-iPhone" -configuration "Release" -archivePath ${archiveName} >> 1.log
xcodebuild -exportArchive -archivePath ${archiveName} -exportPath ${name} -exportOptionsPlist Info.plist >> 1.log

