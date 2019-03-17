if not exist "%~dp0GeneratedReports" mkdir "%~dp0GeneratedReports"

for /d %%a in (
  "%~dp0\packages\OpenCover.*"
) do set "openCoverFolder=%%~fa\"

for /d %%a in (
  "%~dp0\packages\Microsoft.TestPlatform.*"
) do set "microPlat=%%~fa\"

for /d %%a in (
  "%~dp0\packages\ReportGenerator.*"
) do set "repGen=%%~fa\"

"%openCoverFolder%\tools\OpenCover.Console.exe" ^
-register:user ^
-target:"%microPlat%\tools\net451\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" ^
-targetargs:"%~dp0\WhiteTests\bin\Debug\WhiteTests.dll" ^
-filter:"+[SketchAssistantWPF*]*" ^
-mergebyhash ^
-output:"%~dp0\GeneratedReports\opencovertests.xml"

"%repGen%\tools\net47\ReportGenerator.exe" ^
-reports:"%~dp0\GeneratedReports\opencovertests.xml" ^
-targetdir:"%~dp0\GeneratedReports\ReportGeneratorOutput"
