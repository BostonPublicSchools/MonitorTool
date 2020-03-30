pushd ..\
msbuild.exe automation\msbuild\ReleaseBinariesZip.proj /p:pProjectFolder=%cd%;pReleaseNumber=%1;pTargetProject=Wolfpack.Agent /t:Zip
ren Releases\v%1\Binaries_v%1.zip Wolfpack_Binaries_v%1.zip
popd