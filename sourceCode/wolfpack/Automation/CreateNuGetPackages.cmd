SET "NuGetVersion=%1"
IF "%2" == "" GOTO :notPreRelease
SET "NuGetVersion=%1-%2"

:notPreRelease
ECHO NuGetVersion is %NuGetVersion%

pushd ..\releases\v%1\source

rem WebServices
del NuGet\Templates\Wolfpack.WebServices\lib\net45\delete.me
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.WebServices.Interfaces.dll NuGet\Templates\Wolfpack.WebServices\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.WebServices.Interfaces.pdb NuGet\Templates\Wolfpack.WebServices\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.Interfaces.dll NuGet\Templates\Wolfpack.WebServices\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.Interfaces.pdb NuGet\Templates\Wolfpack.WebServices\lib\net45

rem WebServices Client
del NuGet\Templates\Wolfpack.WebServices.Client\lib\net45\delete.me
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.WebServices.Client.dll NuGet\Templates\Wolfpack.WebServices.Client\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.WebServices.Client.pdb NuGet\Templates\Wolfpack.WebServices.Client\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.WebServices.Interfaces.dll NuGet\Templates\Wolfpack.WebServices.Client\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.WebServices.Interfaces.pdb NuGet\Templates\Wolfpack.WebServices.Client\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.Interfaces.dll NuGet\Templates\Wolfpack.WebServices.Client\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.Interfaces.pdb NuGet\Templates\Wolfpack.WebServices.Client\lib\net45


rem Publishers
del NuGet\Templates\Wolfpack.Publisher\lib\net45\delete.me
copy Wolfpack.Agent\bin\debug\magnum.dll NuGet\Templates\Wolfpack.Publisher\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.dll NuGet\Templates\Wolfpack.Publisher\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.Interfaces.dll NuGet\Templates\Wolfpack.Publisher\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.Interfaces.Magnum.dll NuGet\Templates\Wolfpack.Publisher\lib\net45

rem HealthCheck
del NuGet\Templates\Wolfpack.HealthCheck\lib\net45\delete.me
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.dll NuGet\Templates\Wolfpack.HealthCheck\lib\net45
copy Wolfpack.Agent\bin\debug\Wolfpack.Core.Interfaces.dll NuGet\Templates\Wolfpack.HealthCheck\lib\net45

rem Core.Testing
del NuGet\Templates\Wolfpack.Core.Testing\lib\net45\delete.me
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Agent.exe NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Agent.pdb NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.dll NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.pdb NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.Interfaces.Castle.dll NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.Interfaces.Castle.pdb NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.Interfaces.dll NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.Interfaces.pdb NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.Interfaces.Magnum.dll NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.Interfaces.Magnum.pdb NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.Testing.dll NuGet\Templates\Wolfpack.Core.Testing\lib\net45
copy Wolfpack.Core.Testing\bin\debug\Wolfpack.Core.Testing.pdb NuGet\Templates\Wolfpack.Core.Testing\lib\net45


rem Wolfpack (+Sidewinder)
del NuGet\Templates\Wolfpack\tools\delete.me
del NuGet\Templates\Wolfpack\lib\net45\delete.me
del NuGet\Templates\Wolfpack\content\config\delete.me
del NuGet\Templates\Wolfpack\content\scripts\delete.me
copy Wolfpack.Agent\bin\debug\*.* NuGet\Templates\Wolfpack\lib\net45
del NuGet\Templates\Wolfpack\lib\net45\*.xml
del NuGet\Templates\Wolfpack\lib\net45\*.pdb
move NuGet\Templates\Wolfpack\lib\net45\wolfpack.agent.exe.config NuGet\Templates\Wolfpack\content
xcopy Wolfpack.Agent\bin\debug\config\*.* NuGet\Templates\Wolfpack\content\config /Y /R /I /E
xcopy Wolfpack.Agent\bin\debug\content\*.* NuGet\Templates\Wolfpack\content\content /Y /R /I /E
xcopy Wolfpack.Agent\bin\debug\fonts\*.* NuGet\Templates\Wolfpack\content\fonts /Y /R /I /E
xcopy Wolfpack.Agent\bin\debug\scripts\*.* NuGet\Templates\Wolfpack\content\scripts /Y /R /I /E
xcopy Wolfpack.Agent\bin\debug\views\*.* NuGet\Templates\Wolfpack\content\views /Y /R /I /E


popd
pushd ..\
Automation\NuGet\NuGet.exe pack Releases\v%1\source\NuGet\Templates\Wolfpack.WebServices\Wolfpack.WebServices.nuspec -version %NuGetVersion% -OutputDirectory Releases\v%1
Automation\NuGet\NuGet.exe pack Releases\v%1\source\NuGet\Templates\Wolfpack.WebServices.Client\Wolfpack.WebServices.Client.nuspec -version %NuGetVersion% -OutputDirectory Releases\v%1
Automation\NuGet\NuGet.exe pack Releases\v%1\source\NuGet\Templates\Wolfpack.Publisher\Wolfpack.Publisher.nuspec -version %NuGetVersion% -OutputDirectory Releases\v%1
Automation\NuGet\NuGet.exe pack Releases\v%1\source\NuGet\Templates\Wolfpack.HealthCheck\Wolfpack.HealthCheck.nuspec -version %NuGetVersion% -OutputDirectory Releases\v%1 
Automation\NuGet\NuGet.exe pack Releases\v%1\source\NuGet\Templates\Wolfpack\Wolfpack.nuspec -version %NuGetVersion% -OutputDirectory Releases\v%1 
Automation\NuGet\NuGet.exe pack Releases\v%1\source\NuGet\Templates\Wolfpack.Core.Testing\Wolfpack.Core.Testing.nuspec -version %NuGetVersion% -OutputDirectory Releases\v%1 
popd

