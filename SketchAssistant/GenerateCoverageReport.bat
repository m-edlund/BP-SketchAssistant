if not exist "%~dp0GeneratedReports" mkdir "%~dp0GeneratedReports"

"%~dp0\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe" ^
-register:user ^
-target:"%~dp0\packages\Microsoft.TestPlatform.16.0.0\tools\net451\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" ^
-targetargs:"%~dp0\WhiteTests\bin\Debug\WhiteTests.dll" ^
-filter:"+[SketchAssistantWPF*]*" ^
-mergebyhash ^
-output:"%~dp0\GeneratedReports\opencovertests.xml"

"%~dp0\packages\ReportGenerator.4.0.14\tools\net47\ReportGenerator.exe" ^
-reports:"%~dp0\GeneratedReports\opencovertests.xml" ^
-targetdir:"%~dp0\GeneratedReports\ReportGeneratorOutput"
