param (
    [Parameter(Mandatory=$false, HelpMessage="Name of lox file to interpret.")]
    [string]$fileName,

    [Parameter(Mandatory=$false, HelpMessage="Optional flag to signal if CSLox should be rebuilt before running.")]
    [switch]$build
)
BEGIN
{
    $csLoxProjectDir = "..\src\cslox\CSLox.ConsoleApp\CSLox.ConsoleApp.csproj"
    $csLoxExeDir = "..\src\cslox\CSLox.ConsoleApp\bin\Debug\net6.0\CSLox.ConsoleApp.exe"
}
PROCESS
{
    if ($build)
    {
        dotnet build $csLoxProjectDir
    }

    if ($fileName.Length -gt 0)
    {
        Invoke-Expression "$csLoxExeDir $fileName"
    }
    else
    {
        Invoke-Expression $csLoxExeDir
    }
}
END
{

}