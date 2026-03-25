@echo off
setlocal

set "ROOT=%~dp0"
set "COMMAND=%~1"

if "%COMMAND%"=="" goto :help

if /I "%COMMAND%"=="frontend" goto :frontend
if /I "%COMMAND%"=="frontend:build" goto :frontend_build
if /I "%COMMAND%"=="frontend:install" goto :frontend_install
if /I "%COMMAND%"=="backend" goto :backend
if /I "%COMMAND%"=="backend:test" goto :backend_test
if /I "%COMMAND%"=="backend:restore" goto :backend_restore
if /I "%COMMAND%"=="backend:watch" goto :backend_watch
if /I "%COMMAND%"=="help" goto :help

echo Unknown command: %COMMAND%
echo.
goto :help

:frontend
pushd "%ROOT%frontend"
call pnpm.cmd dev
set "EXIT_CODE=%ERRORLEVEL%"
popd
exit /b %EXIT_CODE%

:frontend_build
pushd "%ROOT%frontend"
call pnpm.cmd build
set "EXIT_CODE=%ERRORLEVEL%"
popd
exit /b %EXIT_CODE%

:frontend_install
pushd "%ROOT%frontend"
call pnpm.cmd install
set "EXIT_CODE=%ERRORLEVEL%"
popd
exit /b %EXIT_CODE%

:backend
dotnet run --project "%ROOT%backend\FinanceApp.API\FinanceApp.API.csproj"
exit /b %ERRORLEVEL%

:backend_watch
dotnet watch --project "%ROOT%backend\FinanceApp.API\FinanceApp.API.csproj" run
exit /b %ERRORLEVEL%

:backend_test
dotnet test "%ROOT%backend\FinanceApp.slnx"
exit /b %ERRORLEVEL%

:backend_restore
dotnet restore "%ROOT%backend\FinanceApp.slnx"
exit /b %ERRORLEVEL%

:help
echo FinanceApp root commands
echo.
echo   .\finance.cmd frontend          Start frontend dev server
echo   .\finance.cmd frontend:install  Install frontend dependencies
echo   .\finance.cmd frontend:build    Build frontend
echo   .\finance.cmd backend           Run backend API
echo   .\finance.cmd backend:watch     Run backend API with dotnet watch
echo   .\finance.cmd backend:test      Run backend tests
echo   .\finance.cmd backend:restore   Restore backend packages
echo.
exit /b 0
