call BuildForRelease.cmd %1
call CreateNuGetPackages.cmd %1
call PushNuGetToMyGet.cmd %1
