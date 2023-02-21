param (

)
BEGIN
{
    $ast_project = "..\src\cslox\tools\GenerateAst\GenerateAst.csproj"
    $ast_exe = "..\src\cslox\tools\GenerateAst\bin\Debug\net6.0\GenerateAst.exe"
    $out_dir = "..\src\cslox\CSLox"
}
PROCESS
{
    dotnet build $ast_project
    Invoke-Expression "$ast_exe $out_dir"
}
END
{

}