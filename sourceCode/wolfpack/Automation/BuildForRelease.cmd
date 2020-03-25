pushd ..\
msbuild.exe Automation\msbuild\ReleaseSource.proj /p:pProjectFolder=%cd%;pSolutionFile=Wolfpack.sln;pReleaseNumber=%1
popd
